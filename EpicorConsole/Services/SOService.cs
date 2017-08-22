using EpicorConsole.Data;
using EpicorConsole.Epicor.SalesOrderSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class SOService : BaseService
    {
        SalesOrderSvcContractClient soClient;
        CustomerService customerService;

        public SOService(Guid sessionId)
        {
            this.sessionId = sessionId;
            builder.Path = "ERP101500/Erp/BO/SalesOrder.svc";
            soClient = GetClient<SalesOrderSvcContractClient, SalesOrderSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            soClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));

            customerService = new CustomerService(sessionId);
        }
        
        public async Task SyncSOs()
        {
            log.Information("Syncing SOs...");
            try
            {
                using (var db = new EpicorIntergrationEntities())
                {
                    //Header
                    var addedSOHeaders = db.SO_HEADER.Where(c => c.DMSFlag == "N");
                    if (addedSOHeaders.Any())
                    {
                        foreach (var soHeader in addedSOHeaders)
                        {
                            try
                            {
                                SalesOrderTableset soTableset = new SalesOrderTableset();
                                soClient.GetNewOrderHed(ref soTableset);
                                var soHeaderRow = soTableset.OrderHed.Where(p => p.RowMod == "A").FirstOrDefault();
                                if (soHeaderRow != null)
                                {
                                    MapToHeaderRow(soHeaderRow, soHeader);
                                    await MapToHeaderRowAsync(soHeaderRow, soHeader);
                                    soClient.Update(ref soTableset);
                                    soHeader.DMSFlag = "S";
                                    Console.WriteLine($"Added soHeader: #{soHeader.DocNum} successfully!");

                                    //Details
                                    var soDetails = db.SO_DETAIL.Where(c => c.DocNum == soHeader.DocNum && c.DMSFlag == "N");
                                    foreach(var soDetail in soDetails)
                                    {
                                        try
                                        {
                                            soClient.GetNewOrderDtl(ref soTableset, soHeaderRow.OrderNum);
                                            var soDetailRow = soTableset.OrderDtl.Where(p => p.RowMod == "A").FirstOrDefault();
                                            if (soDetailRow != null)
                                            {
                                                MapToDetailRow(soDetailRow, soDetail);
                                                soClient.Update(ref soTableset);
                                                soDetail.DMSFlag = "S";
                                                Console.WriteLine($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} successfully!");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            soDetail.DMSFlag = "F";
                                            Console.WriteLine($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
                                            Console.WriteLine(e.GetBaseException().Message);
                                            continue;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                soHeader.DMSFlag = "F";
                                Console.WriteLine($"Added soHeader: #{soHeader.DocNum} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private void MapToHeaderRow(OrderHedRow row, SO_HEADER entity)
        {
            row.Company = !string.IsNullOrEmpty(entity.CompanyCode) ? entity.CompanyCode : row.Company;
            row.TermsCode = entity.TermCode;
            row.CurrencyCode = entity.CurrencyCode;
            row.CustomerCustID = !string.IsNullOrEmpty(entity.CustomerCode) ? entity.CustomerCode : row.CustomerCustID;
            //row.BTCustID = !string.IsNullOrEmpty(entity.CustomerCode) ? entity.CustomerCode : row.BTCustID;
            //row.ShipToCustId = !string.IsNullOrEmpty(entity.ShiptoCode) ? entity.ShiptoCode : row.ShipToCustId;
            //row.OrderDate = entity.CreatedDateTime != null ? entity.CreatedDateTime : row.OrderDate;
        }

        private async Task MapToHeaderRowAsync(OrderHedRow row, SO_HEADER entity)
        {
            //row.CustNum = await customerService.GetCustNum(entity.CustomerCode);
            //row.BTCustNum = await customerService.GetCustNum(entity.CustomerCode);
            //row.ShipToCustNum = await customerService.GetCustNum(entity.ShiptoCode);
        }

        private void MapToDetailRow(OrderDtlRow row, SO_DETAIL entity)
        {
            row.Company = !string.IsNullOrEmpty(entity.CompanyCode) ? entity.CompanyCode : row.Company;
            //row.OrderNum = entity.DocNum;
            //row.OrderLine = entity.LineNum;
            //row.LineDesc = entity.LineDesc;
            //row.PartNum = entity.ProductCode != null ? entity.ProductCode : row.PartNum;
            //row.SalesUM = entity.UoM;
            //row.IUM = entity.IUM;
            //row.SellingQuantity = entity.Quantity;
            //row.PlanUserID = entity.User01;
        }
    }
}

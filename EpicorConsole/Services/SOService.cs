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
                    var updatedSOHeaders = db.SO_HEADER.Where(c => c.DMSFlag == "U");
                    if (addedSOHeaders.Any() || updatedSOHeaders.Any())
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
                                    soHeader.DocNum = soTableset.OrderHed[0].OrderNum;
                                    soHeader.DMSFlag = "S";
                                    Console.WriteLine($"Added soHeader: #{soHeader.DocNum} successfully!");

                                    //Update DocNum of Details
                                    var soDetails = db.SO_DETAIL.Where(c => c.HeaderId == soHeader.Id);
                                    foreach(var soDetail in soDetails)
                                    {
                                        soDetail.DocNum = soHeader.DocNum;
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

                        foreach(var soHeader in updatedSOHeaders)
                        {
                            try
                            {
                                SalesOrderTableset soTableset = soClient.GetByID(soHeader.DocNum);
                                var soHeaderRow = soTableset.OrderHed.FirstOrDefault();
                                if (soHeaderRow != null)
                                {
                                    soHeaderRow.RowMod = "U";
                                    MapToHeaderRow(soHeaderRow, soHeader);
                                    await MapToHeaderRowAsync(soHeaderRow, soHeader);
                                    soClient.Update(ref soTableset);
                                    soHeader.DMSFlag = "S";
                                    Console.WriteLine($"Updated soHeader: #{soHeader.DocNum} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                soHeader.DMSFlag = "F";
                                Console.WriteLine($"Updated soHeader: #{soHeader.DocNum} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        await db.SaveChangesAsync();
                    }

                    //Detail
                    var addedSODetails = db.SO_DETAIL.Where(c => c.DMSFlag == "N");
                    var updatedSODetails = db.SO_DETAIL.Where(c => c.DMSFlag == "U");
                    if (addedSODetails.Any() || updatedSODetails.Any())
                    {
                        foreach (var soDetail in addedSODetails)
                        {
                            try
                            {
                                SalesOrderTableset soTableset = new SalesOrderTableset();
                                soClient.GetNewOrderDtl(ref soTableset, soDetail.DocNum);
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

                        foreach (var soDetail in updatedSODetails)
                        {
                            try
                            {
                                SalesOrderTableset soTableset = soClient.GetByID(soDetail.DocNum);
                                var soDetailRow = soTableset.OrderDtl.FirstOrDefault(p => p.OrderLine == soDetail.LineNum);
                                if (soDetailRow != null)
                                {
                                    soDetailRow.RowMod = "U";
                                    MapToDetailRow(soDetailRow, soDetail);
                                    soClient.Update(ref soTableset);
                                    soDetail.DMSFlag = "S";
                                    Console.WriteLine($"Updated soDetail: #{soDetail.DocNum}/{soDetail.LineNum} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                soDetail.DMSFlag = "F";
                                Console.WriteLine($"Updated soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
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
            row.OrderNum = entity.DocNum > 0 ? entity.DocNum : row.OrderNum;
            row.Company = !string.IsNullOrEmpty(entity.CompanyCode) ? entity.CompanyCode : row.Company;
            row.TermsCode = entity.PaymentTerm;
            row.CurrencyCode = entity.Currency;
            row.CustomerCustID = !string.IsNullOrEmpty(entity.CustomerCode) ? entity.CustomerCode : row.CustomerCustID;
            row.BTCustID = !string.IsNullOrEmpty(entity.CustomerCode) ? entity.CustomerCode : row.BTCustID;
            row.ShipToCustId = !string.IsNullOrEmpty(entity.ShiptoCode) ? entity.ShiptoCode : row.ShipToCustId;
            row.OrderDate = entity.CreatedDateTime != null ? entity.CreatedDateTime : row.OrderDate;
        }

        private async Task MapToHeaderRowAsync(OrderHedRow row, SO_HEADER entity)
        {
            row.CustNum = await customerService.GetCustNum(entity.CustomerCode);
            row.BTCustNum = await customerService.GetCustNum(entity.CustomerCode);
            row.ShipToCustNum = await customerService.GetCustNum(entity.ShiptoCode);
        }

        private void MapToDetailRow(OrderDtlRow row, SO_DETAIL entity)
        {
            row.Company = !string.IsNullOrEmpty(entity.CompanyCode) ? entity.CompanyCode : row.Company;
            row.OrderNum = entity.DocNum;
            row.OrderLine = entity.LineNum;
            row.LineDesc = entity.LineDesc;
            row.PartNum = entity.ProductCode != null ? entity.ProductCode : row.PartNum;
            row.SalesUM = entity.UoM;
            row.IUM = entity.IUM;
            row.SellingQuantity = entity.Quantity;
            row.PlanUserID = entity.User01;
        }
    }
}

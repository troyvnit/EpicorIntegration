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
            builder.Path = "Epicor101/Erp/BO/SalesOrder.svc";
            soClient = GetClient<SalesOrderSvcContractClient, SalesOrderSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            soClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));

            customerService = new CustomerService(sessionId);
        }
        
        public async Task SyncSOs()
        {
            log.Information("Syncing SOs...");
            Console.WriteLine($"Syncing SOs...");
            try
            {
                using (var db = new EpicorIntergrationEntities())
                {
                    //Header
                    var addedSOHeaders = db.SO_HEADER.Where(c => c.DMSFlag == "A");
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
                                    soClient.Update(ref soTableset);
                                    var orderNum = soTableset.OrderHed[0].OrderNum;
                                    soHeader.Ordernum = orderNum;
                                    soHeader.DMSFlag = "S";
                                    Console.WriteLine($"Added soHeader: #{soHeader.DocNum} successfully!");

                                    //Details
                                    var soDetails = db.SO_DETAIL.Where(c => c.DocNum == soHeader.DocNum && c.DMSFlag == "A");
                                    foreach(var soDetail in soDetails)
                                    {
                                        try
                                        {
                                            soClient.GetNewOrderDtl(ref soTableset, orderNum);
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
            row.Company = entity.CompanyCode;
            row.TermsCode = entity.TermCode;
            row.CurrencyCode = entity.CurrencyCode;
            row.CustNum = int.Parse(entity.Custnum);
            row.BTCustNum = int.Parse(entity.BTCustnum);
            row.ShipToCustNum = int.Parse(entity.ShiptoCustnum);
            row.OrderDate = entity.DeliveryDate;
        }

        private void MapToDetailRow(OrderDtlRow row, SO_DETAIL entity)
        {
            row.Company = entity.CompanyCode;
            row.LineDesc = entity.LineDesc;
            row.PartNum = entity.ProductCode;
            row.SalesUM = entity.SaleUM;
            row.IUM = entity.IUM;
            row.SellingQuantity = entity.Quantity;
            row.WarehouseCode = entity.WhsCode;
            row.DocUnitPrice = entity.Price;
        }
    }
}

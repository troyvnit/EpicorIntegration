using EpicorConsole.Data;
using EpicorConsole.Epicor.SalesOrderSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            builder.Path = $"{environment}/Erp/BO/SalesOrder.svc";
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
                    var addedSOHeaders = db.SO_HEADER.Where(c => /*c.DMSFlag == "A"*/ c.DMSFlag == "N" || c.DMSFlag == "U");
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
                                    Console.WriteLine($"Added soHeader: #{orderNum} successfully!");

                                    //Details
                                    var soDetails = db.SO_DETAIL.Where(c => c.DocNum == soHeader.DocNum && /*c.DMSFlag == "A"*/ c.DMSFlag == "N" || c.DMSFlag == "U");
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
                                                soDetail.CreatedBy = soTableset.OrderDtl.Max(d => d.OrderLine).ToString();
                                                soDetail.DMSFlag = "S";
                                                Console.WriteLine($"Added soDetail: #{orderNum}/{soDetail.LineNum} successfully!");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            soDetail.DMSFlag = "F";
                                            log.Error($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
                                            Console.WriteLine($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
                                            Console.WriteLine(e.GetBaseException().Message);
                                            continue;
                                        }
                                    }
                                    
                                    foreach (var soDetail in soDetails)
                                    {
                                        try
                                        {
                                            var lineNum = int.Parse(soDetail.CreatedBy);
                                            soClient.GetNewOrderRelTax(ref soTableset, orderNum, lineNum, 1, soDetail.TaxCode, soDetail.RateCode);
                                            soClient.ChangeManualTaxCalc(orderNum, lineNum, 1, soDetail.TaxCode, soDetail.RateCode, ref soTableset);
                                        }
                                        catch (Exception e)
                                        {
                                            soDetail.DMSFlag = "F";
                                            log.Error($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
                                            Console.WriteLine($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
                                            Console.WriteLine(e.GetBaseException().Message);
                                            continue;
                                        }
                                    }
                                    soClient.Update(ref soTableset);

                                    soTableset.OrderHed[0].ReadyToCalc = false;
                                    soTableset.OrderHed[0].RowMod = "U";
                                    soClient.Update(ref soTableset);
                                    
                                    foreach (var soDetail in soDetails.Where(d => d.VATGroup != null))
                                    {
                                        try
                                        {
                                            var lineNum = int.Parse(soDetail.CreatedBy);
                                            var soDetailRow = soTableset.OrderDtl.FirstOrDefault(d => d.OrderLine == lineNum);
                                            soDetailRow.TaxCatID = soDetail.VATGroup;
                                            soDetailRow.RowMod = "U";
                                        }
                                        catch (Exception e)
                                        {
                                            log.Error($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
                                            Console.WriteLine($"Added soDetail: #{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
                                            Console.WriteLine(e.GetBaseException().Message);
                                            continue;
                                        }
                                    }

                                    soTableset.OrderHed[0].ReadyToCalc = true;
                                    soTableset.OrderHed[0].RowMod = "U";
                                    soClient.Update(ref soTableset);
                                }
                            }
                            catch (Exception e)
                            {
                                soHeader.DMSFlag = "F";
                                log.Error($"Added soHeader: #{soHeader.DocNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
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
            row.BaseCurrencyID = entity.CurrencyCode;
            row.CustNum = int.Parse(entity.Custnum);
            row.BTCustNum = int.Parse(entity.BTCustnum);
            row.ShipToCustNum = int.Parse(entity.ShiptoCustnum);
            row.OrderDate = entity.DeliveryDate;
            row.RequestDate = entity.DeliveryDate;
            row.NeedByDate = entity.DeliveryDate;
            row.ShipViaCode = "VAN";
            row.TaxRegionCode = "VAT";
            row.UserChar1 = entity.DocNum;
            var salesRepList = entity.SaleID.Split('~');
            row.SalesRepCode1 = salesRepList.Length > 0 ? salesRepList[0] : null;
            row.SalesRepCode2 = salesRepList.Length > 1 ? salesRepList[1] : null;
            row.SalesRepCode3 = salesRepList.Length > 2 ? salesRepList[2] : null;
            row.SalesRepCode4 = salesRepList.Length > 3 ? salesRepList[3] : null;
            row.SalesRepCode5 = salesRepList.Length > 4 ? salesRepList[4] : null;
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
            row.DiscountPercent = entity.DiscountPercent.HasValue ? entity.DiscountPercent.Value : 0;
            row.PriceListCode = entity.PriceListCode;
            row.BreakListCode = entity.BreakListCode;
            row.DiscBreakListCode = entity.DisBreakListCode;
            row.ProdCode = entity.ProdCode;
        }
    }
}

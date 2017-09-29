using EpicorConsole.Data;
using EpicorConsole.Epicor.SalesOrderSvc;
using EpicorConsole.Epicor.SessionModSvc;
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
        SessionModSvcContractClient sessionModClient;

        public SOService()
        {
            var sessionModService = new SessionModService();
            var sessionId = sessionModService.Login();
            this.sessionId = sessionId;
            this.sessionModClient = sessionModService.sessionModClient;
            builder.Path = $"{environment}/Erp/BO/SalesOrder.svc";
            soClient = GetClient<SalesOrderSvcContractClient, SalesOrderSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            soClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
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
                    var addedSOHeaders = db.SO_HEADER.Where(c => c.DMSFlag == "N" /*c.DMSFlag == "N" || c.DMSFlag == "U"*/).OrderBy(h => h.CompanyCode);
                    if (addedSOHeaders.Any())
                    {
                        foreach (var soHeader in addedSOHeaders)
                        {
                            try
                            {
                                string siteID, siteName, workstationID, workstationDescription, employeeID, countryGroupCode, countryCode, tenantID, companyName, systemCode;

                                var currentCompany = sessionModClient.GetCurrentValues(out companyName, out siteID, out siteName, out employeeID, out workstationID, out workstationDescription, out systemCode, out countryGroupCode, out countryCode, out tenantID);

                                if(currentCompany != soHeader.CompanyCode)
                                {
                                    sessionModClient.SetCompany(soHeader.CompanyCode, out siteID, out siteName, out workstationID, out workstationDescription, out employeeID, out countryGroupCode, out countryCode, out tenantID);
                                }

                                //company = sessionModClient.GetCurrentValues(out companyName, out siteID, out siteName, out employeeID, out workstationID, out workstationDescription, out systemCode, out countryGroupCode, out countryCode, out tenantID);
                                SalesOrderTableset soTableset = new SalesOrderTableset();
                                soClient.GetNewOrderHed(ref soTableset);
                                var soHeaderRow = soTableset.OrderHed.Where(p => p.RowMod == "A").FirstOrDefault();
                                if (soHeaderRow != null)
                                {
                                    MapToHeaderRow(soHeaderRow, soHeader);
                                    soClient.Update(ref soTableset);
                                    var orderNum = soTableset.OrderHed[0].OrderNum;
                                    soHeader.Ordernum = orderNum;
                                    soHeader.LastUpdatedDateTime = DateTime.Now;
                                    soHeader.DMSFlag = "S";
                                    Console.WriteLine($"Added soHeader: [{soHeader.CompanyCode}]#{orderNum} successfully!");

                                    soTableset.OrderHed[0].ReadyToCalc = false;
                                    soTableset.OrderHed[0].RowMod = "U";
                                    soClient.Update(ref soTableset);

                                    //Details
                                    var soDetails = db.SO_DETAIL.Where(c => c.DocNum == soHeader.DocNum && c.DMSFlag == "N" && c.ProductCode != "DISCOUNT"/* c.DMSFlag == "N" || c.DMSFlag == "U"*/).ToList().OrderBy(c => int.Parse(c.LineNum));
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
                                                Console.WriteLine($"Added soDetail: [{soHeader.CompanyCode}]#{orderNum}/{soDetail.LineNum} successfully!");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            soDetail.DMSFlag = "F";
                                            soDetail.SystemLog = $"Added soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}";
                                            log.Error($"Added soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
                                            Console.WriteLine($"Added soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
                                            Console.WriteLine(e.GetBaseException().Message);
                                            continue;
                                        }
                                    }
                                    
                                    foreach (var soDetail in soDetails.Where(d => d.DMSFlag != "F"))
                                    {
                                        try
                                        {
                                            var lineNum = int.Parse(soDetail.CreatedBy);
                                            soClient.GetNewOrderRelTax(ref soTableset, orderNum, lineNum, 1, soDetail.TaxCode, soDetail.RateCode);
                                            soClient.ChangeManualTaxCalc(orderNum, lineNum, 1, soDetail.TaxCode, soDetail.RateCode, ref soTableset);
                                            Console.WriteLine($"Calculate tax soDetail: [{soHeader.CompanyCode}]#{orderNum}/{soDetail.LineNum} successfully!");
                                        }
                                        catch (Exception e)
                                        {
                                            soDetail.DMSFlag = "F";
                                            soDetail.SystemLog = $"Calculate tax soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}";
                                            log.Error($"Calculate tax soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
                                            Console.WriteLine($"Calculate tax soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
                                            Console.WriteLine(e.GetBaseException().Message);
                                            continue;
                                        }
                                    }
                                    soClient.Update(ref soTableset);
                                    
                                    foreach (var soDetail in soDetails.Where(d => d.VATGroup != null && d.DMSFlag != "F"))
                                    {
                                        try
                                        {
                                            var lineNum = int.Parse(soDetail.CreatedBy);
                                            var soDetailRow = soTableset.OrderDtl.FirstOrDefault(d => d.OrderLine == lineNum);
                                            soDetailRow.TaxCatID = soDetail.VATGroup;
                                            soDetailRow.RowMod = "U";
                                            Console.WriteLine($"Updated VATGroup soDetail: [{soHeader.CompanyCode}]#{orderNum}/{soDetail.LineNum} successfully!");
                                        }
                                        catch (Exception e)
                                        {
                                            soDetail.DMSFlag = "F";
                                            soDetail.SystemLog = $"Updated VATGroup soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}";
                                            log.Error($"Updated VATGroup soDetail:[{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
                                            Console.WriteLine($"Updated VATGroup soDetail: [{soHeader.CompanyCode}]#{soDetail.DocNum}/{soDetail.LineNum} failed! - {e.Message}");
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
                                soHeader.SystemLog = $"Added soHeader: [{soHeader.CompanyCode}]#{soHeader.DocNum} failed! - {e.GetBaseException().Message}";
                                log.Error($"Added soHeader: [{soHeader.CompanyCode}]#{soHeader.DocNum} failed! - {e.GetBaseException().Message}", e.GetBaseException());
                                Console.WriteLine($"Added soHeader: [{soHeader.CompanyCode}]#{soHeader.DocNum} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        await db.SaveChangesAsync();
                    }

                    sessionModClient.Logout();
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

using EpicorConsole.Data;
using EpicorConsole.Epicor.POSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class POService : BaseService
    {
        POSvcContractClient poClient;

        public POService(Guid sessionId)
        {
            this.sessionId = sessionId;
            builder.Path = "ERP101500/Erp/BO/PO.svc";
            poClient = GetClient<POSvcContractClient, POSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            poClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }

        [DisableConcurrentExecution(100000)]
        public async Task SyncPOs()
        {
            Console.WriteLine("Syncing POs...");
            try
            {
                using (var db = new EpicorIntegrationEntities())
                {
                    //Header
                    var addedPOHeaders = db.PO_HEADER.Where(c => c.DMSFlag == "N");
                    var updatedPOHeaders = db.PO_HEADER.Where(c => c.DMSFlag == "U");
                    if (addedPOHeaders.Any() || updatedPOHeaders.Any())
                    {
                        foreach (var poHeader in addedPOHeaders)
                        {
                            try
                            {
                                POTableset poTableset = new POTableset();
                                poClient.GetNewPOHeader(ref poTableset);
                                var poHeaderRow = poTableset.POHeader.Where(p => p.RowMod == "A").FirstOrDefault();
                                if (poHeaderRow != null)
                                {
                                    MapToHeaderRow(poHeaderRow, poHeader);
                                    poClient.Update(ref poTableset);
                                    poHeader.PONum = poTableset.POHeader[0].PONum;
                                    poHeader.DMSFlag = "S";
                                    Console.WriteLine($"Added poHeader: #{poHeader.PONum} successfully!");

                                    //Update PONum of Details
                                    var poDetails = db.PO_DETAIL.Where(c => c.HeaderId == poHeader.Id);
                                    foreach (var poDetail in poDetails)
                                    {
                                        poDetail.PONum = poHeader.PONum;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                poHeader.DMSFlag = "F";
                                Console.WriteLine($"Added poHeader: #{poHeader.PONum} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        foreach(var poHeader in updatedPOHeaders)
                        {
                            try
                            {
                                POTableset poTableset = poClient.GetByID(poHeader.PONum);
                                var poHeaderRow = poTableset.POHeader.FirstOrDefault();
                                if (poHeaderRow != null)
                                {
                                    poHeaderRow.RowMod = "U";
                                    MapToHeaderRow(poHeaderRow, poHeader);
                                    poClient.Update(ref poTableset);
                                    poHeader.DMSFlag = "S";
                                    Console.WriteLine($"Updated poHeader: #{poHeader.PONum} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                poHeader.DMSFlag = "F";
                                Console.WriteLine($"Updated poHeader: #{poHeader.PONum} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        await db.SaveChangesAsync();
                    }

                    //Detail
                    var addedPODetails = db.PO_DETAIL.Where(c => c.DMSFlag == "N");
                    var updatedPODetails = db.PO_DETAIL.Where(c => c.DMSFlag == "U");
                    if (addedPODetails.Any() || updatedPODetails.Any())
                    {
                        foreach (var poDetail in addedPODetails)
                        {
                            try
                            {
                                POTableset poTableset = new POTableset();
                                poClient.GetNewPODetail(ref poTableset, poDetail.PONum);
                                var poDetailRow = poTableset.PODetail.Where(p => p.RowMod == "A").FirstOrDefault();
                                if (poDetailRow != null)
                                {
                                    MapToDetailRow(poDetailRow, poDetail);
                                    poClient.Update(ref poTableset);
                                    poDetail.DMSFlag = "S";
                                    Console.WriteLine($"Added poDetail: #{poDetail.PONum}/{poDetail.POLine} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                poDetail.DMSFlag = "F";
                                Console.WriteLine($"Added poDetail: #{poDetail.PONum}/{poDetail.POLine} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        foreach (var poDetail in updatedPODetails)
                        {
                            try
                            {
                                POTableset poTableset = poClient.GetByID(poDetail.PONum);
                                var poDetailRow = poTableset.PODetail.FirstOrDefault(p => p.POLine == poDetail.POLine);
                                if (poDetailRow != null)
                                {
                                    poDetailRow.RowMod = "U";
                                    MapToDetailRow(poDetailRow, poDetail);
                                    poClient.Update(ref poTableset);
                                    poDetail.DMSFlag = "S";
                                    Console.WriteLine($"Updated poDetail: #{poDetail.PONum}/{poDetail.POLine} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                poDetail.DMSFlag = "F";
                                Console.WriteLine($"Updated poDetail: #{poDetail.PONum}/{poDetail.POLine} failed! - {e.Message}");
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

        private void MapToHeaderRow(POHeaderRow row, PO_HEADER entity)
        {
            row.PONum = entity.PONum > 0 ? entity.PONum : row.PONum;
            row.Company = !string.IsNullOrEmpty(entity.CompanyCode) ? entity.CompanyCode : row.Company;
            row.EntryPerson = !string.IsNullOrEmpty(entity.EntryPerson) ? entity.EntryPerson : row.EntryPerson;
            row.POType = !string.IsNullOrEmpty(entity.POType) ? entity.POType : row.POType;
            row.OrderDate = entity.OrderDate != null ? entity.OrderDate : row.OrderDate;
            row.BuyerID = !string.IsNullOrEmpty(entity.BuyerID) ? entity.BuyerID : row.BuyerID;
            row.TermsCode = !string.IsNullOrEmpty(entity.PaymentTerm) ? entity.PaymentTerm : row.TermsCode;
            row.ShipViaCode = !string.IsNullOrEmpty(entity.ShipViaCode) ? entity.ShipViaCode : row.ShipViaCode;
            row.CurrencyCode = !string.IsNullOrEmpty(entity.Currency) ? entity.Currency : row.CurrencyCode;
            row.ExchangeRate = entity.ExchangeRate.HasValue ? entity.ExchangeRate.Value : row.ExchangeRate;
            row.ShipState = !string.IsNullOrEmpty(entity.ShipState) ? entity.ShipState : row.ShipState;
            row.ApprovalStatus = !string.IsNullOrEmpty(entity.ApprovalStatus) ? entity.ApprovalStatus : row.ApprovalStatus;
            row.VendorNum = entity.VendorNumber;
            row.PostDate = entity.PostingDate;
            row.PromiseDate = entity.DeliveryDate;
        }

        private void MapToDetailRow(PODetailRow row, PO_DETAIL entity)
        {
            row.Company = !string.IsNullOrEmpty(entity.CompanyCode) ? entity.CompanyCode : row.Company;
            row.PONUM = entity.PONum;
            row.POLine = entity.POLine;
            row.CalcTranType = !string.IsNullOrEmpty(entity.CalcTranType) ? entity.CalcTranType : row.CalcTranType;
            row.PartNum = entity.PartNum != null ? entity.PartNum : row.PartNum;
            row.QtyOption = !string.IsNullOrEmpty(entity.QtyOption) ? entity.QtyOption : row.QtyOption;
            row.CalcVendQty = entity.CalcVendQty;
            row.PUM = !string.IsNullOrEmpty(entity.PUM) ? entity.PUM : row.PUM;
            row.DocUnitCost = entity.DocUnitCost;
            row.DocScrUnitCost = entity.DocScrUnitCost;
            row.CalcDueDate = entity.CalcDueDate;
            row.Taxable = entity.Taxable;
            row.RcvInspectionReq = entity.RcvInspectionReq;
        }
    }
}

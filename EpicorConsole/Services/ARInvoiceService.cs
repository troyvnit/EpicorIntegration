using AutoMapper;
using EpicorConsole.Data;
using EpicorConsole.Epicor.ARInvoiceSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class ARInvoiceService : BaseService
    {
        ARInvoiceSvcContractClient arInvoiceClient;

        public ARInvoiceService(Guid sessionId)
        {
            this.sessionId = sessionId;
            //builder.Path = $"{environment}/Erp/BO/ARInvoice.svc";
            //arInvoiceClient = GetClient<ARInvoiceSvcContractClient, ARInvoiceSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            //arInvoiceClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }
        
        public async Task SyncARInvoices()
        {
            log.Information("Syncing ARInvoices...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    var arInvoices = erpdb.sptyx_DMSARInvoice();
                    using (var db = new EpicorIntergrationEntities())
                    {
                        var addedARInvoiceHeaders = new List<ARINVOICE_HEADER>();
                        var addedARInvoiceDetails = new List<ARINVOICE_DETAIL>();
                        foreach (var arInvoice in arInvoices)
                        {
                            if (!db.ARINVOICE_HEADER.Any(p => p.CompanyCode == arInvoice.Company && p.DocNum == arInvoice.Docnum.ToString())
                                && !addedARInvoiceHeaders.Any(p => p.CompanyCode == arInvoice.Company && p.DocNum == arInvoice.Docnum.ToString()))
                            {
                                try
                                {
                                    var arInvoiceHeader = new ARINVOICE_HEADER();
                                    MapHeaderToEntity(arInvoiceHeader, arInvoice);
                                    arInvoiceHeader.ARINVOICE_DMSFLAG = "N";
                                    addedARInvoiceHeaders.Add(arInvoiceHeader);
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"Added arInvoiceHeader: #{arInvoice.Docnum}");
                                }
                                catch (Exception e)
                                {
                                    log.Error($"Failed adding arInvoiceHeader: #{arInvoice.Company}/{arInvoice.Docnum} - {e.GetBaseException().Message}", e.GetBaseException());
                                    Console.WriteLine($"Failed adding arInvoiceHeader: #{arInvoice.Company}/{arInvoice.Docnum}");
                                    Console.WriteLine(e.GetBaseException().Message);
                                    continue;
                                }
                            }

                            if (!db.ARINVOICE_DETAIL.Any(p => p.CompanyCode == arInvoice.Company && p.DocNum == arInvoice.Docnum.ToString() && p.LineNum == arInvoice.Linenum.ToString())
                                && !addedARInvoiceDetails.Any(p => p.CompanyCode == arInvoice.Company && p.DocNum == arInvoice.Docnum.ToString() && p.LineNum == arInvoice.Linenum.ToString()))
                            {
                                try
                                {
                                    var arInvoiceDetail = new ARINVOICE_DETAIL();
                                    MapDetailToEntity(arInvoiceDetail, arInvoice);
                                    addedARInvoiceDetails.Add(arInvoiceDetail);
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"Added arInvoiceDetail: #{arInvoice.Docnum}");
                                }
                                catch (Exception e)
                                {
                                    log.Error($"Failed adding arInvoiceDetail: #{arInvoice.Company}/{arInvoice.Docnum} - {e.GetBaseException().Message}", e.GetBaseException());
                                    Console.WriteLine($"Failed adding arInvoiceDetail: #{arInvoice.Company}/{arInvoice.Docnum}");
                                    Console.WriteLine(e.GetBaseException().Message);
                                    continue;
                                }
                            }
                        }

                        if (addedARInvoiceHeaders.Any())
                        {
                            db.ARINVOICE_HEADER.AddRange(addedARInvoiceHeaders);
                        }
                        if (addedARInvoiceDetails.Any())
                        {
                            db.ARINVOICE_DETAIL.AddRange(addedARInvoiceDetails);
                        }
                        await db.SaveChangesAsync();
                    }
                }
                //bool more = true;
                //int page = 0;
                //DateTime expired = DateTime.Now.AddMinutes(10);
                //while (more && expired >= DateTime.Now) 
                //{
                //    page++;
                //    Console.WriteLine($"Working on AR Invoice Header page: #{page}");

                //    var rs = await arInvoiceClient.GetRowsAsync(new Epicor.ARInvoiceSvc.GetRowsRequest()
                //    {
                //        pageSize = 5,
                //        absolutePage = page
                //    });
                //    var result = rs.GetRowsResult;
                //    more = rs.morePages;

                //    using (var db = new EpicorIntergrationEntities())
                //    {
                //        //Header
                //        var invcHeads = result.InvcHead.ToArray();
                //        foreach (var invcHead in invcHeads)
                //        {
                //            var arInvoiceHeader = db.ARINVOICE_HEADER.FirstOrDefault(p => p.InvoiceNum == invcHead.InvoiceNum);
                //            if (arInvoiceHeader == null)
                //            {
                //                arInvoiceHeader = new ARINVOICE_HEADER();
                //                arInvoiceHeader.DMSFlag = "N";
                //                MapHeaderToEntity(arInvoiceHeader, invcHead);
                //                db.ARINVOICE_HEADER.Add(arInvoiceHeader);
                //                Console.WriteLine($"Added AR Invoice Header: #{invcHead.InvoiceNum}");
                //            }
                //            else
                //            {
                //                MapHeaderToEntity(arInvoiceHeader, invcHead);
                //                arInvoiceHeader.DMSFlag = "U";
                //                db.ARINVOICE_HEADER.Attach(arInvoiceHeader);
                //                db.Entry(arInvoiceHeader).State = System.Data.Entity.EntityState.Modified;
                //                Console.WriteLine($"Updated AR Invoice Header: #{invcHead.InvoiceNum}");
                //            }
                //        }

                //        //Detail
                //        var invcDtls = result.InvcDtl.ToArray();
                //        foreach (var invcDtl in invcDtls)
                //        {
                //            var arInvoiceDetail = db.ARINVOICE_DETAIL.FirstOrDefault(p => p.InvoiceNum == invcDtl.InvoiceNum && p.InvoiceLine == invcDtl.InvoiceLine);
                //            if (arInvoiceDetail == null)
                //            {
                //                arInvoiceDetail = new ARINVOICE_DETAIL();
                //                arInvoiceDetail.DMSFlag = "N";
                //                MapDetailToEntity(arInvoiceDetail, invcDtl);
                //                db.ARINVOICE_DETAIL.Add(arInvoiceDetail);
                //                Console.WriteLine($"Added AR Invoice Detail: #{invcDtl.InvoiceNum}");
                //            }
                //            else
                //            {
                //                MapDetailToEntity(arInvoiceDetail, invcDtl);
                //                arInvoiceDetail.DMSFlag = "U";
                //                db.ARINVOICE_DETAIL.Attach(arInvoiceDetail);
                //                db.Entry(arInvoiceDetail).State = System.Data.Entity.EntityState.Modified;
                //                Console.WriteLine($"Updated AR Invoice Detail: #{invcDtl.InvoiceNum}");
                //            }
                //        }

                //        //Save all
                //        await db.SaveChangesAsync();
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private void MapHeaderToEntity(ARINVOICE_HEADER entity, sptyx_DMSARInvoice_Result row)
        {
            entity.CompanyCode = row.Company;
            entity.DocNum = row.Docnum.ToString();
            entity.CustomerCode = row.CustomerCode;
            entity.SalesmanCode = row.SalesmanCode;
            entity.TotalAmount = row.TotalAmount;
            entity.InvoiceDate = row.InvoiceDate;
            entity.Type = row.Type.ToString();
            entity.Remarks = row.Remarks;
            entity.SO_DocNum = row.SO_Docnum;
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }


        private void MapDetailToEntity(ARINVOICE_DETAIL entity, sptyx_DMSARInvoice_Result row)
        {
            entity.CompanyCode = row.Company;
            entity.DocNum = row.Docnum.ToString();
            entity.LineNum = row.Linenum.ToString();
            entity.ProductCode = row.Productcode;
            entity.UoM = row.UOM;
            entity.Quantity = row.OurShipQty;
            entity.Price = row.Price;
            entity.Discount_Amount = row.Discount_Amount;
            entity.Discount_Percent = row.Discount_Percent;
            entity.Amount = row.Amount;
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }
    }
}

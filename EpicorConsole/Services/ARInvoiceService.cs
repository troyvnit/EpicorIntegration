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
            builder.Path = "ERP101500/Erp/BO/ARInvoice.svc";
            arInvoiceClient = GetClient<ARInvoiceSvcContractClient, ARInvoiceSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            arInvoiceClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }
        
        public async Task SyncARInvoices()
        {
            log.Information("Syncing ARInvoices...");
            try
            {
                bool more = true;
                int page = 0;
                DateTime expired = DateTime.Now.AddMinutes(10);
                while (more && expired >= DateTime.Now) 
                {
                    page++;
                    Console.WriteLine($"Working on AR Invoice Header page: #{page}");

                    var rs = await arInvoiceClient.GetRowsAsync(new Epicor.ARInvoiceSvc.GetRowsRequest()
                    {
                        pageSize = 5,
                        absolutePage = page
                    });
                    var result = rs.GetRowsResult;
                    more = rs.morePages;

                    using (var db = new EpicorIntergrationEntities())
                    {
                        //Header
                        var invcHeads = result.InvcHead.ToArray();
                        foreach (var invcHead in invcHeads)
                        {
                            var arInvoiceHeader = db.ARINVOICE_HEADER.FirstOrDefault(p => p.InvoiceNum == invcHead.InvoiceNum);
                            if (arInvoiceHeader == null)
                            {
                                arInvoiceHeader = new ARINVOICE_HEADER();
                                arInvoiceHeader.DMSFlag = "N";
                                MapHeaderToEntity(arInvoiceHeader, invcHead);
                                db.ARINVOICE_HEADER.Add(arInvoiceHeader);
                                Console.WriteLine($"Added AR Invoice Header: #{invcHead.InvoiceNum}");
                            }
                            else
                            {
                                MapHeaderToEntity(arInvoiceHeader, invcHead);
                                arInvoiceHeader.DMSFlag = "U";
                                db.ARINVOICE_HEADER.Attach(arInvoiceHeader);
                                db.Entry(arInvoiceHeader).State = System.Data.Entity.EntityState.Modified;
                                Console.WriteLine($"Updated AR Invoice Header: #{invcHead.InvoiceNum}");
                            }
                        }

                        //Detail
                        var invcDtls = result.InvcDtl.ToArray();
                        foreach (var invcDtl in invcDtls)
                        {
                            var arInvoiceDetail = db.ARINVOICE_DETAIL.FirstOrDefault(p => p.InvoiceNum == invcDtl.InvoiceNum && p.InvoiceLine == invcDtl.InvoiceLine);
                            if (arInvoiceDetail == null)
                            {
                                arInvoiceDetail = new ARINVOICE_DETAIL();
                                arInvoiceDetail.DMSFlag = "N";
                                MapDetailToEntity(arInvoiceDetail, invcDtl);
                                db.ARINVOICE_DETAIL.Add(arInvoiceDetail);
                                Console.WriteLine($"Added AR Invoice Detail: #{invcDtl.InvoiceNum}");
                            }
                            else
                            {
                                MapDetailToEntity(arInvoiceDetail, invcDtl);
                                arInvoiceDetail.DMSFlag = "U";
                                db.ARINVOICE_DETAIL.Attach(arInvoiceDetail);
                                db.Entry(arInvoiceDetail).State = System.Data.Entity.EntityState.Modified;
                                Console.WriteLine($"Updated AR Invoice Detail: #{invcDtl.InvoiceNum}");
                            }
                        }

                        //Save all
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private void MapHeaderToEntity(ARINVOICE_HEADER entity, InvcHeadRow row)
        {
            entity.InvoiceNum = row.InvoiceNum;
            entity.CustomerCode = row.CustomerCustID;
            entity.CompanyCode = row.Company;
            entity.Status = row.LegalNumber;
            entity.TotalAmount = row.DocInvoiceAmt;
            entity.InvoiceDate = row.InvoiceDate;
            entity.Type = row.InvoiceType;
            entity.SO_DocNum = row.OrderNum.ToString();
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }


        private void MapDetailToEntity(ARINVOICE_DETAIL entity, InvcDtlRow row)
        {
            entity.InvoiceNum = row.InvoiceNum;
            entity.InvoiceLine = row.InvoiceLine;
            entity.CompanyCode = row.Company;
            entity.WhsCode = row.WarehouseCode;
            entity.ProductCode = row.PartNum;
            entity.UoM = row.IUM;
            entity.Quantity = row.OurShipQty;
            entity.Price = row.DocUnitPrice;
            entity.Discount_Amount = row.DocDiscount;
            entity.Discount_Percent = row.DiscountPercent;
            entity.SO_DocNum = row.OrderNum.ToString();
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }
    }
}

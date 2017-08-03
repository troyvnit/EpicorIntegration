using EpicorConsole.Data;
using EpicorConsole.Epicor.PartSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class PartService : BaseService
    {
        PartSvcContractClient partClient; 

        public PartService(Guid sessionId)
        {
            this.sessionId = sessionId;
            builder.Path = "ERP101500/Erp/BO/Part.svc";
            partClient = GetClient<PartSvcContractClient, PartSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            partClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }
        
        [DisableConcurrentExecution(100000)]
        public async Task SyncParts()
        {
            Console.WriteLine("Syncing Parts...");
            try
            {
                var rs = await partClient.GetRowsAsync(new Epicor.PartSvc.GetRowsRequest());
                var result = rs.GetRowsResult;
                var parts = result.Part.ToArray();
                using (var db = new EpicorIntegrationEntities())
                {
                    foreach (var part in parts)
                    {
                        var product = db.PRODUCTs.FirstOrDefault(p => p.ItemCode == part.PartNum);
                        if (product == null)
                        {
                            product = new PRODUCT();
                            product.DMSFlag = "N";
                            MapToEntity(product, part);
                            db.PRODUCTs.Add(product);
                            Console.WriteLine($"Added product: #{part.PartNum}");
                        }
                        else
                        {
                            MapToEntity(product, part);
                            product.DMSFlag = "U";
                            db.PRODUCTs.Attach(product);
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            Console.WriteLine($"Updated product: #{part.PartNum}");
                        }
                    }
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private void MapToEntity(PRODUCT entity, PartRow row)
        {
            entity.ItemCode = row.PartNum;
            entity.ItemName = row.PartDescription;
            entity.ForeignName = row.PartDescription;
            entity.Status = row.InActive ? "I" : "A";
            entity.SalesUnit = row.SalesUM;
            entity.SalesVATGroup = row.TaxCatID;
            entity.PurchaseUnit = row.PUM;
            entity.PurchaseItemsPerUnit = row.PurchasingFactor;
            entity.InventoryUOM = row.IUM;
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }
    }
}

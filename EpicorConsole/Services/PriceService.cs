using EpicorConsole.Data;
using EpicorConsole.Epicor.PriceLstPartsSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class PriceService : BaseService
    {
        PriceLstPartsSvcContractClient priceClient;
        List<PriceLst> priceLsts;

        public PriceService(Guid sessionId)
        {
            this.sessionId = sessionId;
            builder.Path = "ERP101500/Erp/BO/PriceLstParts.svc";
            priceClient = GetClient<PriceLstPartsSvcContractClient, PriceLstPartsSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            priceClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }

        [DisableConcurrentExecution(100000)]
        public async Task SyncPrices()
        {
            Console.WriteLine("Syncing Prices...");
            try
            {
                //var rs = await priceClient.GetRowsAsync(new Epicor.PriceLstPartsSvc.GetRowsRequest());
                //var result = rs.GetRowsResult;
                //var priceLstParts = result.PriceLstParts.ToArray();
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    priceLsts = erpdb.PriceLsts.ToList();
                    var priceLstParts = erpdb.PriceLstParts.ToList();
                    using (var db = new EpicorIntegrationEntities())
                    {
                        foreach (var priceLstPart in priceLstParts)
                        {
                            var price = db.PRICE_LIST.FirstOrDefault(p => p.ListCode == priceLstPart.ListCode && p.ItemCode == priceLstPart.PartNum && p.CompanyCode == priceLstPart.Company);
                            if (price == null)
                            {
                                price = new PRICE_LIST();
                                price.DMSFlag = "N";
                                MapToEntity(price, priceLstPart);
                                db.PRICE_LIST.Add(price);
                                Console.WriteLine($"Added price: #{priceLstPart.ListCode}");
                            }
                            else
                            {
                                MapToEntity(price, priceLstPart);
                                price.DMSFlag = "U";
                                db.PRICE_LIST.Attach(price);
                                db.Entry(price).State = System.Data.Entity.EntityState.Modified;
                                Console.WriteLine($"Updated price: #{priceLstPart.ListCode}");
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

        private void MapToEntity(PRICE_LIST entity, PriceLstPart row)
        {
            var priceList = priceLsts.FirstOrDefault(p => p.Company == row.Company && p.ListCode == row.ListCode);
            if(priceList != null)
            {
                entity.PriceListName = priceList.ListDescription;
                entity.Currency = priceList.CurrencyCode;
            }
            entity.ListCode = row.ListCode;
            entity.ItemCode = row.PartNum;
            entity.Price = decimal.Round(row.BasePrice, 0);
            entity.CompanyCode = row.Company;
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }
    }
}

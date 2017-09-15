using AutoMapper;
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
        public PriceService()
        {
        }
        
        public async Task SyncPrices()
        {
            log.Information("Syncing Prices...");
            Console.WriteLine("Syncing Prices...");
            try
            {
                //var rs = await priceClient.GetRowsAsync(new Epicor.PriceLstPartsSvc.GetRowsRequest());
                //var result = rs.GetRowsResult;
                //var priceLstParts = result.PriceLstParts.ToArray();
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    var priceLstParts = erpdb.sptyx_DMSPriceList().ToList();
                    var totalRow = priceLstParts.Count();
                    using (var db = new EpicorIntergrationEntities())
                    {
                        int runningRow = 0;
                        foreach (var priceLstPart in priceLstParts)
                        {
                            runningRow++;
                            var price = db.PRICE_LIST.FirstOrDefault(p => p.Company == priceLstPart.Company && p.PriceListNum == priceLstPart.PriceListNum && p.Partnum == priceLstPart.PartNum);
                            if (price == null)
                            {
                                try
                                {
                                    price = Mapper.Map<PRICE_LIST>(priceLstPart);
                                    price.DMSFlag = "N";
                                    db.PRICE_LIST.Add(price);
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"[{runningRow}/{totalRow}]Added price: #{priceLstPart.Company}/{priceLstPart.PriceListNum}/{priceLstPart.PartNum}");
                                }
                                catch (Exception e)
                                {
                                    log.Error($"Failed adding price: #{priceLstPart.Company}/{priceLstPart.PriceListNum}/{priceLstPart.PartNum} - {e.GetBaseException().Message}", e.GetBaseException());
                                    Console.WriteLine($"Failed adding price: #{priceLstPart.Company}/{priceLstPart.PriceListNum}/{priceLstPart.PartNum}");
                                    Console.WriteLine(e.GetBaseException().Message);
                                    continue;
                                }
                            }
                            else
                            {
                                try
                                {
                                    Mapper.Map(priceLstPart, price);
                                    price.DMSFlag = "U";
                                    db.PRICE_LIST.Attach(price);
                                    db.Entry(price).State = System.Data.Entity.EntityState.Modified;
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"[{runningRow}/{totalRow}]Updated price: #{priceLstPart.Company}/{priceLstPart.PriceListNum}/{priceLstPart.PartNum}");
                                }
                                catch (Exception e)
                                {
                                    log.Error($"Failed updating price: #{priceLstPart.Company}/{priceLstPart.PriceListNum}/{priceLstPart.PartNum} - {e.GetBaseException().Message}", e.GetBaseException());
                                    Console.WriteLine($"Failed updating price: #{priceLstPart.PriceListNum}");
                                    Console.WriteLine(e.GetBaseException().Message);
                                    continue;
                                }
                            }
                        }
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                log.Error($"System error: {e.GetBaseException().Message}", e.GetBaseException());
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
    }
}

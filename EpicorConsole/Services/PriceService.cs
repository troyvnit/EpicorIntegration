﻿using AutoMapper;
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
                    var priceLstParts = erpdb.sptyx_DMSPriceList();
                    using (var db = new ERPIntegrationEntities())
                    {
                        foreach (var priceLstPart in priceLstParts)
                        {
                            var price = db.PRICE_LIST.FirstOrDefault(p => p.Company == priceLstPart.Company && p.PriceListNum == priceLstPart.PriceListNum && p.Partnum == priceLstPart.PartNum);
                            if (price == null)
                            {
                                try
                                {
                                    price = Mapper.Map<PRICE_LIST>(priceLstPart);
                                    price.DMSFlag = "N";
                                    db.PRICE_LIST.Add(price);
                                    await db.SaveChangesAsync();
                                    Console.WriteLine($"Added price: #{priceLstPart.PriceListNum}");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Failed adding price: #{priceLstPart.PriceListNum}");
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
                                    await db.SaveChangesAsync();
                                    Console.WriteLine($"Updated price: #{priceLstPart.PriceListNum}");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Failed updating price: #{priceLstPart.PriceListNum}");
                                    Console.WriteLine(e.GetBaseException().Message);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
    }
}

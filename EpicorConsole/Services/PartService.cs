using AutoMapper;
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
        }
        
        [DisableConcurrentExecution(100000)]
        public async Task SyncParts()
        {
            Console.WriteLine("Syncing Parts...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    var parts = erpdb.xvtyx_DMSProduct.ToList();
                    using (var db = new EpicorIntergrationEntities())
                    {
                        foreach (var part in parts)
                        {
                            var product = db.PRODUCTs.FirstOrDefault(p => p.ItemCode == part.ItemCode && p.Company == part.Company);
                            if (product == null)
                            {
                                try
                                {
                                    product = Mapper.Map<PRODUCT>(part);
                                    product.DMSFlag = "N";
                                    db.PRODUCTs.Add(product);
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"Added product: #{part.ItemCode}");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Failed adding product: #{part.ItemCode}");
                                    Console.WriteLine(e.GetBaseException().Message);
                                    continue;
                                }
                            }
                            else
                            {
                                try
                                {
                                    Mapper.Map(part, product);
                                    product.DMSFlag = "U";
                                    db.PRODUCTs.Attach(product);
                                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"Updated product: #{part.ItemCode}");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Failed updating product: #{part.ItemCode}");
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
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
    }
}

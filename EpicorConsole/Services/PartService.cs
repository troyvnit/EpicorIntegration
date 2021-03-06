﻿using AutoMapper;
using EpicorConsole.Data;
using EpicorConsole.Epicor.PartSvc;
using Hangfire;
using Hangfire.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class PartService : BaseService
    {
        public PartService()
        {
        }
        
        public async Task SyncParts()
        {
            log.Information("Syncing Parts...");
            Console.WriteLine("Syncing Parts...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    using (var db = new EpicorIntergrationEntities())
                    {
                        var sysRevID = db.PRODUCTs.Max(p => p.SysRevID);
                        var ud_SysRevID = db.PRODUCTs.Max(p => p.UD_SysRevID);
                        var parts = erpdb.sptyx_DMSProduct(sysRevID, ud_SysRevID).ToList();
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
                                    log.Error($"Failed adding product: #{part.ItemCode} - {e.GetBaseException().Message}", e.GetBaseException());
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
                                    log.Error($"Failed updating product: #{part.ItemCode} - {e.GetBaseException().Message}", e.GetBaseException());
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
                log.Error($"System error: {e.GetBaseException().Message}", e.GetBaseException());
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
    }
}

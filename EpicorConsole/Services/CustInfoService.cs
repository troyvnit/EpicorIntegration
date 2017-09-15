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
    public class CustInfoService : BaseService
    {
        public CustInfoService()
        {
        }
        
        public async Task SyncCustInfos()
        {
            log.Information("Syncing CustInfos...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    var custInfos = erpdb.sptyx_DMSCustInfo().ToList();
                    var totalRow = custInfos.Count();
                    using (var db = new EpicorIntergrationEntities())
                    {
                        int runningRow = 0;
                        foreach (var custInfo in custInfos)
                        {
                            runningRow++;
                            var cod = db.CUSTOMER_INFO.FirstOrDefault(p => p.Custnum == custInfo.Custnum && p.Company == custInfo.Company);
                            if (cod == null)
                            {
                                try
                                {
                                    cod = Mapper.Map<CUSTOMER_INFO>(custInfo);
                                    cod.DMSFlag = "N";
                                    db.CUSTOMER_INFO.Add(cod);
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"[{runningRow}/{totalRow}]Added Cust Info: #{custInfo.Company}/{custInfo.Custnum}");
                                }
                                catch (Exception e)
                                {
                                    log.Error($"Failed adding Cust Info: #{custInfo.Company}/{custInfo.Custnum}", e.GetBaseException());
                                    Console.WriteLine($"Failed adding Cust Info: #{custInfo.Custnum}");
                                    Console.WriteLine(e.GetBaseException().Message);
                                    continue;
                                }
                            }
                            else
                            {
                                try
                                {
                                    Mapper.Map(custInfo, cod);
                                    cod.DMSFlag = "U";
                                    db.CUSTOMER_INFO.Attach(cod);
                                    db.Entry(cod).State = System.Data.Entity.EntityState.Modified;
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"[{runningRow}/{totalRow}]Updated Cust Info: #{custInfo.Company}/{custInfo.Custnum}");
                                }
                                catch (Exception e)
                                {
                                    log.Error($"Failed updating Cust Info: #{custInfo.Company}/{custInfo.Custnum}", e.GetBaseException());
                                    Console.WriteLine($"Failed updating Cust Info: #{custInfo.Company}/{custInfo.Custnum}");
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

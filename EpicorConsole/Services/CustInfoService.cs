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

        [DisableConcurrentExecution(100000)]
        public async Task SyncCustInfos()
        {
            Console.WriteLine("Syncing CustInfos...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    var custInfos = erpdb.sptyx_DMSCustInfo();
                    using (var db = new EpicorIntergrationEntities())
                    {
                        foreach (var custInfo in custInfos)
                        {
                            var cod = db.CUSTOMER_INFO.FirstOrDefault(p => p.Custnum == custInfo.Custnum && p.Company == custInfo.Company);
                            if (cod == null)
                            {
                                try
                                {
                                    cod = Mapper.Map<CUSTOMER_INFO>(custInfo);
                                    cod.DMSFlag = "N";
                                    db.CUSTOMER_INFO.Add(cod);
                                    //await db.SaveChangesAsync();
                                    Console.WriteLine($"Added Cust Info: #{custInfo.Custnum}");
                                }
                                catch (Exception e)
                                {
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
                                    Console.WriteLine($"Updated Cust Info: #{custInfo.Custnum}");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Failed updating Cust Info: #{custInfo.Custnum}");
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

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
                    using (var db = new EpicorIntegrationEntities())
                    {
                        foreach (var custInfo in custInfos)
                        {
                            var cod = db.CUSTOMER_INFO.FirstOrDefault(p => p.Custnum == custInfo.Custnum && p.Company == custInfo.Company);
                            if (cod == null)
                            {
                                cod = Mapper.Map<CUSTOMER_INFO>(custInfo);
                                cod.DMSFlag = "N";
                                db.CUSTOMER_INFO.Add(cod);
                                Console.WriteLine($"Added Cust Over Due: #{custInfo.Custnum}");
                            }
                            else
                            {
                                Mapper.Map(custInfo, cod);
                                cod.DMSFlag = "U";
                                db.CUSTOMER_INFO.Attach(cod);
                                db.Entry(cod).State = System.Data.Entity.EntityState.Modified;
                                Console.WriteLine($"Updated Cust Over Due: #{custInfo.Custnum}");
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

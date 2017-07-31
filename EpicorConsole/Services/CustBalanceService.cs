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
    public class CustBalanceService : BaseService
    {
        public CustBalanceService()
        {
        }

        [DisableConcurrentExecution(100000)]
        public async Task SyncCustBalances()
        {
            Console.WriteLine("Syncing CustBalances...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    var custBalances = erpdb.sptyx_DMSCustBalance();
                    using (var db = new EpicorIntegrationEntities())
                    {
                        foreach (var custBalance in custBalances)
                        {
                            var cb = db.CUST_BALANCE.FirstOrDefault(p => p.CustomerCode == custBalance.CustID);
                            if (cb == null)
                            {
                                cb = new CUST_BALANCE();
                                cb.DMSFlag = "N";
                                MapToEntity(cb, custBalance);
                                db.CUST_BALANCE.Add(cb);
                                Console.WriteLine($"Added Cust Balance: #{custBalance.CustID}");
                            }
                            else
                            {
                                MapToEntity(cb, custBalance);
                                cb.DMSFlag = "U";
                                db.CUST_BALANCE.Attach(cb);
                                db.Entry(cb).State = System.Data.Entity.EntityState.Modified;
                                Console.WriteLine($"Updated Cust Balance: #{custBalance.CustID}");
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

        private void MapToEntity(CUST_BALANCE entity, sptyx_DMSCustBalance_Result row)
        {
            entity.CompanyCode = row.Company;
            entity.CustomerId = row.Custnum;
            entity.CustomerCode = row.CustID;
            entity.CreditLimit = row.CreditLimit;
            entity.Balance = (decimal)(row.Balance.GetValueOrDefault());
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }
    }
}

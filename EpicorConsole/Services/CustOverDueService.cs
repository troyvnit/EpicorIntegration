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
    public class CustOverDueService : BaseService
    {
        public CustOverDueService()
        {
        }
        
        public async Task SyncCustOverDues()
        {
            log.Information("Syncing CustOverDues...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    var custOverDues = erpdb.sptyx_DMSCustOverDue();
                    using (var db = new EpicorIntergrationEntities())
                    {
                        foreach (var custOverDue in custOverDues)
                        {
                            var cod = db.CUST_OVER_DUE.FirstOrDefault(p => p.CustomerCode == custOverDue.CustID);
                            if (cod == null)
                            {
                                cod = new CUST_OVER_DUE();
                                cod.DMSFlag = "N";
                                MapToEntity(cod, custOverDue);
                                db.CUST_OVER_DUE.Add(cod);
                                Console.WriteLine($"Added Cust Over Due: #{custOverDue.CustID}");
                            }
                            else
                            {
                                MapToEntity(cod, custOverDue);
                                cod.DMSFlag = "U";
                                db.CUST_OVER_DUE.Attach(cod);
                                db.Entry(cod).State = System.Data.Entity.EntityState.Modified;
                                Console.WriteLine($"Updated Cust Over Due: #{custOverDue.CustID}");
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

        private void MapToEntity(CUST_OVER_DUE entity, sptyx_DMSCustOverDue_Result row)
        {
            entity.CompanyCode = row.Company;
            entity.CustomerId = row.Custnum.GetValueOrDefault();
            entity.CustomerCode = row.CustID;
            entity.PaymentTerm = row.PaymentTerm;
            entity.CreditLimit = (decimal)(row.CreditLimit.GetValueOrDefault());
            entity.BalanceDue = (decimal)(row.Balance.GetValueOrDefault());
            entity.OverDue = (decimal)(row.OverDue.GetValueOrDefault());
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }
    }
}

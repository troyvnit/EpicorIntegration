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
    public class SOCancelService : BaseService
    {
        public SOCancelService()
        {
        }
        
        public async Task SyncSOCancels()
        {
            Console.WriteLine("Syncing SOCancels...");
            try
            {
                using (var erpdb = new ERPAPPTRAINEntities())
                {
                    erpdb.sptyx_SOCancel();
                    Console.WriteLine("Synced SOCancels...");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
    }
}

using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EpicorConsole.Epicor.AbcCodeSvc;
using EpicorConsole.Epicor.PartSvc;
using EpicorConsole.Epicor.SessionModSvc;
using System;
using System.Threading.Tasks;
using EpicorConsole.Data;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using EpicorConsole.Services;
using Hangfire;
using Hangfire.Common;

namespace EpicorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var sessionModService = new SessionModService();
            var sessionId = sessionModService.Login();

            Console.WriteLine("Logged in");

            var partService = new PartService(sessionId);
            var priceService = new PriceService(sessionId);
            var customerService = new CustomerService(sessionId);

            var connectionString = ConfigurationManager.ConnectionStrings["EpicorHangfire"].ConnectionString;
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            using (var server = new BackgroundJobServer())
            {
                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                
                //RecurringJob.AddOrUpdate(() => partService.SyncParts().Wait(), Cron.Minutely);
                //RecurringJob.AddOrUpdate(() => priceService.SyncPrices().Wait(), Cron.Minutely);
                RecurringJob.AddOrUpdate(() => customerService.SyncCustomers().Wait(), Cron.Minutely);

                Console.ReadKey();
            }
        }
    }
}

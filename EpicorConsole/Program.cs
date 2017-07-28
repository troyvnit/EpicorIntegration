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

            var poService = new POService(sessionId);

            var connectionString = ConfigurationManager.ConnectionStrings["EpicorHangfire"].ConnectionString;
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            using (var server = new BackgroundJobServer())
            {
                Console.WriteLine("Hangfire Server started. Press any key to exit...");

                RecurringJob.AddOrUpdate(() => DoSyncPart(sessionId), Cron.Minutely);
                RecurringJob.AddOrUpdate(() => DoSyncPrice(sessionId), Cron.Minutely);
                RecurringJob.AddOrUpdate(() => DoSyncCustomer(sessionId), Cron.Minutely);
                RecurringJob.AddOrUpdate(() => DoSyncPO(sessionId), Cron.Minutely);

                Console.ReadKey();
            }
        }

        public static async Task DoSyncPart(Guid sessionId)
        {
            var partService = new PartService(sessionId);
            await partService.SyncParts();
        }

        public static async Task DoSyncPrice(Guid sessionId)
        {
            var priceService = new PriceService(sessionId);
            await priceService.SyncPrices();
        }

        public static async Task DoSyncCustomer(Guid sessionId)
        {
            var customerService = new CustomerService(sessionId);
            await customerService.SyncCustomers();
        }

        public static async Task DoSyncPO(Guid sessionId)
        {
            var poService = new POService(sessionId);
            await poService.SyncPOs();
        }
    }
}

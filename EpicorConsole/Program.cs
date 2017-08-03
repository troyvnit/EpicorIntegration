﻿using System.Net;
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

                //Remove if exists
                RecurringJob.RemoveIfExists("DoSyncPart");
                RecurringJob.RemoveIfExists("DoSyncPrice");
                RecurringJob.RemoveIfExists("DoSyncCustomer");
                RecurringJob.RemoveIfExists("DoSyncPO");
                RecurringJob.RemoveIfExists("DoSyncPart");
                RecurringJob.RemoveIfExists("DoSyncCustBalance");
                RecurringJob.RemoveIfExists("DoSyncCustOverDue");
                RecurringJob.RemoveIfExists("DoSyncPartTran");

                //Add or update
                //RecurringJob.AddOrUpdate("DoSyncPart", () => DoSyncPart(sessionId), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncPrice", () => DoSyncPrice(sessionId), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncCustomer", () => DoSyncCustomer(sessionId), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncPO", () => DoSyncPO(sessionId), Cron.Minutely);
                RecurringJob.AddOrUpdate("DoSyncARInvoice", () => DoSyncARInvoice(sessionId), Cron.MinuteInterval(5));
                RecurringJob.AddOrUpdate("DoSyncCustBalance", () => DoSyncCustBalance(), Cron.Minutely);
                RecurringJob.AddOrUpdate("DoSyncCustOverDue", () => DoSyncCustOverDue(), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncPartTran", () => DoSyncPartTran(sessionId), Cron.Minutely);

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

        public static async Task DoSyncARInvoice(Guid sessionId)
        {
            var arInvoiceService = new ARInvoiceService(sessionId);
            await arInvoiceService.SyncARInvoices();
        }

        public static async Task DoSyncCustBalance()
        {
            var custBalanceService = new CustBalanceService();
            await custBalanceService.SyncCustBalances();
        }

        public static async Task DoSyncCustOverDue()
        {
            var custOverDueService = new CustOverDueService();
            await custOverDueService.SyncCustOverDues();
        }

        public static async Task DoSyncPartTran(Guid sessionId)
        {
            var partTranService = new PartTranService(sessionId);
            await partTranService.SyncPartTrans();
        }
    }
}

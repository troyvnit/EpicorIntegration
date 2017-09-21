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
using AutoMapper;
using Hangfire.Storage;

namespace EpicorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<sptyx_DMSProduct_Result, PRODUCT>().ForMember(p => p.Id, o => o.UseDestinationValue());
                cfg.CreateMap<sptyx_DMSPriceList_Result, PRICE_LIST>().ForMember(p => p.Id, o => o.UseDestinationValue());
                cfg.CreateMap<sptyx_DMSCustInfo_Result, CUSTOMER_INFO>().ForMember(p => p.Id, o => o.UseDestinationValue());
            });

            Console.WriteLine("Logged in");

            var connectionString = ConfigurationManager.ConnectionStrings["EpicorHangfire"].ConnectionString;
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            using (var server = new BackgroundJobServer())
            {
                Console.WriteLine("Hangfire Server started. Press any key to exit...");

                //Remove if exists
                //using (var connection = JobStorage.Current.GetConnection())
                //{
                //    foreach (var recurringJob in connection.GetRecurringJobs())
                //    {
                //        RecurringJob.RemoveIfExists(recurringJob.Id);
                //        Console.WriteLine("Removed Job: " + recurringJob.Id);
                //    }
                //}

                //Add or update
                //RecurringJob.AddOrUpdate("DoSyncPart", () => DoSyncPart(), Cron.Daily(21));
                //RecurringJob.AddOrUpdate("DoSyncPrice", () => DoSyncPrice(), Cron.Daily(21));
                //RecurringJob.AddOrUpdate("DoSyncCustInfo", () => DoSyncCustInfo(), Cron.Daily(21));
                //RecurringJob.AddOrUpdate("DoSyncCustomer", () => DoSyncCustomer(sessionId), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncPO", () => DoSyncPO(sessionId), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncSO", () => DoSyncSO(), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncARInvoice", () => DoSyncARInvoice(), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncSOCancel", () => DoSyncSOCancel(), Cron.MinuteInterval(15));
                //RecurringJob.AddOrUpdate("DoSyncCustOverDue", () => DoSyncCustOverDue(), Cron.Minutely);
                //RecurringJob.AddOrUpdate("DoSyncPartTran", () => DoSyncPartTran(), Cron.Minutely);
                //DoSyncSO(sessionId, sessionModService.sessionModClient).Wait();
                //DoSyncPrice().Wait();
                //DoSyncCustInfo().Wait();
                //DoSyncPart().Wait();
                //DoSyncARInvoice().Wait();
                //DoSyncPartTran().Wait();
                DoSyncCustomer().Wait();
                Console.ReadKey();
            }
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncPart()
        {
            var partService = new PartService();
            await partService.SyncParts();
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncPrice()
        {
            var priceService = new PriceService();
            await priceService.SyncPrices();
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncCustomer()
        {
            var customerService = new CustomerService();
            await customerService.SyncCustomers();
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncPO(Guid sessionId)
        {
            var poService = new POService(sessionId);
            await poService.SyncPOs();
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncSO()
        {
            var hour = DateTime.Now.Hour;
            if (hour >= 8 && hour <= 20)
            {
                var soService = new SOService();
                await soService.SyncSOs();
            }
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncARInvoice()
        {
            var hour = DateTime.Now.Hour;
            if (hour >= 8 && hour <= 20)
            {
                var arInvoiceService = new ARInvoiceService();
                await arInvoiceService.SyncARInvoices();
            }
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncSOCancel()
        {
            var hour = DateTime.Now.Hour;
            if (hour >= 8 && hour <= 20)
            {
                var soCancelService = new SOCancelService();
                await soCancelService.SyncSOCancels();
            }
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncCustInfo()
        {
            var custInfoService = new CustInfoService();
            await custInfoService.SyncCustInfos();
        }

        [DisableConcurrentExecution(100000)]
        public static async Task DoSyncPartTran()
        {
            var hour = DateTime.Now.Hour;
            if (hour >= 8 && hour <= 20)
            {
                var users = new string[] { "pmn", "pms", "gvn", "gvs", "gvc", "gbn", "grv" };
                foreach (var user in users)
                {
                    var partTranService = new PartTranService();
                    await partTranService.SyncPartTrans(user.ToUpper());
                }
            }
        }
    }
}

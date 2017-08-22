using EpicorConsole.Services;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EpicorWeb.Services
{
    public class SyncService
    {
        public void Run()
        {
            var sessionModService = new SessionModService();
            var sessionId = sessionModService.Login();

            RecurringJob.AddOrUpdate("DoSyncPart", () => DoSyncPart(sessionId), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncPrice", () => DoSyncPrice(sessionId), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncCustomer", () => DoSyncCustomer(sessionId), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncPO", () => DoSyncPO(sessionId), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncSO", () => DoSyncSO(sessionId), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncARInvoice", () => DoSyncARInvoice(sessionId), Cron.MinuteInterval(5));
            //RecurringJob.AddOrUpdate("DoSyncCustBalance", () => DoSyncCustBalance(), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncCustOverDue", () => DoSyncCustOverDue(), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncCustInfo", () => DoSyncCustInfo(), Cron.Minutely);
            //RecurringJob.AddOrUpdate("DoSyncPartTran", () => DoSyncPartTran(sessionId), Cron.Minutely);
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncPart(Guid sessionId)
        {
            var partService = new PartService(sessionId);
            await partService.SyncParts();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncPrice(Guid sessionId)
        {
            var priceService = new PriceService(sessionId);
            await priceService.SyncPrices();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncCustomer(Guid sessionId)
        {
            var customerService = new CustomerService(sessionId);
            await customerService.SyncCustomers();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncPO(Guid sessionId)
        {
            var poService = new POService(sessionId);
            await poService.SyncPOs();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncSO(Guid sessionId)
        {
            var soService = new SOService(sessionId);
            await soService.SyncSOs();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncARInvoice(Guid sessionId)
        {
            var arInvoiceService = new ARInvoiceService(sessionId);
            await arInvoiceService.SyncARInvoices();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncCustBalance()
        {
            var custBalanceService = new CustBalanceService();
            await custBalanceService.SyncCustBalances();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncCustOverDue()
        {
            var custOverDueService = new CustOverDueService();
            await custOverDueService.SyncCustOverDues();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncCustInfo()
        {
            var custInfoService = new CustInfoService();
            await custInfoService.SyncCustInfos();
        }

        [DisableConcurrentExecution(100000)]
        public async Task DoSyncPartTran(Guid sessionId)
        {
            var partTranService = new PartTranService(sessionId);
            await partTranService.SyncPartTrans();
        }
    }
}
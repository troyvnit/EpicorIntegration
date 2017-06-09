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

            var connectionString = ConfigurationManager.ConnectionStrings["EpicorHangfire"].ConnectionString;
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            using (var server = new BackgroundJobServer())
            {
                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                
                RecurringJob.AddOrUpdate(() => partService.SyncParts().Wait(), Cron.Minutely);
                RecurringJob.AddOrUpdate(() => priceService.SyncPrices().Wait(), Cron.Minutely);

                Console.ReadKey();
            }

            //var manager = new RecurringJobManager();
            //manager.AddOrUpdate("SyncParts", Job.FromExpression(() => partService.SyncParts().Wait()), Cron.Minutely());
            //manager.AddOrUpdate("SyncPrices", Job.FromExpression(() => priceService.SyncPrices().Wait()), Cron.Minutely());

            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => { return true; };
            //EndpointBindingType bindingType = EndpointBindingType.SOAPHttp;
            //string epicorUserID = "dms.user03"; string epiorUserPassword = "tri@2017";
            //string scheme = "http"; if (bindingType == EndpointBindingType.BasicHttp) { scheme = "https"; }
            //UriBuilder builder = new UriBuilder(scheme, "DMS_SERVER.greenvet.com");
            //builder.Path = "ERP101500/Ice/Lib/SessionMod.svc";
            //SessionModSvcContractClient sessionModClient = GetClient<SessionModSvcContractClient, SessionModSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            //builder.Path = "ERP101500/Erp/BO/Part.svc";
            //PartSvcContractClient partClient = GetClient<PartSvcContractClient, PartSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            //Guid sessionId = Guid.Empty;
            //sessionId = sessionModClient.Login();
            //builder.Path = "ERP100500/Ice/Lib/SessionMod.svc";
            //sessionModClient = GetClient<SessionModSvcContractClient, SessionModSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            //sessionModClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
            //partClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));


            //PartTableset partTableset = new PartTableset();
            //partClient.GetNewPart(ref partTableset);
            //var newPart = partTableset.Part.Where(p => p.RowMod == "A").FirstOrDefault();
            //if (newPart != null)
            //{
            //    newPart.PartDescription = "Troy Lee";
            //    newPart.PartNum = "99999";
            //    newPart.TypeCode = "P";
            //    newPart.IUM = "GOI";
            //    newPart.PUM = "GOI";
            //    newPart.SalesUM = "GOI";
            //}
            //partClient.Update(ref partTableset);

            //var line = Console.ReadLine();
            //if(line == "y")
            //{
            //    GetParts(partClient).Wait();
            //}

            //Console.ReadLine();
        }
    }
}

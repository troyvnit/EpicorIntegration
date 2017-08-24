using AutoMapper;
using EpicorConsole.Data;
using Hangfire;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(EpicorWeb.Startup))]
namespace EpicorWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var connectionString = ConfigurationManager.ConnectionStrings["EpicorHangfire"].ConnectionString;
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<sptyx_DMSProduct_Result, PRODUCT>().ForMember(p => p.Id, o => o.UseDestinationValue());
                cfg.CreateMap<sptyx_DMSPriceList_Result, PRICE_LIST>().ForMember(p => p.Id, o => o.UseDestinationValue());
                cfg.CreateMap<sptyx_DMSCustInfo_Result, CUSTOMER_INFO>().ForMember(p => p.Id, o => o.UseDestinationValue());
            });
        }
    }
}

using AutoMapper;
using EpicorConsole.Data;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Serilog.Sinks;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using System.Configuration;

namespace EpicorConsole.Services
{
    public class BaseService
    {
        protected string epicorUserID = "dms";
        protected string epiorUserPassword = "dmsgreenvet";
        protected UriBuilder builder;
        protected EndpointBindingType bindingType = EndpointBindingType.SOAPHttp;
        protected string scheme = "http";
        protected string environment = "Epicor101";
        protected Guid sessionId;
        protected ILogger log;

        public BaseService()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EpicorHangfire"].ConnectionString;
            log = new LoggerConfiguration().WriteTo.MSSqlServer(connectionString, "Logs", autoCreateSqlTable: true).CreateLogger();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => { return true; };
            builder = new UriBuilder(scheme, "ERP.greenvet.com");
        }

        public enum EndpointBindingType { SOAPHttp, BasicHttp }

        private static WSHttpBinding GetWsHttpBinding()
        {
            var binding = new WSHttpBinding();
            const int maxBindingSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = maxBindingSize;
            binding.ReaderQuotas.MaxDepth = maxBindingSize;
            binding.ReaderQuotas.MaxStringContentLength = maxBindingSize;
            binding.ReaderQuotas.MaxArrayLength = maxBindingSize;
            binding.ReaderQuotas.MaxBytesPerRead = maxBindingSize;
            binding.ReaderQuotas.MaxNameTableCharCount = maxBindingSize;
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            return binding;
        }

        public static BasicHttpBinding GetBasicHttpBinding()
        {
            var binding = new BasicHttpBinding();
            const int maxBindingSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = maxBindingSize;
            binding.ReaderQuotas.MaxDepth = maxBindingSize;
            binding.ReaderQuotas.MaxStringContentLength = maxBindingSize;
            binding.ReaderQuotas.MaxArrayLength = maxBindingSize;
            binding.ReaderQuotas.MaxBytesPerRead = maxBindingSize;
            binding.ReaderQuotas.MaxNameTableCharCount = maxBindingSize;
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            return binding;
        }

        protected static TClient GetClient<TClient, TInterface>(string url, string username, string password, EndpointBindingType bindingType) where TClient : ClientBase<TInterface> where TInterface : class
        {
            Binding binding = null; TClient client;
            var endpointAddress = new EndpointAddress(url);
            switch (bindingType)
            {
                case EndpointBindingType.BasicHttp:
                    binding = GetBasicHttpBinding();
                    break;
                case EndpointBindingType.SOAPHttp:
                    binding = GetWsHttpBinding();
                    break;
            }
            client = (TClient)Activator.CreateInstance(typeof(TClient), binding, endpointAddress);
            if (!string.IsNullOrEmpty(username) && (client.ClientCredentials != null))
            {
                client.ClientCredentials.UserName.UserName = username;
                client.ClientCredentials.UserName.Password = password;
            }
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            return client;
        }
    }
}

using EpicorConsole.Epicor.SessionModSvc;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace EpicorConsole.Services
{
    public class SessionModService : BaseService
    {
        public SessionModSvcContractClient sessionModClient;

        public SessionModService()
        {
            builder.Path = $"{environment}/Ice/Lib/SessionMod.svc";
            sessionModClient = GetClient<SessionModSvcContractClient, SessionModSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
        }

        public Guid Login()
        {
            sessionId = sessionModClient.Login();
            builder.Path = $"{environment}/Ice/Lib/SessionMod.svc";
            sessionModClient = GetClient<SessionModSvcContractClient, SessionModSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            sessionModClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
            return sessionId;
        }
    }
}

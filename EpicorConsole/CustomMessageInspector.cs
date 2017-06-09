using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace EpicorConsole
{
    class CustomMessageInspector : IClientMessageInspector
    {
        private Guid _sessionID;
        private string _epicorUserId;

        public CustomMessageInspector(Guid SessionId, string EpicorUserId)
        {
            _sessionID = SessionId;
            _epicorUserId = EpicorUserId;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
        public object BeforeSendRequest(ref Message request, System.ServiceModel.IClientChannel channel)
        {
            if (_sessionID != null && _sessionID != Guid.Empty)
            {
                var sessionHeader = new SessionInfoHeader() {
                    SessionId = _sessionID,
                    EpicorUserId = _epicorUserId
                };
                request.Headers.Add(sessionHeader);
            }
            return request;
        }

    }

    class SessionInfoHeader : MessageHeader
    {
        public Guid SessionId { get; set; }
        public string EpicorUserId { get; set; }

        protected override void OnWriteHeaderContents(System.Xml.XmlDictionaryWriter writer, MessageVersion messageVersion) {
            writer.WriteElementString("SessionID", @"http://schemas.datacontract .org/2004/07/Epicor.Hosting", SessionId.ToString());
            writer.WriteElementString("UserID", @"http://schemas.datacontract.or g/2004/07/Epicor.Hosting", EpicorUserId);
        }
        public override string Name { get { return "SessionInfo"; } }
        public override string Namespace
        { get { return "urn:epic:headers:SessionInfo"; } }
    }

    class HookServiceBehavior : IEndpointBehavior
    {
        private Guid _sessionId;
        private string _epicorUserId;

        public HookServiceBehavior(Guid SessionId, string EpicorUserId)
        {
            _sessionId = SessionId;
            _epicorUserId = EpicorUserId;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new CustomMessageInspector(_sessionId, _epicorUserId));
        }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
        public void Validate(ServiceEndpoint endpoint) { }
    }

}

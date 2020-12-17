using Limilabs.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using MailingLib.Protocol;
namespace MailingLib
{
    public abstract class BaseConnector<T> where T : ClientBase, new()
    {

        private readonly IProtocolCommunicationStrategyFactory _specificMethodFactory;
        public BaseConnector(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory)
        {
            _specificMethodFactory = protocolSpecificMethodFactory;
        }
        protected void StartConnection(T clientBase, TransportProtocol transportProtocol, string hostName, string userName, string password, int port)
        {
            var protocolSpecificMethods = _specificMethodFactory.CreateFor(clientBase);
            if (transportProtocol == TransportProtocol.Unencrypted)
            {
                clientBase.Connect(hostName, port);
            }
            else if (transportProtocol == TransportProtocol.SSLTLS)
            {
                clientBase.ConnectSSL(hostName, port);
            }
            else
            {
                clientBase.Connect(hostName, port);
                protocolSpecificMethods.StartTls(clientBase);                
            }
            protocolSpecificMethods.Login(clientBase, userName, password);
            protocolSpecificMethods.InboxSelect(clientBase);
        }
        protected void CloseConnection(T clientBase)
        {
            _specificMethodFactory.CreateFor(clientBase).CloseConnection(clientBase);
        }
    }
}

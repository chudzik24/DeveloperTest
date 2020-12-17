using Limilabs.Client;
using Limilabs.Client.POP3;

namespace MailingLib.Protocol
{
    public class Pop3CommunicationStrategy : IProtocolCommunicationStrategy
    {
        public void CloseConnection(ClientBase clientBase)
        {
            ((Pop3)clientBase).Close();
        }

        public void InboxSelect(ClientBase clientBase)
        {        
        }

        public void Login(ClientBase clientBase, string userName, string password)
        {
            ((Pop3)clientBase).UseBestLogin(userName, password);
        }

        public void StartTls(ClientBase clientBase)
        {
            ((Pop3)clientBase).StartTLS();
        }
    }
}

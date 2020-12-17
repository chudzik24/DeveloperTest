using Limilabs.Client;
using Limilabs.Client.IMAP;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingLib.Protocol
{
    public class ImapCommunicationStrategy : IProtocolCommunicationStrategy
    {
        public void CloseConnection(ClientBase clientBase)
        {
           ((Imap)clientBase).Close();
        }

        public void InboxSelect(ClientBase clientBase)
        {
            ((Imap)clientBase).SelectInbox();
        }

        public void Login(ClientBase clientBase, string userName, string password)
        {
            ((Imap)clientBase).UseBestLogin(userName, password);
        }

        public void StartTls(ClientBase clientBase)
        {
            ((Imap)clientBase).StartTLS();
        }
    }
}

using Limilabs.Client;

namespace MailingLib.Protocol
{
    public interface IProtocolCommunicationStrategy
    {
        void StartTls(ClientBase clientBase);
        void CloseConnection(ClientBase clientBase);
        void InboxSelect(ClientBase clientBase);
        void Login(ClientBase clientBase, string userName, string password);
    }
}

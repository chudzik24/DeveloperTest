using Limilabs.Client;
using Limilabs.Client.IMAP;
using Limilabs.Client.POP3;
using System;

namespace MailingLib.Protocol
{
    public class ProtocolCommunicationStrategyFactory : IProtocolCommunicationStrategyFactory
    {
        public IProtocolCommunicationStrategy CreateFor<T>(T clientBase) where T : ClientBase
        {
            if (clientBase is Imap)
            {
                return new ImapCommunicationStrategy();
            }
            else if (clientBase is Pop3)
            {
                return new Pop3CommunicationStrategy();
            }
            throw new InvalidOperationException("Wrong protocol");
        }
    }
}

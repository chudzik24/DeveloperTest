using Limilabs.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingLib.Protocol
{
    public interface IProtocolCommunicationStrategyFactory
    {
        IProtocolCommunicationStrategy CreateFor<T>(T clientBase) where T : ClientBase;
    }
}

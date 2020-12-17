using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingLib.Protocol
{
    public enum TransportProtocol
    {
        Unencrypted,
        SSLTLS,
        STARTTLS
    }
}

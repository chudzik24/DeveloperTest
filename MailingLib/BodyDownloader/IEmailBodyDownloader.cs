using MailingLib.Models;
using MailingLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingLib.BodyDownloader
{
    public interface IEmailBodyDownloader : IDisposable
    {
        void Connect(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port);
        EmailBody GetBody(string headerId);
        void Disconnect();
    }
}

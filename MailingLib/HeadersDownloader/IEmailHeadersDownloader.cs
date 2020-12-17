using Limilabs.Client.IMAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailingLib.Protocol;
using MailingLib.Models;
namespace MailingLib.HeadersDownloader
{
    public interface IEmailHeadersDownloader : IDisposable
    {
        void Connect(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port);
        List<EmailHeader> GetHeaders(List<string> headerIds);
        List<string> FetchHeaderIds();
        void Disconnect();
    }
}

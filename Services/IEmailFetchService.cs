using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailingLib;
using MailingLib.BodyDownloader;
using MailingLib.HeadersDownloader;
using MailingLib.Protocol;
using MailingLib.Models;
using System;

namespace Services
{
    public interface IEmailFetchService
    {
        IObservable<List<EmailBody>> EmailBodiesObservable { get; }
        IObservable<List<EmailHeader>> EmailHeaderObservable { get; }
        IObservable<bool> IsProcessingObservable { get; }
        void RequestEmailByHeaderId(string id);
        Task FetchAllEmails(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port, CancellationToken ct);
    }
}

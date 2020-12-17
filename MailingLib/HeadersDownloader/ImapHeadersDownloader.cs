using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Limilabs.Client;
using Limilabs.Client.IMAP;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using MailingLib.Protocol;
using MailingLib.Models;

namespace MailingLib.HeadersDownloader
{
    public class ImapHeadersDownloader : BaseEmailHeadersDownloader<Imap>, IEmailHeadersDownloader
    {
        public ImapHeadersDownloader(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory) : base(protocolSpecificMethodFactory) { }
    
        protected override List<EmailHeader> ExtractHeaders(Imap clientBase, List<string> ids)
        {
            List<MessageInfo> infos = clientBase.GetMessageInfoByUID(ids.Select(z=>Convert.ToInt64(z)).ToList());
            var results = infos.Select(z => new EmailHeader { From = z.Envelope.From.Select(e => e.Address).ToList(), To = z.Envelope.To.Select(e => e.Name).ToList(), Subject = z.Envelope.Subject, Id = z.UID.ToString(), Date=z.Envelope.Date }).ToList();
            return results;
        }

        protected override List<string> ExtractHeadersIds(Imap clientBase)
        {
            clientBase.SelectInbox();
            return clientBase.Search(Flag.Unseen).Select(e=>e.ToString()).ToList();
        }
    }
}


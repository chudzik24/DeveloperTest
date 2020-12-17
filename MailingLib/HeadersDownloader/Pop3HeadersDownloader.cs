using Limilabs.Client.POP3;
using Limilabs.Mail;
using MailingLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailingLib.Models;
namespace MailingLib.HeadersDownloader
{
    public class Pop3HeadersDownloader : BaseEmailHeadersDownloader<Pop3>, IEmailHeadersDownloader
    {
        public Pop3HeadersDownloader(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory) : base(protocolSpecificMethodFactory) { }
        protected override List<EmailHeader> ExtractHeaders(Pop3 clientBase, List<string> ids)
        {
            var cnt = ids.Count;

            MailBuilder builder = new MailBuilder();
            var results = ids.Select(b => new
            {
                Envelop = builder.CreateFromEml(clientBase.GetHeadersByUID(b.ToString())),
                Id = b
            }
            ).Select(z => new EmailHeader { From = z.Envelop.From.Select(e => e.Address).ToList(), To = z.Envelop.To.Select(e => e.Name).ToList(), Subject = z.Envelop.Subject, Id = z.Id, Date = z.Envelop.Date }).ToList();
            return results;
        }
        protected override List<string> ExtractHeadersIds(Pop3 clientBase)
        {
            MailBuilder builder = new MailBuilder();
            return clientBase.GetAll();
        }
    }
}

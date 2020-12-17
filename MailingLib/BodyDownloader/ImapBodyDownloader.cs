using Limilabs.Client.IMAP;
using MailingLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailingLib.Models;
namespace MailingLib.BodyDownloader
{
    public class ImapBodyDownloader : BaseBodyDownloader<Imap>
    {
        public ImapBodyDownloader(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory) : base(protocolSpecificMethodFactory) { }

        protected override EmailBody ExtractBody(Imap clientBase, string id)
        {
            var bodyStructure = clientBase.GetBodyStructureByUID(Convert.ToInt64(id));
            return new EmailBody
            {
                Body = (bodyStructure.Text != null ? clientBase.GetTextByUID(bodyStructure.Text) : clientBase.GetTextByUID(bodyStructure.Html)) ?? string.Empty,
                HeaderId = id
            };
        }
    }
}

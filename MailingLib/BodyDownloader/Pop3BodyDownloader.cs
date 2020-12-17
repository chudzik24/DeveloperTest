using Limilabs.Client.IMAP;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using MailingLib.Protocol;
using MailingLib.Models;
namespace MailingLib.BodyDownloader
{
    public class Pop3BodyDownloader : BaseBodyDownloader<Pop3>
    {
        private readonly MailBuilder _mailBuilder = new MailBuilder();
        public Pop3BodyDownloader(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory) : base(protocolSpecificMethodFactory)
        {
        }
        protected override EmailBody ExtractBody(Pop3 clientBase, string id)
        {
            var bodyStructure = _mailBuilder.CreateFromEml(clientBase.GetMessageByUID(id));
            return new EmailBody
            {
                Body = (!string.IsNullOrEmpty(bodyStructure.Text) ? bodyStructure.GetBodyAsText() : bodyStructure.GetBodyAsHtml()) ?? string.Empty,
                HeaderId = id
            };
        }
    }
}

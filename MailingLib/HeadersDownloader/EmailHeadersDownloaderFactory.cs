using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailingLib.Protocol;
namespace MailingLib.HeadersDownloader
{
    public class EmailHeadersDownloaderFactory : IEmailHeadersDownloaderFactory
    {
        private readonly IProtocolCommunicationStrategyFactory _protocolSpecificMethodFactory;

        public EmailHeadersDownloaderFactory(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory)
        {
            _protocolSpecificMethodFactory = protocolSpecificMethodFactory;
        }


        public IEmailHeadersDownloader CreateForProtocol(EmailProtocol protocol)
        {
            if (protocol == EmailProtocol.IMAP)
            {
                return new ImapHeadersDownloader(_protocolSpecificMethodFactory);
            }
            return new Pop3HeadersDownloader(_protocolSpecificMethodFactory);
        }
    }
}

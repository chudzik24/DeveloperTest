using MailingLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingLib.BodyDownloader
{
    public class EmailBodyDownloaderFactory : IEmailBodyDownloaderFactory
    {
        private readonly IProtocolCommunicationStrategyFactory _protocolSpecificMethodFactory;

        public EmailBodyDownloaderFactory(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactoryd)
        {
            _protocolSpecificMethodFactory = protocolSpecificMethodFactoryd;
        }
        public IEmailBodyDownloader CreateForProtocol(EmailProtocol protocol)
        {
            if (protocol == EmailProtocol.IMAP)
            {
                return new ImapBodyDownloader(_protocolSpecificMethodFactory);
            }
            return new Pop3BodyDownloader(_protocolSpecificMethodFactory);
        }
    }
}

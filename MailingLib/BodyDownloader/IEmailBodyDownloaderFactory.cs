using MailingLib.Protocol;

namespace MailingLib.BodyDownloader
{
    public interface IEmailBodyDownloaderFactory
    {
        IEmailBodyDownloader CreateForProtocol(EmailProtocol protocol);
    }
}

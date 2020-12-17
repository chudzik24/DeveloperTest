using Limilabs.Client;
using Limilabs.Client.IMAP;
using MailingLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using MailingLib.Models;
namespace MailingLib.HeadersDownloader
{
    public abstract class BaseEmailHeadersDownloader<T> : BaseConnector<T>, IEmailHeadersDownloader where T:ClientBase, new()
    {
        protected T _clientBase;
        public BaseEmailHeadersDownloader(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory) : base(protocolSpecificMethodFactory) { }

        public void Connect(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port)
        {
            Disconnect();
            _clientBase = new T();
            StartConnection(_clientBase, transportProtocol, hostName, userName, password, port);
        }
        public List<EmailHeader> GetHeaders(List<string> headerIds)
        {
            return ExtractHeaders(_clientBase, headerIds);
        }     

        protected abstract List<EmailHeader> ExtractHeaders(T clientBase, List<string> ids);
        protected abstract List<string> ExtractHeadersIds(T clientBase);

        public List<string> FetchHeaderIds()
        {
            return ExtractHeadersIds(_clientBase);
        }
        public void Disconnect()
        {
            if (_clientBase?.Connected == true)
            {
                CloseConnection(_clientBase);
            }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _clientBase?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}

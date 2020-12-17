using Limilabs.Client;
using MailingLib.Models;
using MailingLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingLib.BodyDownloader
{
    public abstract class BaseBodyDownloader<T> : BaseConnector<T>, IEmailBodyDownloader where T : ClientBase, new()
    {
        protected T _clientBase;

        public BaseBodyDownloader(IProtocolCommunicationStrategyFactory protocolSpecificMethodFactory) : base(protocolSpecificMethodFactory) {  }
    
        protected abstract EmailBody ExtractBody(T clientBase, string id);

        public void Connect(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port)
        {
            Disconnect();
            _clientBase = new T();
            StartConnection(_clientBase, transportProtocol, hostName, userName, password, port);
        }

        public EmailBody GetBody(string headerId)
        {
            return ExtractBody(_clientBase, headerId);
        }

        public void Disconnect()
        {
            if (_clientBase?.Connected == true)
            {
                CloseConnection(_clientBase);
            }
        }

        #region IDisposable Support
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
        #endregion
    }
}

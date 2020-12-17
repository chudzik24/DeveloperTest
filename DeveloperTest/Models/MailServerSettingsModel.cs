using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MailingLib.Protocol;
namespace DeveloperTest.Models
{
    public class MailServerSettingsModel : ObservableObject
    {
        public IEnumerable<EmailProtocol> Protocols
        {
            get
            {
                return Enum.GetValues(typeof(EmailProtocol)).Cast<EmailProtocol>();
            }
        }

        public IEnumerable<TransportProtocol> TransportProtocols
        {
            get
            {
                return Enum.GetValues(typeof(TransportProtocol)).Cast<TransportProtocol>();
            }
        }
        public string ServerName {
            get
            {
                return _serverName;
            }
            set
            {
                Set(() => ServerName, ref _serverName, value);
            }
        }
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                Set(() => UserName, ref _userName, value);
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                Set(() => Password, ref _password, value);
            }
        }
        public int PortNumber
        {
            get
            {
                return _portNumber;
            }
            set
            {
                Set(() => PortNumber, ref _portNumber, value);
            }
        }
        public TransportProtocol TransportProtocol
        {
            get
            {
                return _transportProtocol;
            }
            set
            {
                Set(() => TransportProtocol, ref _transportProtocol, value);
            }
        }
        public EmailProtocol Protocol
        {
            get
            {
                return _protocol;
            }
            set
            {
                Set(() => Protocol, ref _protocol, value);
            }
        }
        private string _serverName;
        private string _userName;
        private string _password;
        private int _portNumber;
        private TransportProtocol _transportProtocol;
        private EmailProtocol _protocol;
        public MailServerSettingsModel()
        {
            PortNumber = 993;
        }
    }
}

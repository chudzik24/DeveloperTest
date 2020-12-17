using GalaSoft.MvvmLight;
using System.Windows.Input;
using DeveloperTest.Models;
using System.Collections.ObjectModel;
using Services;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Collections.Generic;
using DeveloperTest.Commands;
using System.Windows;
using System.Reactive.Concurrency;
using MailingLib.Models;
using GalaSoft.MvvmLight.Threading;
using DeveloperTest.Utils;

namespace DeveloperTest.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        public ObservableCollection<EmailHeaderModel> HeadersCollection { get; set; } = new ObservableCollection<EmailHeaderModel>();
        public ICommand StartCommand { get; }
        public EmailHeaderModel SelectedHeader
        {
            get
            {
                return _selectedHeader;
            }
            set
            {
                Set(() => SelectedHeader, ref _selectedHeader, value);
                SetActiveBodyForHeader();
            }
        }
        public string DisplayedBody
        {
            get
            {
                return _displayedBody;
            }
            set
            {
                Set(() => DisplayedBody, ref _displayedBody, value);
            }
        }

        private bool _isRunEnabled = true;

        public bool IsRunEnabled
        {
            get
            {
                return _isRunEnabled;
            }
            set
            {
                Set(() => IsRunEnabled, ref _isRunEnabled, value);
            }
        }
        public MailServerSettingsModel ServerSettings { get; } = new MailServerSettingsModel();
        private readonly Dictionary<string, string> _headerIdToContentMapping = new Dictionary<string, string>();
        private readonly IEmailFetchService _emailFetchService;
        private readonly IDispatcherSchedulerProvider _dispatcherSchedulerProvider;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private string _displayedBody;
        private EmailHeaderModel _selectedHeader;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IEmailFetchService emailFetchService, IDispatcherSchedulerProvider dispatcherSchedulerProvider)
        {
            StartCommand = new AsyncCommand(ExecuteAsync, ()=>true);
            _emailFetchService = emailFetchService;
            _dispatcherSchedulerProvider = dispatcherSchedulerProvider;
            _compositeDisposable.Add(emailFetchService.EmailHeaderObservable
                .ObserveOn(_dispatcherSchedulerProvider.DispatcherScheduler).Subscribe(z =>
            {
                z.ForEach(a => HeadersCollection.Add(new EmailHeaderModel
                {
                    Title = a.Subject,
                    HeaderId = a.Id,
                    Date = a.Date,
                    From = string.Join(",",a.From)
                }));
            }));
            
            _compositeDisposable.Add(emailFetchService.EmailBodiesObservable
                .ObserveOn(_dispatcherSchedulerProvider.DispatcherScheduler).Subscribe(z =>
            {
                z.ForEach(e =>
                {
                    if (!_headerIdToContentMapping.ContainsKey(e.HeaderId)) {
                        _headerIdToContentMapping.Add(e.HeaderId, e.Body);
                        if (e?.HeaderId == SelectedHeader?.HeaderId)
                        {
                            SetActiveBodyForHeader();
                        }
                    }
                });               
            }));
            _compositeDisposable.Add(emailFetchService.IsProcessingObservable
                .ObserveOn(_dispatcherSchedulerProvider.DispatcherScheduler).Subscribe(e => IsRunEnabled = !e));
        }

        private async Task ExecuteAsync()
        {
            HeadersCollection.Clear();
            _headerIdToContentMapping.Clear();
            SelectedHeader = null;
            _cts = new CancellationTokenSource();
            DisplayedBody = string.Empty;
            try
            {
                await Task.Run(async () => await _emailFetchService.FetchAllEmails(ServerSettings.Protocol, ServerSettings.TransportProtocol, ServerSettings.ServerName, ServerSettings.UserName, ServerSettings.Password, ServerSettings.PortNumber, _cts.Token), _cts.Token);
            }
            catch(Exception exception)
            {
                _cts.Cancel();
                DisplayedBody = exception.ToString();
            }
        }
        private void SetActiveBodyForHeader()
        {
            var id = _selectedHeader?.HeaderId;
            if (string.IsNullOrEmpty(id))
                return;
            DisplayedBody = string.Empty;
            if (_headerIdToContentMapping.ContainsKey(id))
            {
                DisplayedBody = _headerIdToContentMapping[id];
            }
            else RequestBodyForHeader();
        }
        private void RequestBodyForHeader()
        {
            if (string.IsNullOrEmpty(_selectedHeader?.HeaderId))
                return;
            _emailFetchService.RequestEmailByHeaderId(_selectedHeader.HeaderId);
        }
        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _compositeDisposable?.Dispose();
                    if (_cts?.IsCancellationRequested == false)
                    {
                        _cts?.Cancel();
                    }
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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailingLib;
using MailingLib.BodyDownloader;
using MailingLib.HeadersDownloader;
using MailingLib.Protocol;
using MailingLib.Models;
namespace Services
{
    public class EmailFetchService : IEmailFetchService
    {
        public IObservable<List<EmailBody>> EmailBodiesObservable => _emailBodiesSubject;
        public IObservable<List<EmailHeader>> EmailHeaderObservable => _emailHeadersSubject;
        public IObservable<bool> IsProcessingObservable => _isProcessingSubject;
        #region private members
        private const int MaxBodyProducers = 2;
        private const int MaxHeadersConsumers = 2;
        private const int DelayOnConsumers = 10;
        private const int PriorityQueueCapacity = 1;
        private BlockingCollection<string> _bodiesToProcess = new BlockingCollection<string>();
        private BlockingCollection<string> _priorityIdsToProcess = new BlockingCollection<string>(PriorityQueueCapacity);
        private BlockingCollection<string> _headersToProcess = new BlockingCollection<string>();
        private readonly BehaviorSubject<List<EmailBody>> _emailBodiesSubject = new BehaviorSubject<List<EmailBody>>(new List<EmailBody>());
        private readonly BehaviorSubject<List<EmailHeader>> _emailHeadersSubject = new BehaviorSubject<List<EmailHeader>>(new List<EmailHeader>());
        private readonly BehaviorSubject<bool> _isProcessingSubject = new BehaviorSubject<bool>(false);
        private ConcurrentDictionary<string, bool> _processedBodyIds = new ConcurrentDictionary<string, bool>();
        private readonly IEmailBodyDownloaderFactory _emailBodyDownloaderFactory;
        private readonly IEmailHeadersDownloaderFactory _emailHeadersDownloaderFactory;
        #endregion

        public EmailFetchService(IEmailBodyDownloaderFactory emailBodyDownloaderFactory, IEmailHeadersDownloaderFactory emailHeadersDownloaderFactory)
        {  
            _emailBodyDownloaderFactory = emailBodyDownloaderFactory;
            _emailHeadersDownloaderFactory = emailHeadersDownloaderFactory;
        }
        public void RequestEmailByHeaderId(string id)
        {
            _processedBodyIds.TryGetValue(id, out bool containsHeader);
            if (DataIsProcessing && containsHeader)
            {
                return;
            }
            _priorityIdsToProcess.TryTake(out string thrash);
            _priorityIdsToProcess.Add(id);
        }
        private bool DataIsProcessing => (!_bodiesToProcess.IsCompleted || !_headersToProcess.IsCompleted);
        private bool _isBusy = false;
        private static object _busyLock = new object();
        public async Task FetchAllEmails(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port, CancellationToken ct)
        {
            try
            {
                lock (_busyLock)
                {
                    if (_isBusy)
                    {
                        return;
                    }
                    _isBusy = true;
                    _isProcessingSubject.OnNext(_isBusy);
                }
                _processedBodyIds = new ConcurrentDictionary<string, bool>();
                _priorityIdsToProcess = new BlockingCollection<string>(PriorityQueueCapacity);
                using (var headersDownloader = _emailHeadersDownloaderFactory.CreateForProtocol(protocol))
                {
                    headersDownloader.Connect(protocol, transportProtocol, hostName, userName, password, port);
                    var ids = headersDownloader.FetchHeaderIds();
                    headersDownloader.Disconnect();

                    _bodiesToProcess = new BlockingCollection<string>(new ConcurrentQueue<string>(ids));
                    _headersToProcess = new BlockingCollection<string>(new ConcurrentQueue<string>(ids));
                    _headersToProcess.CompleteAdding();
                    _bodiesToProcess.CompleteAdding();
                }
                try
                {

                    var tasks = new List<Task>();

                    for (int i = 0; i < MaxHeadersConsumers; i++)
                    {
                        tasks.Add(Task.Factory.StartNew(() => StartConsumingHeaders(protocol, transportProtocol, hostName, userName, password, port, ct), ct, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                    }
                    for (int i = 0; i < MaxBodyProducers; i++)
                    {
                        tasks.Add(Task.Factory.StartNew(() => StartConsuming(protocol, transportProtocol, hostName, userName, password, port, ct), ct, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                    }
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _bodiesToProcess = new BlockingCollection<string>();
                    _headersToProcess = new BlockingCollection<string>();
                    _processedBodyIds = new ConcurrentDictionary<string, bool>();
                    _priorityIdsToProcess = new BlockingCollection<string>(PriorityQueueCapacity);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                lock (_busyLock)
                {
                    _isBusy = false;
                    _isProcessingSubject.OnNext(_isBusy);
                }
            }
        }

        private async Task StartConsumingHeaders(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port, CancellationToken ct)
        {
            using (var headerDownloader = _emailHeadersDownloaderFactory.CreateForProtocol(protocol))
            {
                string result = null;
                headerDownloader.Connect(protocol, transportProtocol, hostName, userName, password, port);
                while (!ct.IsCancellationRequested && !_headersToProcess.IsCompleted)
                {
                    _headersToProcess.TryTake(out result);
                    if (result == null)
                    {
                        await Task.Delay(DelayOnConsumers, ct);
                        continue;
                    }
                    var downloadedHeaders = headerDownloader.GetHeaders(new List<string> { result });
                    _emailHeadersSubject.OnNext(downloadedHeaders);
                    await Task.Delay(DelayOnConsumers, ct);
                }               
                headerDownloader.Disconnect();                
            }
        }

        private async Task StartConsuming(EmailProtocol protocol, TransportProtocol transportProtocol, string hostName, string userName, string password, int port, CancellationToken ct)
        {
            using (var bodyDownloader = _emailBodyDownloaderFactory.CreateForProtocol(protocol))

            {
                string result = null;
                bodyDownloader.Connect(protocol, transportProtocol, hostName, userName, password, port);
                while (!ct.IsCancellationRequested && !_bodiesToProcess.IsCompleted)
                {
                    
                    if (!_priorityIdsToProcess.TryTake(out result)) { _bodiesToProcess.TryTake(out result); }
                    if (result == null)
                    {
                        await Task.Delay(DelayOnConsumers, ct);
                        continue;
                    }
                    var valueAlreadyProcessed = false;
                    _processedBodyIds.TryGetValue(result, out valueAlreadyProcessed);
                    if (!valueAlreadyProcessed)
                    {
                        _processedBodyIds.TryAdd(result, true);
                        var bodyResult = bodyDownloader.GetBody(result);
                        _emailBodiesSubject.OnNext(new List<EmailBody> { bodyResult });                        
                    }
                    result = null;
                  
                }
                bodyDownloader.Disconnect();
            }
        }

    }
}

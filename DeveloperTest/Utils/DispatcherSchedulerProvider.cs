using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperTest.Utils
{
    public class DispatcherSchedulerProvider : IDispatcherSchedulerProvider
    {
        public IScheduler DispatcherScheduler => System.Reactive.Concurrency.DispatcherScheduler.Current;
    }
}

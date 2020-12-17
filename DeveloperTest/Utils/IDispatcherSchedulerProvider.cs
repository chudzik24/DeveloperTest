using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
namespace DeveloperTest.Utils
{
    public interface IDispatcherSchedulerProvider 
    {
        IScheduler DispatcherScheduler { get; }
    }
}

/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DeveloperTest"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using Services;
using MailingLib.BodyDownloader;
using MailingLib.HeadersDownloader;
using MailingLib.Protocol;
using GalaSoft.MvvmLight.Views;
using DeveloperTest.Utils;

namespace DeveloperTest.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IEmailFetchService, EmailFetchService>();
            SimpleIoc.Default.Register<IEmailBodyDownloaderFactory, EmailBodyDownloaderFactory>();
            SimpleIoc.Default.Register<IEmailHeadersDownloaderFactory, EmailHeadersDownloaderFactory>();
            SimpleIoc.Default.Register<IProtocolCommunicationStrategyFactory, ProtocolCommunicationStrategyFactory>();
            SimpleIoc.Default.Register<IDispatcherSchedulerProvider, DispatcherSchedulerProvider>();

            
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
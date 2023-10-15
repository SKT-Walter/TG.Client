using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TG.Client.Handler;
using TG.Client.Utils;

namespace TG
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            UserHandler.Instance.PublishMsg("application exit, code:{0}" + e.ApplicationExitCode);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            UserHandler.Instance.PublishMsg("TaskScheduler_UnobservedTaskException" + e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                UserHandler.Instance.PublishMsg("CurrentDomain_UnhandledException, will exit" + (Exception)e.ExceptionObject);
            }
            else
            {
                UserHandler.Instance.PublishMsg("CurrentDomain_UnhandledException," + e.ExceptionObject);
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                UserHandler.Instance.PublishMsg("App_DispatcherUnhandledException" + e.Exception);
            }
            catch (Exception ex)
            {
                UserHandler.Instance.PublishMsg("App_DispatcherUnhandledException"+ ex);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;


            //BinLogHelper.DisableBinLog();
        }
    }
}

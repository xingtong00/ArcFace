using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace Tong.ArcFaceSample
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private readonly ILog _logError = LogManager.GetLogger("logerror");
        private readonly ILog _logInfo = LogManager.GetLogger("loginfo");

        /// <summary>
        /// 应用程序启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Current.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                _logError.Error(e.Exception.Message, e.Exception);
            }
            catch (Exception ex)
            {
                _logError.Error(ex.Message, ex);
                MessageBox.Show("应用程序发生不可恢复的异常，将要退出！");
            }
        }

        /// <summary>
        /// UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                _logError.Error(e.Exception.Message, e.Exception);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                _logError.Error(ex.Message, ex);
                MessageBox.Show("应用程序发生不可恢复的异常，将要退出！");
            }
        }

        /// <summary>
        /// 非UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    _logError.Error(exception.Message, exception);
                }
            }
            catch (Exception ex)
            {
                _logError.Error(ex.Message, ex);
                MessageBox.Show("应用程序发生不可恢复的异常，将要退出！");
            }
        }
    }
}

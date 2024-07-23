using System;
using System.Windows;

namespace Internetdownloadmanager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
               
                string url = e.Args[0];
                string scheduledTimeString = e.Args[1];

                DateTime scheduledTime;
                if (!DateTime.TryParse(scheduledTimeString, out scheduledTime))
                {
                    MessageBox.Show("Invalid scheduled time format. Please enter a valid scheduled time in format 'yyyy-MM-dd HH:mm:ss'");
                    Shutdown();
                    return;
                }

                if (DateTime.Now > scheduledTime)
                {
                    MessageBox.Show("Scheduled time should be in the future");
                    Shutdown();
                    return;
                }

                var timer = new System.Timers.Timer();
                timer.Interval = (scheduledTime - DateTime.Now).TotalMilliseconds;
                timer.Elapsed += (timerSender, timerArgs) =>
                {
                    timer.Stop();

                   
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var mainWindow = new Window5(url, scheduledTime);
                        mainWindow.Show();
                    });
                };
                timer.Start();
            }
            else
            {
              
                var mainWindow = new Window5();
                mainWindow.Show();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Internetdownloadmanager
{
    public partial class Window5 : Window
    {
        private readonly WebClient webClient = new WebClient();
        private DownloadManager downloadManager;
        private DateTime scheduledTime;
        private string url;
        public Window5()
        {
            InitializeComponent();
            InitializeWebClient();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            downloadManager = new DownloadManager();
        }
        public Window5(string url, DateTime scheduledTime) : this()
        {
            this.url = url;
            this.scheduledTime = scheduledTime;
        }

        private void InitializeWebClient()
        {
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string url = txtUrl.Text;
            string scheduledTimeString = txtScheduledTime.Text;

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please enter a URL");
                return;
            }
            DateTime? selectedDate = dpDate.SelectedDate;
            if (selectedDate.HasValue)
            {
                scheduledTime = selectedDate.Value.Add(TimeSpan.Parse(scheduledTimeString));
            }
            if (DateTime.Now > scheduledTime)
            {
                MessageBox.Show("Scheduled time should be in the future");
                return;
            }

            if (string.IsNullOrEmpty(GlobalVariables.SavePath))
            {
                MessageBox.Show("Please set the download path in the settings before scheduling the download.");
                return;
            }
            //MessageBox.Show($"Scheduled Date and Time: {scheduledTime}");

            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                try
                {
                   
                    string fileName = System.IO.Path.GetFileName(uri.LocalPath);
                    string savePath = System.IO.Path.Combine(GlobalVariables.SavePath, fileName);

                    var timer = new System.Timers.Timer();
                    timer.Interval = (scheduledTime - DateTime.Now).TotalMilliseconds;
                    timer.Elapsed += (timerSender, timerArgs) =>
                    {
                        timer.Stop();
                        try
                        {
                            webClient.DownloadFileAsync(uri, savePath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error downloading file: {ex.Message}");
                        }

                        webClient.DownloadFileCompleted += (downloadSender, downloadArgs) =>
                        {
                            if (downloadArgs.Error != null)
                            {
                                MessageBox.Show($"Error downloading file: {downloadArgs.Error.Message}");
                            }
                            else
                            {
                                MessageBox.Show($"File downloaded successfully. Save path: {savePath}");
                            }
                        };
                    };
                    timer.Start();

                    MessageBox.Show("Download scheduled");

                    var countdownTimer = new System.Windows.Threading.DispatcherTimer();
                    countdownTimer.Interval = TimeSpan.FromSeconds(1);
                    countdownTimer.Tick += (countdownTimerSender, countdownTimerArgs) =>
                    {
                        TimeSpan timeLeft = scheduledTime - DateTime.Now;
                        if (timeLeft.TotalSeconds <= 0)
                        {
                            lblTimer.Content = "Download starting now...";
                            countdownTimer.Stop();
                            var invisibleTimer = new System.Windows.Threading.DispatcherTimer();
                            invisibleTimer.Interval = TimeSpan.FromSeconds(2);
                            invisibleTimer.Tick += (invisibleTimerSender, invisibleTimerArgs) =>
                            {
                                lblTimer.Visibility = Visibility.Hidden;
                                invisibleTimer.Stop();
                            };
                            invisibleTimer.Start();
                        }
                        else
                        {
                            lblTimer.Content = $"Time left: {timeLeft.ToString(@"hh\:mm\:ss")}";
                        }
                    };
                    countdownTimer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Invalid URL");
            }
        }



        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            lblProgress.Dispatcher.Invoke(() =>
            {
                lblProgress.Text = $"Downloaded {e.BytesReceived} of {e.TotalBytesToReceive} bytes. {e.ProgressPercentage}% complete. {Environment.NewLine} Save Path: {GlobalVariables.SavePath}";
            });
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show($"Error: {e.Error.Message}");
            }
            else
            {
                MessageBox.Show("Download completed successfully.");
            }
        }





    }
}

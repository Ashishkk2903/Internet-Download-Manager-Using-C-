using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;

namespace Internetdownloadmanager
{
    public partial class Window2 : Window
    {
        private readonly Queue<string> downloadQueue = new Queue<string>();
        private readonly Window1 window1 = new Window1();
        private readonly MainWindow window = new MainWindow();
        private CancellationTokenSource cancellationTokenSource;

        public Window2()
        {
            InitializeComponent();
        }

        private void AddToQueue_Click(object sender, RoutedEventArgs e)
        {
            string url = urlTextBox.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                ShowMessage("Please enter a valid URL.", TimeSpan.FromSeconds(3), Brushes.Red);
                return;
            }
            downloadQueue.Enqueue(url);
            downloadQueueListView.Items.Add(url);
            urlTextBox.Text = "";
        }


        private void ShowMessage(string message, TimeSpan duration, SolidColorBrush colorBrush = null)
        {
            messageTextBlock.Text = message;
            if (colorBrush != null)
                messageTextBlock.Foreground = colorBrush;

            messageTextBlock.Visibility = Visibility.Visible;

            //timer to hide the message after the specified duration
            var timer = new DispatcherTimer { Interval = duration };
            timer.Tick += (sender, args) =>
            {
                messageTextBlock.Visibility = Visibility.Hidden;
                timer.Stop();
            };
            timer.Start();
        }

        private async void StartDownloadQueue_Click(object sender, RoutedEventArgs e)
        {
            if (downloadQueue.Count == 0)
            {
                ShowMessage("The download queue is empty.", TimeSpan.FromSeconds(3));
                return;
            }


            ShowMessage("Download queue has started.", TimeSpan.FromSeconds(3), Brushes.Red);

            cancellationTokenSource = new CancellationTokenSource();

            while (downloadQueue.Count > 0)
            {
                string url = downloadQueue.Dequeue();
                string fileName = Path.GetFileName(url);
                string savePath =  GlobalVariables.SavePath + fileName;
                await DownloadFileAsync(url, savePath, cancellationTokenSource.Token);

                
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    ShowMessage("Download queue stopped.", TimeSpan.FromSeconds(3));
                    break;
                }
            }
        }
        private void StopQueue_Click(object sender, RoutedEventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                downloadQueue.Clear(); 
            }
            else
            {
                ShowMessage("No active download queue to stop.", TimeSpan.FromSeconds(3), Brushes.Blue);
            }
        }


        private async Task DownloadFileAsync(string url, string savePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(GlobalVariables.SavePath))
            {
                ShowMessage("Global save path is not defined. Please set the default path from settings.", TimeSpan.FromSeconds(3), Brushes.Red);
                return;
            }
            Directory.CreateDirectory(GlobalVariables.SavePath);
            using (var client = new WebClient())
            {
                try
                {
                    string fileName = Path.GetFileName(url);
                    string filePath = Path.Combine(GlobalVariables.SavePath, fileName);

                    await client.DownloadFileTaskAsync(new Uri(url), filePath);
                    ShowMessage($"Downloaded successfully: {url}", TimeSpan.FromSeconds(3), Brushes.Green);
                    SaveDownloadInfo(Path.GetFileName(url), savePath, "Downloaded", new FileInfo(savePath).Length, savePath);
                    window1.UpdateDownloadStatus(Path.GetFileName(url), "Downloaded");
                }
                catch (WebException ex)
                {
                    ShowMessage($"Error downloading file: {ex.Message}", TimeSpan.FromSeconds(3), Brushes.Red);
                }
            }
            window.downloadDataGrid.ItemsSource = window.LoadDownloadInfo();
            window.downloadDataGrid.Items.Refresh();

        }

        public void SaveDownloadInfo(string fileName, string savePath, string status, long fileSize, string fileIcon)
        {
            string downloadInfoFile = "downloadInfo.txt";
            using (StreamWriter writer = File.AppendText(downloadInfoFile))
            {
                writer.WriteLine($"{fileName},{savePath},{status},{fileSize},{fileIcon}");
               
            }
            
        }

       

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Internetdownloadmanager
{
    public partial class Window4 : Window
    {
        private List<CustomDownloadInfo> downloadList = new List<CustomDownloadInfo>();
        private DateTime startTime;

        public Window4()
        {
            InitializeComponent();
            startTime = DateTime.Now;
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string[] urls = urlTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string url in urls)
            {
                await DownloadFileAsync(url);
            }
        }



        private async Task DownloadFileAsync(string url)
        {
            if (string.IsNullOrEmpty(GlobalVariables.SavePath) || !Directory.Exists(GlobalVariables.SavePath))
            {
                MessageBox.Show("Global save path is not defined or is invalid. Please set the default path from settings.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var client = new WebClient())
            {
                string fileName = Path.GetFileName(url);
                string savePath = Path.Combine(GlobalVariables.SavePath, fileName);


                Window1 a = new Window1();
                long fileSize = await a.GetFileSizeAsync(url);
                int initialProgressPercentage = 0; 
                long bytesReceived = 0; 
                double bytesPerSecond = 0;
                double transferRateMBps = 0; 

                
                CustomDownloadInfo info = new CustomDownloadInfo
                {
                    FileName = fileName,
                    SavePath = savePath,
                    Status = "Downloading...",
                    FileSize = fileSize
                };
                downloadList.Add(info);

                
                AddDownloadProgressUI(info);
                UpdateUI(fileName, initialProgressPercentage, bytesReceived, fileSize, transferRateMBps, TimeSpan.Zero);

                
                client.DownloadProgressChanged += (sender, e) =>
                {
                    
                    int progressPercentage = e.ProgressPercentage;
                    bytesReceived = e.BytesReceived;
                    bytesPerSecond = bytesReceived / (DateTime.Now - startTime).TotalSeconds;
                    transferRateMBps = bytesPerSecond / (1024 * 1024);

                   
                    long bytesRemaining = fileSize - bytesReceived;
                    double secondsRemaining = bytesRemaining / bytesPerSecond;
                    TimeSpan timeLeft = TimeSpan.FromSeconds(secondsRemaining);

                   
                    UpdateUI(fileName, progressPercentage, bytesReceived, fileSize, transferRateMBps, timeLeft);
                };

                try
                {
                    await client.DownloadFileTaskAsync(new Uri(url), savePath);

                    
                    info.Status = "Downloaded";
                    UpdateUI(fileName, 100, fileSize, fileSize, transferRateMBps, TimeSpan.Zero);

                    
                    SaveDownloadInfo(fileName, savePath, info.Status, fileSize, ""); 
                }
                catch (WebException ex)
                {
                   
                    MessageBox.Show($"Error downloading file {fileName}: {ex.Message}", "WebException", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveDownloadInfo(string fileName, string savePath, string status, long fileSize, string fileIcon)
        {
            string downloadInfoFile = "downloadInfo.txt";
            using (StreamWriter writer = File.AppendText(downloadInfoFile))
            {
                writer.WriteLine($"{fileName},{savePath},{status},{fileSize},{fileIcon}");
               
            }

        }



        private void AddDownloadProgressUI(CustomDownloadInfo info)
        {
           
            StackPanel downloadStackPanel = new StackPanel();
            downloadStackPanel.Margin = new Thickness(0, 10, 0, 0);

            
            TextBlock urlTextBlock = new TextBlock();
            urlTextBlock.Text = $"URL: {info.FileName}";
            downloadStackPanel.Children.Add(urlTextBlock);

            TextBlock statusTextBlock = new TextBlock();
            statusTextBlock.Text = $"Status: {info.Status}";
            downloadStackPanel.Children.Add(statusTextBlock);

            TextBlock fileSizeTextBlock = new TextBlock();
            fileSizeTextBlock.Text = $"File Size: {info.FileSize} bytes";
            downloadStackPanel.Children.Add(fileSizeTextBlock);

            TextBlock percentageTextBlock = new TextBlock();
            percentageTextBlock.Text = "Percentage: ";
            downloadStackPanel.Children.Add(percentageTextBlock);

            TextBlock transferRateTextBlock = new TextBlock();
            transferRateTextBlock.Text = "Transfer Rate: ";
            downloadStackPanel.Children.Add(transferRateTextBlock);

            TextBlock timeLeftTextBlock = new TextBlock();
            timeLeftTextBlock.Text = "Time Left: ";
            downloadStackPanel.Children.Add(timeLeftTextBlock);

           
            downloadsStackPanel.Children.Add(downloadStackPanel);
        }

        private void UpdateUI(string fileName, int progressPercentage, long bytesReceived, long totalBytes, double transferRateMBps, TimeSpan timeLeft)
        {
            
            foreach (var child in downloadsStackPanel.Children)
            {
                if (child is StackPanel downloadStackPanel)
                {
                    TextBlock urlTextBlock = (TextBlock)downloadStackPanel.Children[0];
                    if (urlTextBlock.Text.Contains(fileName))
                    {
                        
                        TextBlock statusTextBlock = (TextBlock)downloadStackPanel.Children[1];
                        statusTextBlock.Text = $"Status: Downloading... {progressPercentage}%";

                        TextBlock percentageTextBlock = (TextBlock)downloadStackPanel.Children[3];
                        percentageTextBlock.Text = $"Percentage: {progressPercentage}%";

                       
                        TextBlock transferRateTextBlock = (TextBlock)downloadStackPanel.Children[4];
                        transferRateTextBlock.Text = $"Transfer Rate: {transferRateMBps.ToString("0.00")} MB/s";

                        TextBlock timeLeftTextBlock = (TextBlock)downloadStackPanel.Children[5];
                        timeLeftTextBlock.Text = $"Time Left: {timeLeft.ToString(@"hh\:mm\:ss")}";

                        break;
                    }
                }
            }
        }





        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class CustomDownloadInfo
    {
        public string FileName { get; set; }
        public string SavePath { get; set; }
        public string Status { get; set; }
        public long FileSize { get; set; }
       
    }
}

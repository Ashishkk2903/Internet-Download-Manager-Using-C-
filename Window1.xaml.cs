using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using static Internetdownloadmanager.MainWindow;
using System.Security.Policy;

namespace Internetdownloadmanager
{
    public partial class Window1 : Window
    {
  
        private WebClient client;
        private readonly DateTime startTime = DateTime.Now;
        private readonly string downloadInfoFile = "downloadInfo.txt";
        private bool isPaused = false;
        private bool isDownloading = false;
        private string savePath;
        private readonly DateTime scheduledTime;
        private readonly string url;

        public Window1(DateTime scheduledTime, string url, string savePath)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.scheduledTime = scheduledTime;
            this.url = url;
            this.savePath = savePath;
            StartDownloadAtScheduledTime();
        }
        public Window1()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public void StartDownloadAtScheduledTime()
        {
          
            TimeSpan delay = scheduledTime - DateTime.Now;
            if (delay.TotalMilliseconds > 0)
            {
               
                Task.Delay(delay).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        StartDownload();
                    });
                });
            }
            else
            {
                MessageBox.Show("Scheduled time has already passed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private async void StartDownload()
        {
            if (string.IsNullOrEmpty(GlobalVariables.SavePath))
            {
                MessageBox.Show("Default save path is not defined. Please set the default path from settings.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                return;
            }
            client = new WebClient();
            client.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            client.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            try
            {
                await client.DownloadFileTaskAsync(new Uri(url), Path.Combine(GlobalVariables.SavePath, Path.GetFileName(url)));
            }
            catch (WebException ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "WebException", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                client.Dispose();
                this.Close();
            }
        }
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string url = urlTextBox.Text;
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please provide a valid URL.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(GlobalVariables.SavePath))
            {
                MessageBox.Show("Default save path is not defined. Please set the default path from settings.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                return;
            }


            string fileName = Path.GetFileName(url);
            savePath = GlobalVariables.SavePath + fileName;

            if (!isDownloading)
            {
                client = new WebClient();
                client.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                client.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                try
                {
                    isDownloading = true;
                    await client.DownloadFileTaskAsync(new Uri(url), Path.Combine(GlobalVariables.SavePath, Path.GetFileName(url)));
                }
                catch (WebException ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "WebException", MessageBoxButton.OK, MessageBoxImage.Error);
                    isDownloading = false;
                }
            }
            else
            {
                MessageBox.Show("A download is already in progress.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //long fileSize = await GetFileSizeAsync(url);
            //string fileIcon = savePath;
            //SaveDownloadInfo(fileName, savePath, "Downloaded", fileSize, fileIcon);
            //UpdateDownloadStatus(fileName, "Downloaded");
        }

        private void SaveDownloadInfo(string fileName, string savePath, string status, long fileSize, string fileIcon)
        {
            string category = GetCategoryFromFileName(fileName);
            using (StreamWriter writer = File.AppendText(downloadInfoFile))
            {
                writer.WriteLine($"{fileName},{savePath},{status},{fileSize},{fileIcon},{category}");
            }

            
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var downloadInfo = new DownloadInfo
                {
                    FileName = fileName,
                    SavePath = savePath,
                    Status = status,
                    FileSize = fileSize/1024,
                    Category = category

                };
                mainWindow.AddDownload(downloadInfo);
            }
            mainWindow.LoadDownloads();
        }


        private string GetCategoryFromFileName(string fileName)
        {
           
          
            string extension = System.IO.Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".tif":
                case ".tiff":
                    return "Images";
                case ".mp4":
                case ".avi":
                case ".mkv":
                case ".mov":
                case ".wmv":
                case ".flv":
                    return "Videos";
                case ".mp3":
                case ".wav":
                case ".ogg":
                case ".wma":
                case ".aac":
                case ".flac":
                    return "Music";
                case ".doc":
                case ".docx":
                case ".txt":
                case ".pdf":
                case ".xls":
                case ".xlsx":
                case ".ppt":
                case ".pptx":
                    return "Documents";
                case ".zip":
                case ".rar":
                case ".7z":
                    return "Compressed";
                case ".exe":
                case ".dll":
                case ".bat":
                case ".sh":
                case ".jar":
                    return "Programs";
                default:
                    return "Other"; // Default category for unknown file 
            }
        }

        public void UpdateDownloadStatus(string fileName, string status)
        {
            string tempFile = Path.GetTempFileName();
            using (var reader = new StreamReader(downloadInfoFile))
            using (var writer = new StreamWriter(tempFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 3 && parts[0] == fileName)
                    {
                        parts[2] = status;
                        line = string.Join(",", parts);
                    }
                    writer.WriteLine(line);
                }
            }
            File.Delete(downloadInfoFile);
            File.Move(tempFile, downloadInfoFile);
        }




        public async Task DownloadFileAsync(string url, string savePath)
        {
            using (var client = new WebClient())
            {
                long fileSize = await GetFileSizeAsync(url);
                const int MaxConnections = 8;
                long partSize = fileSize / MaxConnections;
                long fromByte = 0;
                var tasks = new Task[MaxConnections];

                for (int i = 0; i < MaxConnections; i++)
                {
                    tasks[i] = DownloadPartAsync(client, url, savePath, fromByte, fromByte + partSize - 1);
                    fromByte += partSize;
                    if (i == MaxConnections - 2)
                    {
                        fromByte = fileSize;
                    }
                }

                await Task.WhenAll(tasks);
                CombineParts(savePath);
            }
        }

        public async Task<long> GetFileSizeAsync(string url)
        {
            using (var client = new WebClient())
            {
                var stream = await client.OpenReadTaskAsync(url);
                long fileSizeBytes = Convert.ToInt64(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
                long fileSizeKB = fileSizeBytes / 1024;
                return fileSizeKB;
            }
        }

        private async Task DownloadPartAsync(WebClient client, string url, string savePath, long fromByte, long toByte)
        {
            using (var stream = new FileStream(savePath + ".part", FileMode.Append))
            {
                client.Headers["Range"] = $"bytes={fromByte}-{toByte}";
                using (var downloadStream = await client.OpenReadTaskAsync(new Uri(url)))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await stream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
        }



        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            isDownloading = false;
            if (e.Error != null)
            {
                MessageBox.Show($"Error downloading file: {e.Error.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Download cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("Download completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Get the file name 
                string fileName = Path.GetFileName(urlTextBox.Text);

                // Update download status
                UpdateDownloadStatus(fileName, "Downloaded");

                // Get file size
                long fileSize = new FileInfo(savePath).Length;

                // Add download info
                string fileIcon = savePath;
                SaveDownloadInfo(fileName, savePath, "Downloaded", fileSize, fileIcon);
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            urlTextBlock.Text = "URL: " + urlTextBox.Text;
            statusTextBlock.Text = "Status: Downloading...";
            fileSizeTextBlock.Text = "File Size: " + e.TotalBytesToReceive + " bytes";
            percentageTextBlock.Text = "Percentage: " + e.ProgressPercentage + "%";

            double bytesPerSecond = e.BytesReceived / (DateTime.Now - startTime).TotalSeconds;
            double transferRateMBps = bytesPerSecond / (1024 * 1024);
            transferRateTextBlock.Text = "Transfer Rate: " + transferRateMBps.ToString("0.00") + " MB/s";

            double secondsRemaining = (e.TotalBytesToReceive - e.BytesReceived) / bytesPerSecond;
            TimeSpan timeLeft = TimeSpan.FromSeconds(secondsRemaining);
            timeLeftTextBlock.Text = "Time Left: " + timeLeft.ToString(@"hh\:mm\:ss");
        }

        private void CombineParts(string savePath)
        {
            using (var combinedStream = new FileStream(savePath, FileMode.Create))
            {
                const int MaxConnections = 8;
                for (int i = 0; i < MaxConnections; i++)
                {
                    string partPath = savePath + $".part{i}";
                    using (var partStream = new FileStream(partPath, FileMode.Open))
                    {
                        partStream.CopyTo(combinedStream);
                    }
                    File.Delete(partPath);
                }
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDownloading && !isPaused)
            {
                client.CancelAsync();
                isPaused = true;
                MessageBox.Show("Download paused.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (!isDownloading)
            {
                MessageBox.Show("No download in progress to pause.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (isPaused)
            {
                MessageBox.Show("Download is already paused.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isDownloading)
            {
                isPaused = false;
                if (client != null && !string.IsNullOrEmpty(savePath))
                {
                    try
                    {
                        isDownloading = true;
                        await client.DownloadFileTaskAsync(new Uri(urlTextBox.Text), savePath);
                    }
                    catch (WebException ex)
                    {
                        MessageBox.Show($"Error resuming download: {ex.Message}", "WebException", MessageBoxButton.OK, MessageBoxImage.Error);
                        isDownloading = false;
                    }
                }
                else
                {
                    MessageBox.Show("No paused download to resume.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("A download is already in progress.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDownloading)
            {
                
                client.CancelAsync();
                isDownloading = false;
               
            }
            this.Close();
        }

    }
}

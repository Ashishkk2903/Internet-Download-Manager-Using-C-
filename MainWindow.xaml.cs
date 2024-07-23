using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.ComponentModel;
using System.Windows.Media;
using System.Diagnostics;
using Internetdownloadmanager;
using System.Windows.Threading;


namespace Internetdownloadmanager
{
    public partial class MainWindow : Window
    {

        private DispatcherTimer fileCheckTimer;
        private List<DownloadInfo> allDownloads;
        private Timer syncTimer;
        private string folderPath;
        private Window2 window2Instance;
        private string downloadInfoFile = "downloadInfo.txt";
        private bool categoriesVisible = true;


        public MainWindow()
        {
            InitializeComponent();

            syncTimer = new System.Timers.Timer(5000);
            syncTimer.Elapsed += (sender, e) => { Dispatcher.Invoke(UpdateDownloads); };
            syncTimer.Start();
            downloadDataGrid.ItemsSource = LoadDownloadInfo();
            categoryTreeView.SelectedItemChanged += Category_Selected;
            LoadAllDownloads();
            LoadDownloads();
            LoadCategories();

            fileCheckTimer = new DispatcherTimer();
            fileCheckTimer.Interval = TimeSpan.FromSeconds(5);
            fileCheckTimer.Tick += FileCheckTimer_Tick;
            fileCheckTimer.Start();

        }
        private void FileCheckTimer_Tick(object sender, EventArgs e)
        {
            
            UpdateDownloadsIfChanged();
        }

        private void UpdateDownloadsIfChanged()
        {
           

            downloadDataGrid.ItemsSource = LoadDownloadInfo();
            downloadDataGrid.Items.Refresh();
        }

        private void SchedularMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window5 window5 = new Window5(); 
            window5.Show();
        }
        private void LoadAllDownloads()
        {
            allDownloads = LoadDownloadInfo();
            downloadDataGrid.ItemsSource = allDownloads;
            LoadCategories(); 
        }

        private void RefreshMenuItem_Click(object sender, RoutedEventArgs e)
        {
          
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void LoadDownloads()
        {
            allDownloads = LoadDownloadInfo();
            downloadDataGrid.ItemsSource = allDownloads;
        }

       
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
           
            Close();
        }

        private void ExportDownloadInfoFile(object sender, RoutedEventArgs e)
        {
            try
            {

                var saveFileDialog = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = saveFileDialog.ShowDialog();

               
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string destinationFolder = saveFileDialog.SelectedPath;

                   
                    string sourceFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, downloadInfoFile);
                    string destinationFile = System.IO.Path.Combine(destinationFolder, downloadInfoFile);

                    System.IO.File.Copy(sourceFile, destinationFile, true);

                    MessageBox.Show("DownloadInfo file exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting the DownloadInfo file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void ToggleCategoryVisibilityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                if (menuItem.Header.ToString() == "Hide Categories")
                {
                    ToggleCategoryVisibility();
                }
                else if (menuItem.Header.ToString() == "Show Categories")
                {
                    ToggleCategoryVisibility();
                }
                
            }
        }

        private void ToggleCategoryVisibility()
        {
            if (categoriesVisible)
            {
                
                categoriesGrid.Visibility = Visibility.Collapsed;
                hideShowCategoriesMenuItem.Header = "Show Categories";
                categoriesVisible = false;
            }
            else
            {
                
                categoriesGrid.Visibility = Visibility.Visible;
                hideShowCategoriesMenuItem.Header = "Hide Categories";
                categoriesVisible = true;
            }
        }
        public void OpenMultipleDownloadWindow()
        {
           
            Window4 window4 = new Window4();

           
            window4.Show();
        }
        private void OpenMultipleDownload_Click(object sender, RoutedEventArgs e)
        {
            OpenMultipleDownloadWindow();
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Window3 window3 = new Window3();
            window3.ShowDialog();
        }

        public void AddDownload(DownloadInfo downloadInfo)
        {
            var downloadList = downloadDataGrid.ItemsSource as List<DownloadInfo>;
            downloadList.Add(downloadInfo);
            downloadDataGrid.Items.Refresh();
        }


        private void DownloadDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (downloadDataGrid.SelectedItem != null)
            {
                DownloadInfo selectedDownload = (DownloadInfo)downloadDataGrid.SelectedItem;
                string filePath = selectedDownload.SavePath;
                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }
                else
                {
                    MessageBox.Show("File not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        public class DownloadInfo
        {
            public string FileName { get; set; }
            public string SavePath { get; set; }
            public string Status { get; set; }
            public long FileSize { get; set; }
            public string Category { get; set; }
            public ImageSource FileIcon { get; set; }
        }


        public List<DownloadInfo> LoadDownloadInfo()
        {
            List<DownloadInfo> downloadInfos = new List<DownloadInfo>();
            if (File.Exists(downloadInfoFile))
            {
                string[] lines = File.ReadAllLines(downloadInfoFile);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 4)
                    {
                        string fileName = parts[0];
                        string savePath = parts[1];
                        string status = parts[2];
                        long fileSize = long.Parse(parts[3]);
                        string category = GetCategoryFromFileName(fileName); // Extract category from file name

                        DownloadInfo downloadInfo = new DownloadInfo
                        {
                            FileName = fileName,
                            SavePath = savePath,
                            Status = status,
                            FileSize = fileSize,
                            Category = category
                        };
                        downloadInfos.Add(downloadInfo);
                        downloadDataGrid.Items.Refresh();
                    }
                    else
                    {
                        Console.WriteLine($"Invalid line: {line}");
                    }
                }
            }
            return downloadInfos;
        }

        private void LoadCategories()
        {
            
            categoryTreeView.Items.Clear();

            
            var categories = allDownloads.Select(download => download.Category).Distinct().ToList();

            foreach (var category in categories)
            {
                
                var item = new TreeViewItem()
                {
                    Header = category,
                    Tag = category 
                };
                item.Selected += Category_Selected;
                categoryTreeView.Items.Add(item);
            }

            
            var allCategoriesItem = new TreeViewItem()
            {
                Header = "All downloads",
                Tag = "All" 
            };
            allCategoriesItem.Selected += AllDownloads_Selected; 
            categoryTreeView.Items.Insert(0, allCategoriesItem); 
        }

        private void AllDownloads_Selected(object sender, RoutedEventArgs e)
        {
            
            downloadDataGrid.ItemsSource = allDownloads;
            downloadDataGrid.Items.Refresh();
        }


        private void Category_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = e.OriginalSource as TreeViewItem;

            if (selectedItem != null && selectedItem.Tag != null)
            {
                string selectedCategory = selectedItem.Tag.ToString();
                FilterDownloads(selectedCategory);
            }
            else if (selectedItem != null && selectedItem.Header.ToString() == "All")
            {
                downloadDataGrid.ItemsSource = LoadDownloadInfo();
                downloadDataGrid.Items.Refresh();
            }
        }



        private void FilterDownloads(string category)
        {
            var filteredDownloads = allDownloads.Where(download => download.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            downloadDataGrid.ItemsSource = filteredDownloads;
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
                    return "Other"; 
            }
        }




        private void StartQueueButton_Click(object sender, RoutedEventArgs e)
        {
            window2Instance = new Window2(); 
            window2Instance.Show();
        }

        private void StopQueue_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void UpdateDownloads()
        {
            DataContext = GetDownloadsFromFolder(folderPath);

        }

        public class FileViewModel : INotifyPropertyChanged
        {
            private bool isSelected;

            public string FileName { get; set; }
            public string FileSize { get; set; }
            public BitmapSource FileIcon { get; set; }

            public bool IsSelected
            {
                get { return isSelected; }
                set
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private List<FileViewModel> GetDownloadsFromFolder(string folderPath)
        {
            List<FileViewModel> downloads = new List<FileViewModel>();

            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(file);
                    BitmapSource iconSource = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    downloads.Add(new FileViewModel
                    {
                        FileName = fileInfo.Name,
                        FileSize = fileInfo.Length.ToString(),
                        FileIcon = iconSource,
                    });
                    icon.Dispose();
                }
            }

            return downloads;
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                ContextMenu contextMenu = button.ContextMenu;

                if (contextMenu != null)
                {
                    contextMenu.PlacementTarget = button;
                    contextMenu.IsOpen = true;
                }
                else
                {
                    MessageBox.Show("ContextMenu is not set for this button.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            MessageBox.Show($"You clicked: {menuItem.Header}");
        }

        private void DownloadB_Click(object sender, RoutedEventArgs e)
        {
            Window1 newWindow = new Window1();
            newWindow.Show();
        }

        private void LaunchWindow1Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        private void DeleteButtonall_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete all files?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string[] lines = File.ReadAllLines(downloadInfoFile);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 2)
                        {
                            string filePath = parts[1]; 
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                    }
                    File.WriteAllText(downloadInfoFile, "");
                    downloadDataGrid.ItemsSource = LoadDownloadInfo();

                    MessageBox.Show("All files have been deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var downloadInfo = (DownloadInfo)button.DataContext;
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{downloadInfo.FileName}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var downloadList = downloadDataGrid.ItemsSource as List<DownloadInfo>;
                downloadList.Remove(downloadInfo);
                try
                {
                    File.Delete(downloadInfo.SavePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                RemoveEntryFromFile(downloadInfo);
                downloadDataGrid.Items.Refresh();
            }
        }

        private void RemoveEntryFromFile(DownloadInfo downloadInfo)
        {
            try
            {
                string[] lines = File.ReadAllLines(downloadInfoFile);
                using (StreamWriter writer = new StreamWriter(downloadInfoFile + ".tmp"))
                {
                    foreach (string line in lines)
                    {
                        if (!line.StartsWith($"{downloadInfo.FileName},{downloadInfo.SavePath},"))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
                File.Delete(downloadInfoFile);
                File.Move(downloadInfoFile + ".tmp", downloadInfoFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while removing the entry from the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }
}
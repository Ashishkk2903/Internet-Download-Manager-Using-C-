using System.Windows;
using System;
using System.IO;

namespace Internetdownloadmanager
{
    public partial class Window3 : Window
    {
        private const string SavePathFileName = "savePath.txt";
        public Window3()
        {
            InitializeComponent();
            savePathTextBox.Text = GlobalVariables.SavePath;
            LoadSavePath();
        }
        private void LoadSavePath()
        {
            try
            {
                if (!File.Exists(SavePathFileName))
                {
                    
                    File.WriteAllText(SavePathFileName, "");
                }

                
                string savedPath = File.ReadAllText(SavePathFileName);
                GlobalVariables.SavePath = savedPath;
                savePathTextBox.Text = savedPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading save path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveSavePath()
        {
            try
            {
                File.WriteAllText(SavePathFileName, savePathTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving save path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                savePathTextBox.Text = dialog.SelectedPath;
                string selectedPath = dialog.SelectedPath;

                if (!selectedPath.EndsWith("\\"))
                {
                    selectedPath += "\\";
                }
                savePathTextBox.Text = selectedPath;

            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.SavePath = savePathTextBox.Text;
            SaveSavePath();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

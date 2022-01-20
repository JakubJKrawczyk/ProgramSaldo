using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using ProgramPraca.Data;
using System.IO;
using System.Windows;
namespace ProgramPraca
{
    /// <summary>
    /// Logika interakcji dla klasy PolaczenieSettings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Mongo.LoadConnectionSettings();
            TextHostProp.Text = Mongo.ServerName;
            TextDatabaseProp.Text = Mongo.DataBaseName;
            TextDatatableProp.Text = Mongo.CollectionName;
            TextFilePath.Text = Logger.LogsPath;
            TextBackupPath.Text = Mongo.BackupPath;
        }

        private void ZapiszUstawienia(object sender, RoutedEventArgs e)
        {
           
            File.WriteAllText("ConnectionSettings.txt",$"SERVER = {TextHostProp.Text}\n"+
            $"DATABASE = {TextDatabaseProp.Text}\nTABLE = {TextDatatableProp.Text}\nLOGS_PATH = {TextFilePath.Text}\nBACKUP_PATH = {TextBackupPath.Text}");
            Mongo.LoadConnectionSettings();
            Close();
        }

        private void FilePath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new();
            fileDialog.Filter = "Text files (*.txt)|*.txt";
            if (fileDialog.ShowDialog() == true)
            { 
                TextFilePath.Text = fileDialog.FileName;

            };
            
        }
        private void BackupPath(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog Dialog = new();
            if (Dialog.ShowDialog(this).GetValueOrDefault())
            {
                TextBackupPath.Text = Dialog.SelectedPath;
                Mongo.BackupPath = Dialog.SelectedPath;
            }
           
        }
    }
}

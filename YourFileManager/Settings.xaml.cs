using System.IO;
using System.Security.Cryptography;
using System.Text;
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

            TextHostProp.Text = MongoDb.ServerName;
            TextUserProp.Text = MongoDb.UserName;
            TextPasswdProp.Text = MongoDb.Password;
            TextDatabaseProp.Text = MongoDb.DataBaseName;
            TextDatatableProp.Text = MongoDb.CollectionName;
        }

        private void ZapiszUstawienia(object sender, RoutedEventArgs e)
        {
           
            File.WriteAllText("ConnectionSettings.txt",$"SERVER = {TextHostProp.Text}\n"+
            $"USERNAME = {TextUserProp.Text}\nPASSWORD = {TextPasswdProp.Text}\nDATABASE = {TextDatabaseProp.Text}\nTABLE = {TextDatatableProp.Text}");
            MongoDb.LoadConnectionSettings();
            Close();
        }
    }
}

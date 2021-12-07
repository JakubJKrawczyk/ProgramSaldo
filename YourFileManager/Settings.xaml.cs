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

            TextHostProp.Text = ConnectDataBase.Server;
            TextUserProp.Text = ConnectDataBase.User;
            TextPasswdProp.Text = ConnectDataBase.Password;
            TextDatabaseProp.Text = ConnectDataBase.DataBase;
            TextDatatableProp.Text = ConnectDataBase.Table;
        }

        private void ZapiszUstawienia(object sender, RoutedEventArgs e)
        {
           
            File.WriteAllText("ConnectionSettings.txt",$"SERVER = {TextHostProp.Text}\n"+
            $"USERNAME = {TextUserProp.Text}\nPASSWORD = {TextPasswdProp.Text}\nDATABASE = {TextDatabaseProp.Text}\nTABLE = {TextDatatableProp.Text}");
            ConnectDataBase.LoadSettings();
            Close();
        }
    }
}

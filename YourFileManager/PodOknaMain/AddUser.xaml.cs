using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public ConnectDataBase manager = new();
        public AddUser()
        {
            InitializeComponent();
            
        }

        private void SaveUser(object sender, RoutedEventArgs e)
        {
            manager.MakeConnection();
            MySqlCommand cmd = new();
            cmd.Connection = manager.Connection;
            object selectedItem = EnumNewType.SelectedItem;
            string selected = selectedItem.ToString().Split(":")[1].Trim();
            cmd.CommandText = $"Insert Into user(Login,Haslo,Typ) Values('{TextNewUsername.Text}','{TextNewPassword.Text}','{selected}')";
            MessageBox.Show(cmd.CommandText);
            cmd.Connection.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }catch(Exception ex)
            {
                MessageBox.Show($"Error: \n\n {ex.Message}");
            }
            cmd.Connection.Close();
            Close();
        }
    }
}

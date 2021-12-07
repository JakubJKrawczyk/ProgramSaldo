using System.Data;
using System.Windows;
using System.Windows.Input;
namespace ProgramPraca
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        ConnectDataBase manager = new ConnectDataBase();

        public object Keys { get; private set; }

        public Login()
        {
            InitializeComponent();
            manager.MakeConnection();
            TextLogin.Text = "";
            TextPassword.Password = "";

            
        }

        private void login(object sender, RoutedEventArgs e)
        {
            string login = TextLogin.Text.Trim();
            string haslo = TextPassword.Password.Trim();
            if (login=="" || haslo== "")
            {
                MessageBox.Show("Pola Login i Hasło nie mogą być puste!");
                return;
            }
            manager.MakeConnection();
            DataTable UserTable = new();
            UserTable = manager.GetDataFromQuerry($"SELECT UserID, Login, Typ from user WHERE Login = '{TextLogin.Text.Trim()}' AND Haslo = '{TextPassword.Password.Trim()}'");
            if (UserTable != null)
            {
                if (UserTable.Rows.Count == 0)
                {
                    MessageBox.Show("Zły login lub hasło!");
                }
                else
                {
                    DataRow Row = UserTable.Rows[0];
                    
                    User.Username = Row[1].ToString();
                    User.Type = Row[2].ToString();
                    
                    
                    var appWindow = new Main();
                    
                    appWindow.Show();
                    this.Close();
                    
                    
                }
            }
            else
            {
                return;
            }
            
        }

        private void KeyDownE(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                login(sender, e);
            }
        }

        private void polaczenie(object sender, RoutedEventArgs e)
        {
            var oknoUstawien = new Settings();
            oknoUstawien.Owner = this;
            oknoUstawien.Show();
        }
    }
}

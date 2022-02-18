using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProgramPraca.Data;
using System;
using System.Data;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
namespace ProgramPraca

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public object Keys { get; private set; }
        public IMongoCollection<BsonDocument> Users { get; set; }

        public Login()
        {
            InitializeComponent();
            TextLogin.Text = "";
            TextPassword.Password = "";

            
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("Aplikacja musi zostać uruchomiona z uprawnieniami administratora!");
                Application.Current.Shutdown();
            };
            

        }

        private void login(object sender, RoutedEventArgs e)
        {
            string login = TextLogin.Text.Trim();
            string haslo = TextPassword.Password.Trim();
            if (login == "" || haslo == "")
            {
                MessageBox.Show("Pola Login i Hasło nie mogą być puste!");
                return;
            }


            //Nowy Sposób MongoDB
            Mongo.MakeConnection();

            Users = Mongo.Database.GetCollection<BsonDocument>("users");
            var filter = new BsonDocument();
            filter.Add("name", "users");
            var listOfCollectionNames = Mongo.Database.ListCollections(new ListCollectionsOptions { Filter = filter });

            if (!listOfCollectionNames.Any())
            {

            };
            if (Users.CountDocuments($"{{}}") == 0) Mongo.InsertAdmin();

            if (Logger.LogsPath == "")
            {
                MessageBox.Show("Wskaż ścieżkę do pliku, w którym chcesz zapisywać historie działań programu!");
                return;
            }
            FilterDefinition<BsonDocument> Filter = Builders<BsonDocument>.Filter.Eq("Login", login);
            BsonDocument user = null;
            try
            {

            user = Users.Find(Filter).SingleOrDefault();
            }catch (Exception ex)
            {
                MessageBox.Show($"Błąd połączenia z bazą danych! Sprawdź ustawienia i spróbuj ponownie.\n\nERROR: {ex.Message}");
                Application.Current.Shutdown();
            }



            if (user != null)
            {

               if (user["Haslo"] == haslo)
                {


                  Main appWindow = new Main(user);

                    appWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Zły login lub hasło!");
                    return;
                }

            }
            else
            {
                MessageBox.Show("Nie ma takiego użytkownika!");
            }






        }

        private void KeyDownE(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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

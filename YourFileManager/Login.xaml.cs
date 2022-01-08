﻿using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
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
        public object Keys { get; private set; }

        public Login()
        {
            InitializeComponent();
            
            TextLogin.Text = "";
            TextPassword.Password = "";

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

            MongoDb.MakeConnection();
            FilterDefinition<UserModel> Filter = Builders<UserModel>.Filter.Eq(a => a.UserLogin, login);

            var userCollection = MongoDb.Database.GetCollection<UserModel>("user");

            UserHolder.User = userCollection.Find(Filter).SingleOrDefault();

            if (UserHolder.User != null)
            {
                if (UserHolder.User.UserPassword == haslo)
                {


                    Main appWindow = new();

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
                return;
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

using MongoDB.Bson;
using MongoDB.Driver;
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

namespace ProgramPraca
{
    /// <summary>
    /// Logika interakcji dla klasy MyAccount.xaml
    /// </summary>
    public partial class MyAccount : Window
    {
        public MyAccount()
        {
            InitializeComponent();
            LabelUserName.Text = UserHolder.User.UserLogin;
            TextBoxNewPassword.Text = UserHolder.User.UserPassword;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IMongoCollection<BsonDocument> users = Mongo.Database.GetCollection<BsonDocument>("user");
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("Haslo", TextBoxNewPassword.Text);
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Login", LabelUserName.Text);
            try
            {

            users.UpdateOne(filter, update);
            }catch(Exception error)
            {
                MessageBox.Show($"Error: {error.Message}");
                return;
            }
            MessageBox.Show("Hasło zostało zaktualizowane.");
        }
    }
}

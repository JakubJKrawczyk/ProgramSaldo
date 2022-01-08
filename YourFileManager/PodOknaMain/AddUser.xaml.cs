using MongoDB.Bson;
using MongoDB.Driver;
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
        public AddUser()
        {
            InitializeComponent();
            
        }

        private void SaveUser(object sender, RoutedEventArgs e)
        {
            
            
            UserModel newUser = new();
            newUser.UserId = ObjectId.GenerateNewId();
            newUser.UserLogin = TextNewUsername.Text;
            newUser.UserPassword = TextNewPassword.Text;
            
            newUser.UserType = EnumNewType.SelectedItem.ToString();
            IMongoCollection<BsonDocument> users = MongoDb.Database.GetCollection<BsonDocument>("user");

            users.InsertOne(newUser.ToBsonDocument());
            MessageBox.Show($"Sukces! Dodałeś nowego użytkownika! \nOto jego dane:\n{newUser.ToJson()}");
            Close();

            
        }
        
    }
}

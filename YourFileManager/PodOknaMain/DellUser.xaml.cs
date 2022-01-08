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

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy DellUser.xaml
    /// </summary>
    public partial class DellUser : Window
    {
        public DellUser()
        {
            InitializeComponent();
            IMongoCollection<BsonDocument> users = MongoDb.Database.GetCollection<BsonDocument>("user");
            List<string> usersNames = new();
            foreach(BsonDocument doc in users.AsQueryable())
            {
                usersNames.Add(doc["Login"].AsString);
            }
            ComboBoxUser.ItemsSource = usersNames;
            ComboBoxUser.SelectedItem = ComboBoxUser.Items[0];
        }

        

        private void DellUserFunc(object sender, RoutedEventArgs e)
        {
            string userName = ComboBoxUser.SelectedItem.ToString();
            MessageBox.Show(userName);
            if (UserHolder.User.UserLogin == userName)
            {

            }
            IMongoCollection<BsonDocument> users = MongoDb.Database.GetCollection<BsonDocument>("user");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Login", userName);

            MessageBox.Show($"Usunięto {users.DeleteOne(filter).DeletedCount} użytkowników.");
            Close();
        }
    }
}

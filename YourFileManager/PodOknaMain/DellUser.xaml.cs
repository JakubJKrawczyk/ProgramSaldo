using MongoDB.Bson;
using MongoDB.Driver;
using ProgramPraca.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            IMongoCollection<BsonDocument> users = Mongo.Database.GetCollection<BsonDocument>("user");
            List<string> usersNames = new();
            foreach(BsonDocument doc in users.AsQueryable())
            {
                usersNames.Add(doc["Login"].AsString);
            }
            ListBoxUsers.ItemsSource = usersNames;
            
        }

        

        private void DellUserFunc(object sender, RoutedEventArgs e)
        {
            var userNames = ListBoxUsers.SelectedItems;
            IMongoCollection<BsonDocument> users = Mongo.Database.GetCollection<BsonDocument>("user");
            long count = 0;
            foreach (string userName in userNames)
            {
                if (UserHolder.User.UserLogin == userName) continue;
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Login", userName);
                Logger.DeletedUser = userName;
                Logger.CreateAction(4);
                count += users.DeleteOne(filter).DeletedCount;

            }
            MessageBox.Show($"Usunięto {count} użytkowników.");

            DellUser w = new();
            w.Show();
            Close();
        }
    }
}

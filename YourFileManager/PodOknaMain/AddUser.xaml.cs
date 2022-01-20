using MongoDB.Bson;
using MongoDB.Driver;
using ProgramPraca.Data;
using System.Collections.Generic;
using System.Windows;

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
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            List<string> userTypes = new();
            userTypes.Add("uzytkownik");
            userTypes.Add("administrator");
            ComboBoxUserType.ItemsSource = userTypes;
            ComboBoxUserType.SelectedItem = ComboBoxUserType.Items[0];
            TextNewUsername.Text = "";
            TextNewPassword.Text = "";
        }

        private void SaveUser(object sender, RoutedEventArgs e)
        {
            
            if(TextNewUsername.Text != "")
            {
                UserModel newUser = new();
                newUser.UserId = ObjectId.GenerateNewId();
                newUser.UserLogin = TextNewUsername.Text;
                newUser.UserPassword = TextNewPassword.Text;

                newUser.UserType = ComboBoxUserType.SelectedItem.ToString();
                IMongoCollection<BsonDocument> users = Mongo.Database.GetCollection<BsonDocument>("user");

                users.InsertOne(newUser.ToBsonDocument());
                MessageBox.Show($"Sukces! Dodałeś nowego użytkownika! \nOto jego dane:\n{newUser.ToJson()}");
                Logger.AddedUser = newUser.UserLogin;
                Logger.CreateAction(3);
                AddUser w = new();
                w.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Pole UserName nie może być puste!");
                return;
            }
            

            
        }
        
    }
}

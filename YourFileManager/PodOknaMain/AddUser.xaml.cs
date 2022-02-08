using MongoDB.Bson;
using MongoDB.Driver;
using ProgramPraca.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
       readonly List<CheckBox> checkBoxes = new();
        public AddUser()
        {
            checkBoxes.Add(CheckBoxFaktury);
            checkBoxes.Add(CheckBoxKadry);
            checkBoxes.Add(CheckBoxKsiegi);

            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            List<string> userTypes = new();
            userTypes.Add("uzytkownik");
            userTypes.Add("administrator");
            userTypes.Add("superadministrator");
            ComboBoxUserType.ItemsSource = userTypes;
            ComboBoxUserType.SelectedItem = ComboBoxUserType.Items[0];
            TextNewUsername.Text = "";
            TextNewPassword.Text = "";
        }

        private void SaveUser(object sender, RoutedEventArgs e)
        {

            if (TextNewUsername.Text != "")
            {
                string privilages = "";
                foreach (var checkbox in checkBoxes)
                {
                    if (checkbox.IsChecked == true)
                    {
                        privilages += $"{checkbox.Name.Trim().ToLower()};";
                    }
                }
                UserModel newUser = new();
                newUser.UserId = ObjectId.GenerateNewId();
                newUser.UserLogin = TextNewUsername.Text;
                newUser.UserPassword = TextNewPassword.Text;
                newUser.Privilages = privilages;
                newUser.UserType = ComboBoxUserType.SelectedItem.ToString();
                IMongoCollection<BsonDocument> users = Mongo.Database.GetCollection<BsonDocument>("user");
                users.InsertOne(newUser.ToBsonDocument());
                LabelStatus.Name = $"Sukces! Dodałeś nowego użytkownika! \nOto jego dane:\n{newUser.ToJson()}";
                Logger.AddedUser = newUser.UserLogin;
                Logger.CreateAction(3);

                SetToDefault();
            }
            else
            {
                MessageBox.Show("Pole UserName nie może być puste!");
                return;
            }



        }
        private void SetToDefault()
        {
            ComboBoxUserType.SelectedItem = ComboBoxUserType.Items[0];
            TextNewUsername.Text = "";
            TextNewPassword.Text = "";

            foreach (CheckBox checkbox in checkBoxes)
            {
                checkbox.IsChecked = false;
            }
        }

    }
}

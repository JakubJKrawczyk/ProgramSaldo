using MongoDB.Bson;
using MongoDB.Driver;
using ProgramPraca.Data;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy AddColumn.xaml
    /// </summary>
    public partial class AddColumn : Window
    {

        public Dictionary<string,string> ColumnsWithColors { get; set; } = new();
        IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}");
        IMongoCollection<BsonDocument> collectionColumns = Mongo.Database.GetCollection<BsonDocument>($"columns-{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}");


        public AddColumn()
        {
            InitializeComponent();
            ChangeEnumVisibility();


            //combobox of types
            ComboBoxOwnerType.Items.Add("Kadry");
            ComboBoxOwnerType.Items.Add("Księgi");
            ComboBoxOwnerType.Items.Add("Faktury");
            ComboBoxOwnerType.Items.Add("Ogólny");
            ComboBoxOwnerType.SelectedItem = ComboBoxOwnerType.Items[0];
            //

            var Colors = Mongo.Database.GetCollection<BsonDocument>("colors");
            if (Colors != null)
            {
                foreach(var color in Colors.FindAsync<BsonDocument>($"{{}}").Result.ToList())
                {
                    ListBoxColors.Items.Add(color["name"]);
                    
                    
                }
            }
            else
            {
                Mongo.Database.CreateCollection("colors");
                BsonDocument defaultColor = new();
                defaultColor.Add("name", "white");
                defaultColor.Add("value", "#FFFFFF");
                var colors = Mongo.Database.GetCollection<BsonDocument>("colors");
                colors.InsertOneAsync(defaultColor);
                BsonDocument firstColor = colors.Find($"{{}}").FirstOrDefault();
                if (firstColor != null)
                {
                    ListBoxColors.Items.Add(firstColor["name"]);
                }
            }
            

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {


            if (TextBoxColumnName.Text == "" || TextBoxColumnName.Text == null) { MessageBox.Show("Nazwa kolumny nie może być pusta!"); return; }
            if (CheckBoxIsEnum.IsChecked == false && CheckBoxIsChecked.IsChecked == false)
            {
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set(TextBoxColumnName.Text, "");
                BsonDocument column = new();
                column.Add("columnName", TextBoxColumnName.Text);
                column.Add("columnType", "string");
                column.Add("columnOwner", ComboBoxOwnerType.SelectedItem.ToString());
                if (ListBoxColors.SelectedItem is null)
                {
                    ListBoxColors.SelectedItem = ListBoxColors.Items[0];
                }
                column.Add("columnColor", ListBoxColors.SelectedItem.ToString());

                collection.UpdateManyAsync($"{{}}", update);

                collectionColumns.InsertOneAsync(column);
                Mongo.ChangeCount($"{{}}", true, collection);
                Mongo.FillDataGrid(Main.SelectedDate, Main.dt);



            }
            else if(CheckBoxIsEnum.IsChecked == true)
            {
               
                if(ColumnsWithColors.Count == 0) { MessageBox.Show("Lista opcji pola wyboru nie może być pusta!"); return; }
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set(TextBoxColumnName.Text, "");
                collection.UpdateManyAsync($"{{}}", update);

                string columnEnumValues = "";

                foreach (var element in ColumnsWithColors)
                {
                    columnEnumValues += $"{element.Key}:{element.Value};";
                }
                BsonDocument column = new();
                column.Add("columnName", TextBoxColumnName.Text);
                column.Add("columnType", "enum");

                column.Add("columnOwner", ComboBoxOwnerType.SelectedItem.ToString());
                column.Add("columnEnumValues", columnEnumValues);
                
                
                collectionColumns.InsertOneAsync(column);
                Mongo.ChangeCount($"{{}}", true, collection);
                Mongo.FillDataGrid(Main.SelectedDate, Main.dt);
            }
            else
            {
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set(TextBoxColumnName.Text, "falsz");
                collection.UpdateManyAsync($"{{}}", update);
                BsonDocument column = new();
                column.Add("columnName", TextBoxColumnName.Text);
                column.Add("columnType", "check");
                column.Add("columnOwner", ComboBoxOwnerType.SelectedItem.ToString());
                collectionColumns.InsertOneAsync(column);
                Mongo.ChangeCount($"{{}}", true, collection);
                Mongo.FillDataGrid(Main.SelectedDate, Main.dt);
            }
            
            //Logs
            Logger.AddedColumn = TextBoxColumnName.Text;
            Logger.CreateAction(1);
            //
            AddColumn w = new();
            w.Show();
            Close();

        }

       
        private void ChangeEnumVisibility()
        {
            if (CheckBoxIsEnum.IsChecked == false)
            {
                ListBoxEnumValues.Visibility = Visibility.Hidden;
                TextBoxNewEnumValue.Visibility = Visibility.Hidden;
                plus.Visibility = Visibility.Hidden;
                minus.Visibility = Visibility.Hidden;

            }
            else
            {
                ListBoxEnumValues.Visibility = Visibility.Visible;
                TextBoxNewEnumValue.Visibility = Visibility.Visible;
                plus.Visibility = Visibility.Visible;
                minus.Visibility = Visibility.Visible;
            }
        }

        private void PlusPressed(object sender, RoutedEventArgs e)
        {
            if (ColumnsWithColors.ContainsKey(TextBoxNewEnumValue.Text)) { MessageBox.Show($"Opcja {TextBoxNewEnumValue.Text} już istnieje!"); TextBoxNewEnumValue.Text = ""; return; }
            string EnumColor = ListBoxColors.SelectedItem == null ? ListBoxColors.Items[0].ToString() : ListBoxColors.SelectedItem.ToString();
            ListBoxEnumValues.Items.Add($"{TextBoxNewEnumValue.Text}:{EnumColor}");
            ColumnsWithColors.Add(TextBoxNewEnumValue.Text, EnumColor);
            TextBoxNewEnumValue.Text = "";
            
        }

        private void MinusPressed(object sender, RoutedEventArgs e)
        {
            ColumnsWithColors.Remove(ListBoxEnumValues.SelectedItem.ToString());
            ListBoxEnumValues.Items.Remove(ListBoxEnumValues.SelectedItem);

        }

        private void CheckBoxIsEnum_Click(object sender, RoutedEventArgs e)
        {
            CheckBoxIsChecked.IsEnabled = CheckBoxIsEnum.IsChecked == true ? false : true;
            ChangeEnumVisibility();
            
        }

        private void OnSelectChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void CheckBoxIsCheck(object sender, RoutedEventArgs e)
        {
            CheckBoxIsEnum.IsEnabled = CheckBoxIsChecked.IsChecked == true ? false : true;
            
        }
    }
}

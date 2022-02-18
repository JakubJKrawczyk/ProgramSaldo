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

        public DateTime Date { get; set; }
        public Dictionary<string,int> ColumnsWithColors { get; set; } = new();
        public AddColumn(DateTime date)
        {
            InitializeComponent();
            ChangeEnumVisibility();
            Date = date;


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
                colors.InsertOne(defaultColor);
                BsonDocument firstColor = colors.Find($"{{}}").FirstOrDefault();
                if (firstColor != null)
                {
                    ListBoxColors.Items.Add(firstColor["name"]);
                }
            }
            

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxIsEnum.IsChecked == false)
            {
                IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{Date.Year}-{Date.Month}");
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set(TextBoxColumnName.Text, "");
                BsonDocument column = new();
                column.Add("columnName", TextBoxColumnName.Text);
                // column.Add("columnColor", );
                column.Add("columnOwner", ComboBoxOwnerType.SelectedItem.ToString());
                if (ListBoxColors.SelectedItem is null)
                {
                    ListBoxColors.SelectedItem = ListBoxColors.Items[0];
                }
                column.Add("columnColor", ListBoxColors.SelectedItem.ToString());

                collection.UpdateMany($"{{}}", update);

                var collectionColumns = Mongo.Database.GetCollection<BsonDocument>($"columns-{Mongo.CollectionName}-{Date.Year}-{Date.Month}");
                collectionColumns.InsertOne(column);
                Mongo.ChangeCount($"{{}}", true, collection);
                Mongo.FillDataGrid(Date, Main.dt);



            }
            else
            {
                IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{Date.Year}-{Date.Month}");
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set(TextBoxColumnName.Text, "");
                collection.UpdateMany($"{{}}", update);

                var collectionColumns = Mongo.Database.GetCollection<BsonDocument>($"columns-{Mongo.CollectionName}-{Date.Year}-{Date.Month}");
                string columnEnumValues = "";
                foreach(var value in ListBoxEnumValues.Items)
                {
                    columnEnumValues += $"{value};";
                }
                BsonDocument column = new();
                column.Add("columnName", TextBoxColumnName.Text);
                column.Add("columnOwner", ComboBoxOwnerType.SelectedItem.ToString());
                column.Add("columnEnumValues", columnEnumValues);
                if(ListBoxColors.SelectedItem is null)
                {
                    ListBoxColors.SelectedItem = ListBoxColors.Items[0];
                }
                column.Add("columnColor", ListBoxColors.SelectedItem.ToString());
                collectionColumns.InsertOneAsync(column);
                Mongo.ChangeCount($"{{}}", true, collection);
                Mongo.FillDataGrid(Date, Main.dt);
            }
            
            //Logs
            Logger.AddedColumn = TextBoxColumnName.Text;
            Logger.CreateAction(1);
            //
            AddColumn w = new(Date);
            w.Show();
            Close();

        }

        private void CreateEnum(object selectedItems)
        {

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
            ListBoxEnumValues.Items.Add(TextBoxNewEnumValue.Text);
            ColumnsWithColors.Add(TextBoxNewEnumValue.Text, 0);
            TextBoxNewEnumValue.Text = "";
            
        }

        private void MinusPressed(object sender, RoutedEventArgs e)
        {
            ColumnsWithColors.Remove(ListBoxEnumValues.SelectedItem.ToString());
            ListBoxEnumValues.Items.Remove(ListBoxEnumValues.SelectedItem);

        }

        private void CheckBoxIsEnum_Click(object sender, RoutedEventArgs e)
        {
            ChangeEnumVisibility();
        }

        private void OnSelectChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}

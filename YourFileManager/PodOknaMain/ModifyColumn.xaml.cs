using MongoDB.Bson;
using MongoDB.Driver;
using ProgramPraca.Data;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy ModifyColumn.xaml
    /// </summary>
    /// 


    public partial class ModifyColumn : Window
    {

        public IMongoCollection<BsonDocument> Collection { get; set; }
        Dictionary<string, string> nameColorsPairs = new();
        public ModifyColumn()
        {
            InitializeComponent();
            Collection = Mongo.Database.GetCollection<BsonDocument>($"columns-{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}");
            
            ComboboxColumn.ItemsSource = Mongo.GetColumnsNamesFromomColumnsCollection(Collection);

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ComboboxColumn.SelectedItem is null) return;
            
            var collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}");

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Rename(ComboboxColumn.SelectedItem.ToString(), TextBoxNewName.Text);
            collection.UpdateManyAsync($"{{}}", update);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("collectioName", ComboboxColumn.SelectedItem.ToString());
            update = Builders<BsonDocument>.Update.Set("collectioName", ComboboxColumn.SelectedItem.ToString());
            Collection.UpdateOneAsync(filter, update);
            Mongo.FillDataGrid(Main.SelectedDate, Main.dt);

            //logs
            Logger.ColumnOldName = ComboboxColumn.SelectedItem.ToString();
            Logger.ColumnNewName = TextBoxNewName.Text;
            Logger.CreateAction(5);
            //
            ComboboxColumn.Items.Clear();
            ComboboxColumn.ItemsSource = Mongo.GetColumnsNamesFromomColumnsCollection(Collection);
            ComboboxColumn.SelectedItem = ComboboxColumn.Items[0];
            TextBoxNewName.Text = "";

        }

        private void ComboboxColumn_DataContextChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("columnName", ComboboxColumn.SelectedItem.ToString());
            BsonDocument selectedColumn = Collection.Find(filter).SingleOrDefaultAsync().Result;
            if (selectedColumn["columnType"] == "enum")
            {
                string[] enumValuesWithColors = selectedColumn["columnEnumValues"].ToString().Split(";");
                IMongoCollection<BsonDocument> colors = Mongo.Database.GetCollection<BsonDocument>("colors");
                foreach (BsonDocument color in colors.FindAsync($"{{}}").Result.ToList())
                {
                  
                    ListBoxColors.Items.Add(color["name"]);
                }
                foreach(var pair in enumValuesWithColors)
                {
                    if (pair == "") continue;
                    ListBoxEnumValues.Items.Add(pair);
                    nameColorsPairs.Add(pair.Split(":")[0], pair.Split(":")[1]);
                }
            }else if(selectedColumn["columnType"] == "check")
            {

            }
            else
            {

            }
        }

        private void MinusPressed(object sender, RoutedEventArgs e)
        {

        }

        private void PlusPressed(object sender, RoutedEventArgs e)
        {

        }
    };
}

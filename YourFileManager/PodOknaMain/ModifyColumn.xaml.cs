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
        public DateTime Date { get; set; }

        public IMongoCollection<BsonDocument> Collection { get; set; }
        public ModifyColumn(DateTime date)
        {
            InitializeComponent();
            Date = date;   
            Collection = Mongo.Database.GetCollection<BsonDocument>($"columns-{Mongo.CollectionName}-{Date.Year}-{Date.Month}");
            
            ComboboxColumn.ItemsSource = Mongo.GetColumnsNamesFromCollection(Collection);

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ComboboxColumn.SelectedItem is null) return;
            
            var collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{Date.Year}-{Date.Month}");

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Rename(ComboboxColumn.SelectedItem.ToString(), TextBoxNewName.Text);
            collection.UpdateManyAsync($"{{}}", update);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("collectioName", ComboboxColumn.SelectedItem.ToString());
            update = Builders<BsonDocument>.Update.Set("collectioName", ComboboxColumn.SelectedItem.ToString());
            Collection.UpdateOneAsync(filter, update);
            Mongo.FillDataGrid(Date,Main.dt);

            //logs
            Logger.ColumnOldName = ComboboxColumn.SelectedItem.ToString();
            Logger.ColumnNewName = TextBoxNewName.Text;
            Logger.CreateAction(5);
            //
            ComboboxColumn.Items.Clear();
            ComboboxColumn.ItemsSource = Mongo.GetColumnsNamesFromCollection(Collection);
            ComboboxColumn.SelectedItem = ComboboxColumn.Items[0];
            TextBoxNewName.Text = "";

        }

        
    };
}

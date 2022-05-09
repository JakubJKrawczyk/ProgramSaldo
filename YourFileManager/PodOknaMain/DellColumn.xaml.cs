using MongoDB.Bson;
using MongoDB.Driver;
using ProgramPraca.Data;
using System;
using System.Windows;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy DellColumn.xaml
    /// </summary>
    public partial class DellColumn : Window
    {

        public DellColumn()
        {
            InitializeComponent();
            

            if (Mongo.CheckIfCollectonExists($"columns-{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}")) { 
                IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>($"columns-{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}");
                var docs = collection.FindAsync($"{{}}").Result.ToList();
                
                foreach (BsonDocument column in docs)
                {
                    ComboboxColumn.Items.Add(column["columnName"]);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ComboboxColumn.SelectedItem is not null)
            {
                IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}");
                IMongoCollection<BsonDocument> collectionColumns = Mongo.Database.GetCollection<BsonDocument>($"columns-{Mongo.CollectionName}-{Main.SelectedDate.Year}-{Main.SelectedDate.Month}");
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Unset(ComboboxColumn.SelectedItem.ToString());

                collection.UpdateManyAsync($"{{}}", update);
                Mongo.ChangeCount($"{{}}", false, collection);
                Mongo.FillDataGrid(Main.SelectedDate, Main.dt);

                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("columnName", ComboboxColumn.SelectedItem.ToString());
                
                collectionColumns.DeleteOneAsync(filter);

                //logs
                Logger.DeletedColumn = ComboboxColumn.SelectedItem.ToString();
                Logger.CreateAction(2);
                //
                Close();
            }
            
        }
    }
}

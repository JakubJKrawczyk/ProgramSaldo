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
        public DateTime Date { get; set; }

        public DellColumn(DateTime date)
        {
            InitializeComponent();
            Date = date;    
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ComboboxColumn.SelectedItem is not null)
            {
                IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>(Mongo.CollectionName);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Unset(ComboboxColumn.SelectedItem.ToString());

                collection.UpdateMany($"{{}}", update);
                Mongo.ChangeCount($"{{}}", false, collection);
                Mongo.FillDataGrid(Date, Main.dt);
                
                //logs
                Logger.DeletedColumn = ComboboxColumn.SelectedItem.ToString();
                Logger.CreateAction(2);
                //
                Close();
            }
            
        }
    }
}

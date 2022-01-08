using MongoDB.Bson;
using MongoDB.Driver;
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
            ComboboxColumn.ItemsSource = Main.columns;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ComboboxColumn.SelectedItem is not null)
            {
                IMongoCollection<BsonDocument> collection = MongoDb.Database.GetCollection<BsonDocument>("klienci");
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Unset(ComboboxColumn.SelectedItem.ToString());

                collection.UpdateMany($"{{}}", update);
                MongoDb.changeCount($"{{}}", false, collection);
                MongoDb.FillDataGrid(Main.dt);
                Main.FillListOfColumns();
                Close();
            }
            
        }
    }
}

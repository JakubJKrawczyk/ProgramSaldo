using MongoDB.Bson;
using MongoDB.Driver;
using System.Windows;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy AddColumn.xaml
    /// </summary>
    public partial class AddColumn : Window
    {
        public AddColumn()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IMongoCollection<BsonDocument> collection = MongoDb.Database.GetCollection<BsonDocument>("klienci");

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set(TextBoxColumnName.Text, "");
            
            collection.UpdateMany($"{{}}", update);
            MongoDb.changeCount($"{{}}", true, collection);
            MongoDb.FillDataGrid(Main.dt);
            Main.FillListOfColumns();
            Close();
        }
    }
}

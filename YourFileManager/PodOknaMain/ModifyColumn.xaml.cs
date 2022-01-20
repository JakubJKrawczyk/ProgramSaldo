using MongoDB.Bson;
using MongoDB.Driver;
using ProgramPraca.Data;
using System;
using System.Windows;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy ModifyColumn.xaml
    /// </summary>
    /// 


    public partial class ModifyColumn : Window
    {
        int Refresh = 0;
        public ModifyColumn()
        {
            InitializeComponent();
            ComboboxColumn.ItemsSource = Main.columns;
            Refresh = 0;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ComboboxColumn.SelectedItem is null)
            {
                return;
            }
            var collection = Mongo.Database.GetCollection<BsonDocument>(Mongo.CollectionName);

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Rename(ComboboxColumn.SelectedItem.ToString(), TextBoxNewName.Text);

            collection.UpdateMany($"{{}}", update);
            
            Mongo.FillDataGrid(Main.dt);
            Main.FillListOfColumns();

            //logs
            Logger.ColumnOldName = ComboboxColumn.SelectedItem.ToString();
            Logger.ColumnNewName = TextBoxNewName.Text;
            Logger.CreateAction(5);
            //
            ComboboxColumn.ItemsSource = Main.columns;
            var window = GetWindow(this);
            Refresh = 1;
            window.Close();

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Refresh == 1)
            {
                var newWindow = new ModifyColumn();

                newWindow.Show();
            }
        }
    };
}

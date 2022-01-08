using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Data;
using MySql.Data.MySqlClient;
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Windows.Input;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy ModifyColumn.xaml
    /// </summary>
    /// 
    

    public partial class ModifyColumn : Window
    {
        public int Refresh { get; set; }
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
            var collection = MongoDb.Database.GetCollection<BsonDocument>(MongoDb.CollectionName);

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Rename(ComboboxColumn.SelectedItem.ToString(), TextBoxNewName.Text);

            collection.UpdateMany($"{{}}", update);
            
            MongoDb.FillDataGrid(Main.dt);
            Main.FillListOfColumns();
            
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

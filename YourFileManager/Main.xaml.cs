
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Drawing;
using System.Windows.Media;

namespace ProgramPraca
{
    /// <summary>
    /// Logika interakcji dla klasy MainView.xaml
    /// </summary>
    public partial class Main : Window
    {
        public static DataGrid dt = new();

        public Main()
        {
            InitializeComponent();
            dt = dtGrid;

            if (UserHolder.User.UserType == "administrator")
            {
                ButtonManageColors.Visibility = Visibility.Hidden; 

            }else if(UserHolder.User.UserType == "superadministrator")
            {

            }
            else
            {
                ButtonManageUsers.Visibility = Visibility.Hidden;
                ButtonManageColumns.Visibility = Visibility.Hidden;
                ButtonManageColors.Visibility = Visibility.Hidden;
            }

            try
            {
                Mongo.FillDataGrid(CalendarMonthSelection.SelectedDate.Value, dt);

            }
            catch (Exception e)
            {
                MessageBox.Show($"Błąd wczytywania danych!.\n\nERROR: {e.Message}");

                Application.Current.Shutdown();

            }
            Mongo.CheckBackupDate();

        }




        //Windows Section

        private void ColumnManager(object sender, RoutedEventArgs e)
        {
            ColumnManager w = new ColumnManager(CalendarMonthSelection.SelectedDate.Value);
            w.Show();

        }

        private void UserManager(object sender, RoutedEventArgs e)
        {
            UserManager w = new();
            w.Show();
        }

        private void FiltrData(object sender, RoutedEventArgs e)
        {
            PodOknaMain.Filtr W = new();
            W.Show();
        }
        //


        private void ChangeData(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView row = (DataRowView)dt.SelectedItem;
            DataGridCell cell = e.EditingElement as DataGridCell;
            DateTime date = CalendarMonthSelection.SelectedDate.Value;
            
            var collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{date.Year}-{date.Month}");

            if (row[0].ToString() == "")
            {
                ObjectId newId = new ObjectId();
                newId = ObjectId.GenerateNewId();
                BsonDocument newDoc = new();
                newDoc.Add("_id", newId);
                TextBox value = e.EditingElement as TextBox;
                newDoc.Add(e.Column.Header.ToString(), value.Text);
                newDoc.Add("count", 1);
                collection.InsertOne(newDoc);
            }
            else
            {
                ObjectId id = new ObjectId(row[0].ToString());

                var filter = Builders<BsonDocument>.Filter.Eq("_id", id);

                BsonDocument rowToUpdate = collection.Find(filter).Single();
                if (!rowToUpdate.Contains(e.Column.Header.ToString()))
                {
                    Mongo.ChangeCount(filter, true, collection);

                }
                TextBox value = e.EditingElement as TextBox;
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set(e.Column.Header.ToString(), value.Text);



                collection.UpdateOne(filter, update);


            }



        }


        private void SetColumnsReadOnly(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {


            if (e.Column.Header.ToString() == "_id")
            {
                e.Column.IsReadOnly = true;
            }
            else if (e.Column.Header.ToString() == "count")
            {
                e.Column.Visibility = Visibility.Hidden;
            }

        }





        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                DateTime date = CalendarMonthSelection.SelectedDate.Value;
                Mongo.FillDataGrid(date, dt);
            }
        }







        private void dtGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            if (Key.Delete == e.Key)
            {
                DateTime date = CalendarMonthSelection.SelectedDate.Value;
                if (dt.Items.Count == 2) return;

                DataRowView row = dt.SelectedItem as DataRowView;
                IMongoCollection<BsonDocument> collection = Mongo.Database.GetCollection<BsonDocument>($"{Mongo.CollectionName}-{date.Year}-{date.Month}");
                ObjectId id = new ObjectId(row.Row[0].ToString());
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                collection.DeleteOne(filter);
                Mongo.FillDataGrid(date, dt);
            }
        }

        private void AccountManager(object sender, RoutedEventArgs e)
        {
            MyAccount w = new();

            w.Show();
        }

        private void OnDateChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Mongo.CheckIfMonthCollectonExists((DateTime)e.NewValue))
            {
                var currentDate = (DateTime)e.NewValue;
                Mongo.FillDataGrid(currentDate, dt);

            }
            else
            {
                var currentDate = (DateTime)e.NewValue;

                var previousDate = (DateTime)e.OldValue;
                PodOknaMain.PopUpAskCreateCollection w = new PodOknaMain.PopUpAskCreateCollection(CalendarMonthSelection, previousDate, currentDate);




            }
        }

        private void ManageColors(object sender, RoutedEventArgs e)
        {
            Window w = new();
            
        }

      
    }
}


using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace ProgramPraca
{
    /// <summary>
    /// Logika interakcji dla klasy MainView.xaml
    /// </summary>
    public partial class Main : Window
    {
        public static List<string> columns = new();
        private static DataGrid dt = new();

        public Main()
        {
            InitializeComponent();
            dt = dtGrid;
            if (User.Type != "administrator")
            {
                But1.Visibility = Visibility.Hidden;
                But2.Visibility = Visibility.Hidden;

            }
            ConnectDataBase manager = new ConnectDataBase();
            manager.MakeConnection();
            try
            {
                manager.FillDataGrid(dt);

            }
            catch (Exception e)
            {
                MessageBox.Show($"Błąd wczytywania danych! sprawdź ustawienia połączenia i spróbuj ponownie.\n\nERROR: {e.Message}");

                Application.Current.Shutdown();

            }


        }
        private void RefreshData()
        {
            ConnectDataBase manager = new();
            manager.MakeConnection();
            manager.FillDataGrid(dt);
        }
        private void AddUser(object sender, RoutedEventArgs e)
        {
            PodOknaMain.AddUser W = new();
            W.Show();
        }

        

        

        private void FiltrData(object sender, RoutedEventArgs e)
        {
            PodOknaMain.Filtr W = new();
            W.Show();
        }



        private void ChangeData(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConnectDataBase manager = new();
            DataGrid dt = sender as DataGrid;
            DataRowView row = (DataRowView)dt.SelectedItem;
            MySqlCommand cmd = new();

            if (e.Column.GetType().Name == "DataGridCheckBoxColumn")
            {
                CheckBox t = e.EditingElement as CheckBox;
                cmd.CommandText = $"UPDATE klienci SET {e.Column.Header} = {t.IsChecked} WHERE KlientID = {row[0]};";
            }
            else
            {
                TextBox t = e.EditingElement as TextBox;
                if (manager.CheckIfColumnHasAttributeNULL(e.Column.Header.ToString()) && t.Text is "")
                {
                    MessageBox.Show($"kolumna {e.Column.Header} nie przyjmuje pustych wartości");
                    return;
                }
                cmd.CommandText = $"UPDATE klienci SET {e.Column.Header} = '{t.Text}' WHERE KlientID = {row[0]}";

            }


            manager.MakeConnection();
            cmd.Connection = manager.Connection;
            cmd.Connection.Open();

            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:\n\n{ex.Message}");
            }

            cmd.Connection.Close();
        }

        private void SetColumnReadOnlyAndAddColumnsToList(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            columns.Add(e.Column.Header.ToString());
            if (e.Column.Header.ToString() == "KlientID")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void ColumnManager(object sender, RoutedEventArgs e)
        {
            ColumnManager w = new();
            w.Show();
        }
    }
}

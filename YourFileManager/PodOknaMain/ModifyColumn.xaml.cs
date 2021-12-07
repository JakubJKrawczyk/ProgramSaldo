using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Data;
using MySql.Data.MySqlClient;
using System;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy ModifyColumn.xaml
    /// </summary>
    public partial class ModifyColumn : Window
    {
        Dictionary<string, string> TypesOfColumn = new Dictionary<string, string>();
        ConnectDataBase manager = new();
        public ModifyColumn()
        {
            InitializeComponent();
            ComboboxColumn.ItemsSource = Main.columns;

            TypesOfColumn.Add("enum", "Pole Wyboru");
            TypesOfColumn.Add("int", "Pole Całkowito Liczbowe");
            TypesOfColumn.Add("decimal", "Pole Liczbowe");
            TypesOfColumn.Add("varchar", "Pole Tekstowe");
            TypesOfColumn.Add("date", "Pole Daty");
            TypesOfColumn.Add("tinyint", "Pole Check");
            ComboBoxColumnType.ItemsSource = TypesOfColumn.Values;
            ComboBoxColumnType.IsEnabled = false;
            //Default the ENUM edition fields are invisible
            HideEnumEditor();
        }


        private void HideEnumEditor()
        {

            TextBoxEnumValue.Visibility = Visibility.Hidden;
            ListBoxEnumValues.Visibility = Visibility.Hidden;
            ButtonEnumValueAdd.Visibility = Visibility.Hidden;
            ButtonEnumValueRemove.Visibility = Visibility.Hidden;



        }




        private void ShowEnumEditor()
        {

            TextBoxEnumValue.Visibility = Visibility.Visible;
            ListBoxEnumValues.Visibility = Visibility.Visible;
            ButtonEnumValueAdd.Visibility = Visibility.Visible;
            ButtonEnumValueRemove.Visibility = Visibility.Visible;

            manager.MakeConnection();


            var cmd = manager.Connection.CreateCommand();
            cmd.CommandText = $"SELECT SUBSTRING(COLUMN_TYPE,5) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = @database AND TABLE_NAME = 'klienci' AND COLUMN_NAME = @column";
            cmd.Parameters.AddWithValue("@database", ConnectDataBase.DataBase);
            cmd.Parameters.AddWithValue("@column", ComboboxColumn.SelectedItem.ToString());

            cmd.Connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            ListBoxEnumValues.Items.Clear();
            while (reader.Read())
            {

                string[] choices = reader.GetString(0).Replace("('", "").Replace("','", " ").Replace("')", "").Split(" ");
                foreach (var choice in choices)
                {
                    ListBoxEnumValues.Items.Add(choice);
                }

            }
            cmd.Connection.Close();


        }

        private void SetNewColumnParams(object sender, RoutedEventArgs e)
        {
            manager.MakeConnection();
            var type = TypesOfColumn.FirstOrDefault(r => r.Value == ComboBoxColumnType.SelectedItem.ToString()).Key;
            string check = CheckBoxIsNULL.IsChecked.Value == true ? "NULL" : "NOT NULL";
            MySqlCommand cmd = manager.Connection.CreateCommand();
            string numberOfMarks = type == "tinyint" ? "(1)" : type == "date" ? "" : type == "decimal" ? "(65)" : "(100)";
            if (type == "enum")
            {
                string enumValues = "(";

                foreach (var item in ListBoxEnumValues.Items)
                {
                    if (item == ListBoxEnumValues.Items[ListBoxEnumValues.Items.Count - 1])
                    {
                        enumValues += "'" + item.ToString() + "'";
                    }
                    else
                    {
                        enumValues += "'" + item.ToString() + "',";

                    }
                }
                enumValues += ")";
                cmd.CommandText = $"ALTER TABLE {ConnectDataBase.Table} MODIFY {ComboboxColumn.SelectedItem} {type}{enumValues} {check};";
            }
            else
            {
                cmd.CommandText = $"ALTER TABLE {ConnectDataBase.Table} MODIFY {ComboboxColumn.SelectedItem} {type}{numberOfMarks} {check};";

            }


            MessageBox.Show(cmd.CommandText);
            cmd.Connection.Open();
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show(cmd.CommandText);
            }
            catch (Exception error)
            {
                MessageBox.Show($"Error: {error.Message}");
            }
            cmd.Connection.Close();
        }


        private void CheckIfEnum()
        {
            if (ComboBoxColumnType.SelectedItem.ToString() == "Pole Wyboru")
            {
                ShowEnumEditor();
            }
            else
            {
                HideEnumEditor();
            }
        }

        private void AddNewEnumValue(object sender, RoutedEventArgs e)
        {
            if (TextBoxEnumValue.Text != null || TextBoxEnumValue.Text.Length != 0)
            {

                ListBoxEnumValues.Items.Add(TextBoxEnumValue.Text);
            }

        }

        private void DeleteEnumValue(object sender, RoutedEventArgs e)
        {
            if (ListBoxEnumValues.SelectedItem != null)
            {

                ListBoxEnumValues.Items.Remove(ListBoxEnumValues.SelectedItem);
            }

        }




        private void OnColumnSelect(object sender, System.EventArgs e)
        {
            manager.MakeConnection();
            DataTable dt = manager.GetDataFromQuerry($"SELECT DATA_TYPE FROM information_schema.COLUMNS WHERE TABLE_NAME = 'klienci' AND TABLE_SCHEMA ='{ConnectDataBase.DataBase}' AND COLUMN_NAME = '{ComboboxColumn.SelectedItem}'");
            DataView dv = dt.AsDataView();

            ComboBoxColumnType.SelectedItem = TypesOfColumn[dv[0][0].ToString()];
            ComboBoxColumnType.IsEnabled = true;
            CheckIfEnum();
            if (manager.CheckIfColumnHasAttributeNULL(ComboboxColumn.SelectedItem.ToString()))
            {
                CheckBoxIsNULL.IsChecked = true;
            }
            else CheckBoxIsNULL.IsChecked = false;


        }

        private void CheckIfEnumEvent(object sender, EventArgs e)
        {
            CheckIfEnum();
        }
    };
}

using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ProgramPraca
{

    public class ConnectDataBase
    {
        public static string connectionString;
        public static string Server { get; set; } = "";
        public static string DataBase { get; set; } = "";
        public static string Table { get; set; } = "";
        public static string User { get; set; } = "";
        public static string Password { get; set; } = "";
        public MySqlConnection Connection { get; set; }
        public static MySqlDataAdapter adapter { get; set; }

        public static void LoadSettings()
        {
            if (File.Exists("ConnectionSettings.txt"))
            {
                foreach (var line in File.ReadLines("ConnectionSettings.txt"))
                {
                    string[] splitted = line.Split(" ");
                    if (splitted[0] == "SERVER")
                    {
                        ConnectDataBase.Server = splitted[2];
                    }
                    if (splitted[0] == "USERNAME")
                    {
                        ConnectDataBase.User = splitted[2];
                    }
                    if (splitted[0] == "PASSWORD")
                    {
                        ConnectDataBase.Password = splitted[2];
                    }
                    if (splitted[0] == "DATABASE")
                    {
                        ConnectDataBase.DataBase = splitted[2];
                    }
                    if(splitted[0] == "TABLE")
                    {
                        ConnectDataBase.Table = splitted[2];
                    }
                    
                };

            }
            else
            {
                File.Create("ConnectionSettings.txt").Close();

            }
        }
        public void MakeConnection()
        {
            LoadSettings();
            connectionString = $"SERVER={Server};UID={User};PASSWORD={Password};DATABASE={DataBase};";
            Connection = new MySqlConnection(connectionString);
        }
        public DataTable GetDataFromQuerry(string Querry)
        {
            MySqlCommand cmd = new MySqlCommand(Querry, Connection);
            DataTable dt = new DataTable();
            try
            {
                
                Connection.Open();
            }
            catch(Exception ex){
                MessageBox.Show($"Złe dane łączenia z bazą! Sprawdź je i spróbuj ponownie.\n\n Error Message: \n{ex}");
                return null;
            };
            dt.Load(cmd.ExecuteReader());
            Connection.Close();
            return dt;
            
        }
        public void FillDataGrid(DataGrid dt, string filter = "")
        {
           var cmd = new MySqlCommand();
            cmd.CommandText = $"SELECT * FROM {Table};";
            cmd.Connection = Connection;
            
           
            try
            {
                adapter = new MySqlDataAdapter(cmd);
                DataTable dtable = new DataTable("Klienci");
                adapter.Fill(dtable);
                dt.ItemsSource = dtable.DefaultView;


            }
            catch(Exception e)
            {
                throw new Exception($"ERORR: {e.Message}");
            }
            
        }

        public bool CheckIfColumnHasAttributeNULL(string column)
        {
            MakeConnection();

            MySqlCommand cmd2 = Connection.CreateCommand();
            cmd2.CommandText = "select IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = @column;";
            cmd2.Parameters.AddWithValue("@column", column);
            cmd2.Parameters.AddWithValue("@table", "klienci");
            cmd2.Connection.Open();
            MySqlDataReader cmdResult = cmd2.ExecuteReader();
            bool isNull =false;
            while (cmdResult.Read())
            {

                if (cmdResult.GetString(0) == "YES")
                {
                    isNull = true;
                }
                else isNull = false;

            }
            cmd2.Connection.Close();
            return isNull;
        }
    }
}

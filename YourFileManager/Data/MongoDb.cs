using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProgramPraca
{

    public class MongoDb
    {
        public static MongoClient client { get; set; }
        public static IMongoDatabase Database { get; set; }

        public static string connectionString;
        public static string ServerName { get; set; } = "";
        public static string Port { get; set; } = "27017";
        public static string DataBaseName { get; set; } = "";
        public static string CollectionName { get; set; } = "";
        public static string UserName { get; set; } = "";
        public static string Password { get; set; } = "";

       
        //Data section
        public void InsertAdmin()
        {
            var db = client.GetDatabase("ProgramSaldo");

            var document = new BsonDocument { { "Login", "admin" }, { "Haslo", "123" }, { "Typ", "administrator" } };
            var userCollection = db.GetCollection<BsonDocument>("user");
            userCollection.InsertOne(document);

        }

        public static void FillDataGrid(DataGrid dt, FilterDefinition<BsonDocument> Filter = null)
        {
            var klienciCollection = Database.GetCollection<dynamic>(CollectionName);

            
            if(Filter == null)
            {
                FilterDefinition<dynamic> emptyFilter = Builders<dynamic>.Filter.Empty;
                var collectionData = klienciCollection.Find(emptyFilter).ToList();


                var dataTable = ToDataTable(collectionData.OrderByDescending(item => item.count) );
                
                dt.ItemsSource = dataTable.DefaultView;

            }
        }




        
        public static DataTable ToDataTable(IEnumerable<dynamic> items)
        {
           

            var data = items.ToArray();
            if (data.Count() == 0) return null;

            var dt = new DataTable();
            foreach (var pair in (IDictionary<string, object>)data[0])
            {
                dt.Columns.Add(pair.Key, pair.Value.GetType());
            }


            foreach (var pair in data)
            {
                var row = dt.NewRow();
                
                foreach (var para in (IDictionary<string, object>)pair)
                {
                    if (dt.Columns.Contains(para.Key) == false)
                    {
                        dt.Columns.Add(para.Key, para.Key.GetType());
                    }
                    row[para.Key] = para.Value;

                };
                dt.Rows.Add(row);
            }
            return dt;
        }


        

        public static bool changeCount(FilterDefinition<BsonDocument> filter, bool increaseOrDecrease, IMongoCollection<BsonDocument> collection)
        {
            var objectToPullCount = collection.Find(filter).FirstOrDefault();
            int count = 0;
            if (objectToPullCount == null) return false;
            count = (int)objectToPullCount["count"];
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("count", increaseOrDecrease ? count + 1 : count - 1);
            collection.UpdateOne(filter, update);
            return true;
        }

        //Settings section

        public static void MakeConnection()
        {
            LoadConnectionSettings();
            client = new MongoClient($"mongodb://{ServerName}:{Port}");
            Database = client.GetDatabase(DataBaseName);
        }
        public static void LoadConnectionSettings()
        {
            if (File.Exists("ConnectionSettings.txt"))
            {
                foreach (var line in File.ReadLines("ConnectionSettings.txt"))
                {
                    string[] splitted = line.Split(" ");
                    if (splitted[0] == "SERVER")
                    {
                        MongoDb.ServerName = splitted[2];
                    }
                    if (splitted[0] == "USERNAME")
                    {
                        MongoDb.UserName = splitted[2];
                    }
                    if (splitted[0] == "PASSWORD")
                    {
                        MongoDb.Password = splitted[2];
                    }
                    if (splitted[0] == "DATABASE")
                    {
                        MongoDb.DataBaseName = splitted[2];
                    }
                    if (splitted[0] == "TABLE")
                    {
                        MongoDb.CollectionName = splitted[2];
                    }

                };

            }
            else
            {
                File.Create("ConnectionSettings.txt").Close();

            }
        }
    }
}

using MongoDB.Bson;

using MongoDB.Driver;
using ProgramPraca.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ProgramPraca
{

    public class Mongo
    {
        public static MongoClient Client { get; set; }
        public static IMongoDatabase Database { get; set; }

        public static string ServerName { get; set; } = "";
        public static string Port { get; set; } = "27017";
        public static string DataBaseName { get; set; } = "";
        public static string UserName { get; set; } = "";
        public static string Password { get; set; } = "";
        public static string BackupPath { get; set; }

        //Data section
        

        public static void FillDataGrid(DateTime date, DataGrid dt, FilterDefinition<BsonDocument> Filter = null)
        {
            var klienciCollection = Database.GetCollection<dynamic>($"klienci-{date.Year}-{date.Month}");

            
            if(Filter == null)
            {
                FilterDefinition<dynamic> emptyFilter = Builders<dynamic>.Filter.Empty;
                var collectionData = klienciCollection.Find(emptyFilter).ToList();


                var dataTable = ToDataTable(collectionData.OrderByDescending(item => item.count) );
                if(dataTable is not null)
                {
                    dt.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    DataTable newData = new();
                    newData.Columns.Add("newColumn");
                   
                    dt.ItemsSource = newData.DefaultView;
                    
                }
                

            }
        }
        //Sekcja zarzadzania kolekcjami
        public static void CreateNewMonthCollection(DateTime previousDate, DateTime date)
        {

            var previousCollection = Mongo.Database.GetCollection<BsonDocument>($"klienci-{previousDate.Year}-{previousDate.Month}");
            var previousCollectionColumns = Mongo.Database.GetCollection<BsonDocument>($"columns-klienci-{previousDate.Year}-{previousDate.Month}");
            Mongo.Database.CreateCollection($"klienci-{date.Year}-{date.Month}");
            Mongo.Database.CreateCollection($"columns-klienci-{date.Year}-{date.Month}");
            var currentCollection = Mongo.Database.GetCollection<BsonDocument>($"klienci-{date.Year}-{date.Month}");
            var currentCollectionColumns = Mongo.Database.GetCollection<BsonDocument>($"columns-klienci--{date.Year}-{date.Month}");

            var rows = previousCollection.Find($"{{}}").ToList();
            var columns = previousCollectionColumns.Find($"{{}}").ToList();
            currentCollection.InsertMany(rows);
            currentCollectionColumns.InsertMany(columns);

        }
        public static bool CheckIfMonthCollectonExists(DateTime date)
        {

            return Mongo.Database.GetCollection<BsonDocument>($"klienci-{date.Year}-{date.Month}") == null ? true : false;
        }

        //
        static void MakeBackup()
        {
            if (Directory.Exists($"{BackupPath}/przywracanie"))
            {


                IMongoCollection<BsonDocument> collection = Database.GetCollection<BsonDocument>($"klienci-{DateTime.Now.Year}-{DateTime.Now.Month}");



                
                List<BsonDocument> clients = collection.Find($"{{}}").ToList();
                string path = $"{BackupPath}/przywracanie/{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}/kopiazapasowa-klienci-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}.txt";
                foreach (var doc in clients)
                {
                    File.AppendAllText(path, doc.ToJson());
                }

                collection = Database.GetCollection<BsonDocument>("backup");
                BsonDocument backup = new();
                backup.Add("Sciezka do pliku", path);
                backup.Add("Data dodania", DateTime.Now);

                collection.InsertOne(backup);



            }
            else
            {
                Directory.CreateDirectory($"{BackupPath}/przywracanie");
                MakeBackup();
            };

        }
        public static void CheckBackupDate()
        {
            dynamic backups = Mongo.Database.GetCollection<BsonDocument>("backup").Find($"{{}}").SortByDescending(x => x["Data dodania"]).FirstOrDefault();
            if(backups is null)
            {

                MakeBackup();
            }
            else
            {
                DateTime lastDate = (DateTime)backups["Data dodania"];

                if((DateTime.Now - lastDate.Date).TotalDays >= 1)
                {
                    MakeBackup();
                }
                
            }
        }

       

        public static DataTable ToDataTable(IEnumerable<dynamic> items)
        {
           

            var data = items.ToArray();
            if (data.Length == 0) return null;

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


        

        public static bool ChangeCount(FilterDefinition<BsonDocument> filter, bool increaseOrDecrease, IMongoCollection<BsonDocument> collection)
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
            Client = new MongoClient($"mongodb://{ServerName}:{Port}");
            Database = Client.GetDatabase(DataBaseName);
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
                        Mongo.ServerName = splitted[2];
                    }
                    if (splitted[0] == "DATABASE")
                    {
                        Mongo.DataBaseName = splitted[2];
                    }
                  
                    if (splitted[0] == "LOGS_PATH")
                    {
                        Logger.LogsPath = splitted[2];
                    }
                    if (splitted[0] == "BACKUP_PATH")
                    {
                        Mongo.BackupPath = splitted[2];
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

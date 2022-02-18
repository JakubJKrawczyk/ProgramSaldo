using MongoDB.Bson;
using MongoDB.Driver;
using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProgramPraca
{
    /// <summary>
    /// Logika interakcji dla klasy ColorsManager.xaml
    /// </summary>
    public partial class ColorsManager : Window
    {
        readonly IMongoCollection<BsonDocument> _collection;
        public ColorsManager()
        {
            InitializeComponent();
            var filter = new BsonDocument();
            filter.Add("name", "colors");
            var listOfCollectionNames = Mongo.Database.ListCollections(new ListCollectionsOptions { Filter = filter });

            
            if (listOfCollectionNames.Any())
            {
                _collection = Mongo.Database.GetCollection<BsonDocument>("colors");
                
                
                
            }
            else
            {
                Mongo.Database.CreateCollection("colors");
                _collection = Mongo.Database.GetCollection<BsonDocument>("colors");
                BsonDocument defaultColor = new();
                defaultColor.Add("name", "default");
                defaultColor.Add("value", "#ffffff");
                _collection.InsertOneAsync(defaultColor);
                
            }
            InitializeColors();
            FilterDefinition<BsonDocument> filterDefinition = Builders<BsonDocument>.Filter.Eq("name", ListBoxColors.Items[0].ToString());
            BsonDocument Color = _collection.Find(filterDefinition).FirstOrDefault();
            ColorPicker.Color = (Color)ColorConverter.ConvertFromString(Color["value"].ToString());
        }
        private void InitializeColors()
        {
            foreach (var color in _collection.Find($"{{}}").ToList())
            {
                ListBoxColors.Items.Add(color["name"]);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if(ListBoxColors.SelectedItem != null)
            {
                TextBoxColorName.Text = ListBoxColors.SelectedItem.ToString();
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("name", ListBoxColors.SelectedItem.ToString());
                BsonDocument color = _collection.Find(filter).SingleOrDefault();
                ColorPicker.Color = (Color)ColorConverter.ConvertFromString(color["value"].ToString());
            }
            
           
                
             
            
        }

        private void PlusPressed(object sender, RoutedEventArgs e)
        {
            
                BsonDocument newColor = new BsonDocument();
                newColor.Add("name", TextBoxColorName.Text);
                newColor.Add("value", ColorPicker.Color.ColorToHex());
                _collection.InsertOne(newColor);
            ListBoxColors.Items.Add(TextBoxColorName.Text);
            ColorPicker.Color = (Color)ColorConverter.ConvertFromString(newColor["value"].ToString());
            
        }

       

        private void MinusPressed(object sender, RoutedEventArgs e)
        {
           string colorName =  ListBoxColors.SelectedItem.ToString();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("name", colorName);
            _collection.DeleteOne(filter);
            ListBoxColors.Items.Remove(ListBoxColors.SelectedItem);
        }

       
    }
}

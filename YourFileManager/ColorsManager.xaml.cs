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
        IMongoCollection<BsonDocument> _collection;
        public ColorsManager()
        {
            InitializeComponent();
           _collection =  Mongo.Database.GetCollection<BsonDocument>("colors");
            if(_collection != null)
            {
                foreach(var color in _collection.Find($"{{}}").ToList())
                {
                    ListBoxColors.Items.Add(color["name"]);
                }
            }
            else
            {
                Mongo.Database.CreateCollection("colors");
                _collection = Mongo.Database.GetCollection<BsonDocument>("colors");

            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            TextBoxColorName.Text = ListBoxColors.SelectedItem.ToString();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("name", ListBoxColors.SelectedItem);
            BsonDocument color = _collection.Find(filter).SingleOrDefault();
            if(color != null)
            {
                string colorValue = color["value"].ToString();
                TextBoxColorProperty.Text = colorValue;
                TextBoxColorProperty.BorderBrush = (Brush)ColorConverter.ConvertFromString(colorValue);   
            }
        }

        private void PlusPressed(object sender, RoutedEventArgs e)
        {
            Regex rx = new Regex(@"^[1-9A-Za-z]{6}");
            if (rx.IsMatch(TextBoxColorProperty.Text))
            {
                BsonDocument newColor = new BsonDocument();
                newColor.Add("name", TextBoxColorName.Text.ToLower());
                newColor.Add("value", TextBoxColorProperty.Text.ToLower());
                _collection.InsertOne(newColor);
            }
            else
            {
                MessageBox.Show("Wartość Koloru musi wyglądać np tak #12A3A45");
            }
            
        }

       

        private void MinusPressed(object sender, RoutedEventArgs e)
        {
           string colorName =  ListBoxColors.SelectedItem.ToString().ToLower();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("name", colorName);
            _collection.DeleteOne(filter);
            ListBoxColors.SelectedItems.Remove(ListBoxColors.SelectedItem);
        }

        private void TextBoxColorProperty_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex rx = new Regex(@"^[1-9A-Fa-f]{3,6}");
            if (rx.IsMatch(TextBoxColorProperty.Text))
            {
                Brush color = (Brush)ColorConverter.ConvertFromString(TextBoxColorProperty.Text);
                TextBoxColorProperty.BorderBrush = color;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProgramPraca.PodOknaMain
{
    /// <summary>
    /// Logika interakcji dla klasy PopUpAskCreateCollection.xaml
    /// </summary>
    public partial class PopUpAskCreateCollection : Window
    {
        public DateTime PreviousDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime SelectedDate { get; set; }
        public Label DateLabel { get; set; }
        public PopUpAskCreateCollection(ref Label dateLabel, DateTime previous, DateTime current)
        {
            InitializeComponent();
            PreviousDate = previous;
            CurrentDate = current;
            DateLabel = dateLabel;
        }

        private void Yes(object sender, RoutedEventArgs e)
        {
            Mongo.CreateNewMonthCollection(PreviousDate, CurrentDate);
            Mongo.FillDataGrid(CurrentDate, Main.dt);
            Close();
        }

        private void No(object sender, RoutedEventArgs e)
        {
            Main.SelectedDate = PreviousDate;
            DateLabel.Content = $"{Main.months[PreviousDate.Month - 1]} {PreviousDate.Year}";

            Close();
        }
    }
}

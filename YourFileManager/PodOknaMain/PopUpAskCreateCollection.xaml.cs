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
        public Calendar Calendar { get; set; }
        public PopUpAskCreateCollection(Calendar calendar, DateTime previous, DateTime current)
        {
            InitializeComponent();
            PreviousDate = previous;
            CurrentDate = current;
            Calendar = calendar;
        }

        private void Yes(object sender, RoutedEventArgs e)
        {
            Mongo.CreateNewMonthCollection(PreviousDate, CurrentDate);
            Mongo.FillDataGrid(CurrentDate, Main.dt);
            Close();
        }

        private void No(object sender, RoutedEventArgs e)
        {
            Calendar.SelectedDate = PreviousDate;
            Calendar.DisplayDate = PreviousDate;
            Close();
        }
    }
}

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

namespace ProgramPraca
{
    /// <summary>
    /// Logika interakcji dla klasy ColumnManager.xaml
    /// </summary>
    public partial class ColumnManager : Window
    {
        public DateTime Date { get; set; }
        public ColumnManager(DateTime date)
        {
            InitializeComponent();
            Date = date;
        }

        private void AddColumn(object sender, RoutedEventArgs e)
        {
            PodOknaMain.AddColumn W = new PodOknaMain.AddColumn(Date);
            W.Show();
            Close();
        }
        private void ModifyColumn(object sender, RoutedEventArgs e)
        {
            PodOknaMain.ModifyColumn W = new();
            W.Show();
            Close();
        }

        private void DellColumn(object sender, RoutedEventArgs e)
        {
            PodOknaMain.DellColumn W = new();
            W.Show();
            Close();
        }
    }
}

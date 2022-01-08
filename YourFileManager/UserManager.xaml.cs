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
    /// Logika interakcji dla klasy UserManager.xaml
    /// </summary>
    public partial class UserManager : Window
    {
        public UserManager()
        {
            InitializeComponent();
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            PodOknaMain.AddUser w = new();
            w.Show();
        }

        private void DellUser(object sender, RoutedEventArgs e)
        {
            PodOknaMain.DellUser w = new();
            w.Show();
        }
    }
}

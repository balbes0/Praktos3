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
using Praktos3;
namespace Praktos3
{
    /// <summary>
    /// Логика взаимодействия для ListeningHistory.xaml
    /// </summary>
    public partial class ListeningHistory : Window
    {
        private List<string> listeningHistory;
        public ListeningHistory(List<string> history)
        {
            InitializeComponent();
            listeningHistory = history;
            ListeningHistory1.ItemsSource = listeningHistory;
        }
        private void DeleteHistory_Click(object sender, RoutedEventArgs e)
        {
            listeningHistory.Clear();
            ListeningHistory1.ItemsSource = null; // Очистить отображение списка
        }
    }
}

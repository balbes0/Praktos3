using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using MaterialDesignThemes.Wpf;
using Praktos3;
namespace Praktos3
{
    /// <summary>
    /// Логика взаимодействия для ListeningHistory.xaml
    /// </summary>
    public partial class ListeningHistory : Window
    {
        private List<string> listeningHistory;
        public event EventHandler<string> ItemSelected;
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

        private void ListeningHistory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = ListeningHistory1.SelectedItem.ToString();
            ItemSelected.Invoke(this, selectedItem);
            Close();
        }
    }
}

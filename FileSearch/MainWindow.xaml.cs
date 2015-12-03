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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            var engine = new SearchEngine(textBoxPath.Text, textBoxPattern.Text);
            SearchEngine.FoundFile += UpdateListBox;
            progressBarSearch.IsIndeterminate = true;
            listBoxSearchResults.ItemsSource = await engine.GetFiles();
            progressBarSearch.IsIndeterminate = false;
        }

        private void UpdateListBox()
        {
            Dispatcher.Invoke(() => listBoxSearchResults.Items.Refresh());
        }
    }
}

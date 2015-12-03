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
        SearchEngine engine;

        public MainWindow()
        {
            InitializeComponent();
            
            SearchEngine.FoundFile += UpdateListBox;
            SearchEngine.EndOfSearch += EndOfSearch;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null && b.Content.ToString() == "Cancel")
            {
                engine.Cancel();
                return;
            }

            engine = new SearchEngine(textBoxPath.Text, textBoxPattern.Text);

            progressBarSearch.IsIndeterminate = true;

            listBoxSearchResults.ItemsSource = engine.GetFiles();
            buttonSearch.Content = "Cancel";
        }

        private void UpdateListBox()
        {
            Dispatcher.Invoke(() => listBoxSearchResults.Items.Refresh());
        }

        private void EndOfSearch()
        {
            Dispatcher.Invoke(() => progressBarSearch.IsIndeterminate = false);
            Dispatcher.Invoke(() => buttonSearch.Content = "Search");
        }

        private void listBoxSearchResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBoxSearchResults.SelectedItem == null) return;

            string path = listBoxSearchResults.SelectedItem as string;
            if (path == null) return;

            NotepadProcess np = new NotepadProcess(path);
            np.StartProcess();
        }
    }
}

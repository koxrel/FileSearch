using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SearchEngine _engine;

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
                _engine.Cancel();
                return;
            }

            _engine = new SearchEngine(textBoxPath.Text, textBoxPattern.Text);

            listBoxSearchResults.Items.Clear();

            progressBarSearch.IsIndeterminate = true;

            _engine.GetFiles();

            buttonSearch.Content = "Cancel";
        }

        private void UpdateListBox(string newItem, double progress)
        {
            Dispatcher.Invoke(() => listBoxSearchResults.Items.Add(newItem));
            Dispatcher.Invoke(() => progressBarSearch.IsIndeterminate = false);
            Dispatcher.Invoke(() => progressBarSearch.Value = progress);
        }

        private void EndOfSearch()
        {
            Dispatcher.Invoke(() => progressBarSearch.Value = 100);
            Dispatcher.Invoke(() => buttonSearch.Content = "Search");
        }

        private void listBoxSearchResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string path = listBoxSearchResults.SelectedItem as string;
            if (path == null) return;

            NotepadProcess np = new NotepadProcess(path);
            np.StartProcess();
        }
    }
}

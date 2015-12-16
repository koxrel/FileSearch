using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SearchEngine _engine;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SearchEngine.FoundFile += UpdateListBox;
            SearchEngine.ReportProgress += UpdateProgress;
        }

        private async void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (buttonSearch.Content.ToString() == "Cancel")
            {
                _engine.Cancel();
                buttonSearch.IsEnabled = false;
                return;
            }

            _engine = new SearchEngine(textBoxPath.Text, textBoxPattern.Text);

            listBoxSearchResults.Items.Clear();

            progressBarSearch.IsIndeterminate = true;
            buttonSearch.Content = "Cancel";
            await _engine.GetFiles();

            buttonSearch.Content = "Search";
            buttonSearch.IsEnabled = true;
            progressBarSearch.IsIndeterminate = false;
        }

        private void UpdateListBox(string newItem)
        {
            Dispatcher.Invoke(() => listBoxSearchResults.Items.Add(newItem));
            Dispatcher.Invoke(() => progressBarSearch.IsIndeterminate = false);
        }

        private void UpdateProgress(double progress)
        {
            Dispatcher.Invoke(() => progressBarSearch.Value = progress);
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

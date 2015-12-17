using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
            SearchEngine.FoundFile += newFile => Dispatcher.Invoke(() => listBoxSearchResults.Items.Add(newFile));
            SearchEngine.ReportProgress += progress => Dispatcher.Invoke(() => progressBarSearch.Value = progress);
        }

        private async void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (buttonSearch.Content.ToString() == "Cancel")
            {
                _engine.Cancel();
                buttonSearch.IsEnabled = false;
                progressBarSearch.Foreground = Brushes.Gold;
                return;
            }

            _engine = new SearchEngine(textBoxPath.Text, textBoxPattern.Text);

            listBoxSearchResults.Items.Clear();
            buttonSearch.Content = "Cancel";
            progressBarSearch.Foreground = new SolidColorBrush(Color.FromRgb(6, 176, 37));

            await _engine.GetFiles();

            buttonSearch.Content = "Search";
            buttonSearch.IsEnabled = true;
        }

        private void listBoxSearchResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string path = listBoxSearchResults.SelectedItem as string;
            if (path == null) return;

            var fwp = new FileViewerProcess(path);
            fwp.StartProcess();
        }
    }
}

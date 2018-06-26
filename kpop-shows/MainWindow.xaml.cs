using kpop_shows.Helpers;
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

namespace kpop_shows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BackgroundParser BackgroundParser { get; set; } = new BackgroundParser();

        public MainWindow()
        {
            InitializeComponent();
            BackgroundParser.Start(Dispatcher);
            DataContext = this;
        }

        private void ShowDate_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ShowDate_CanExecute(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = e.Parameter is MusicShowInstance;
    }

    public static class MainWindowCommands
    {
        public static readonly RoutedUICommand ShowDate = new RoutedUICommand("ShowDate", "ShowDate", typeof(MainWindowCommands));
    }
}

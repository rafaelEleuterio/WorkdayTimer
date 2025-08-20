using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkdayTimerDesktopApp.ViewModels;

namespace WorkdayTimerDesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TimerViewModel TimerViewModel { get; set; }
        public MainWindow()
        {
            TimerViewModel = TimerViewModel.GetInstance();
            DataContext = TimerViewModel;
            TimerViewModel.ChangeColor(0);
            InitializeComponent();
        }
        public MainWindow(TimerViewModel timerViewModel)
        {
            TimerViewModel = timerViewModel;
            DataContext = TimerViewModel;
            TimerViewModel.ChangeColor(0);
            InitializeComponent();
        }
    }
}
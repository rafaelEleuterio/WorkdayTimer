using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkdayTimerDesktopApp.Commands;
using WorkdayTimerDesktopApp.Stores;

namespace WorkdayTimerDesktopApp.ViewModels;
public class TimerViewModel : ViewModelBase, IDisposable
{
    private static TimerViewModel _instance = new TimerViewModel();
    private readonly PaletteHelper _paletteHelper = new();
    private readonly TimerStore _timerStore;
    private TimeSpan _timeRunned = new TimeSpan(0, 0, 0);
    private bool _buttonStartEnabled = true;
    private bool _buttonStopEnabled = false;
    private bool _buttonResumeEnabled = false;
    private bool _buttonCoffeeBreakEnabled = false;
    private bool _buttonNonCoffeeBreakEnabled = false;
    public TimeSpan TimeRunned
    {
        get => _timeRunned;
        set
        {
            _timeRunned = value;
            OnPropertyChanged(nameof(TimeRunned));
        }
    }
    public bool ButtonStartEnabled
    {
        get => _buttonStartEnabled;
        set
        {
            _buttonStartEnabled = value;
            OnPropertyChanged(nameof(ButtonStartEnabled));
        }
    }
    public bool ButtonStopEnabled
    {
        get => _buttonStopEnabled;
        set
        {
            _buttonStopEnabled = value;
            OnPropertyChanged(nameof(ButtonStopEnabled));
        }
    }
    public bool ButtonResumeEnabled
    {
        get => _buttonResumeEnabled;
        set
        {
            _buttonResumeEnabled = value;
            OnPropertyChanged(nameof(ButtonResumeEnabled));
        }
    }
    public bool ButtonCoffeeBreakEnabled
    {
        get => _buttonCoffeeBreakEnabled;
        set
        {
            _buttonCoffeeBreakEnabled = value;
            OnPropertyChanged(nameof(ButtonCoffeeBreakEnabled));
        }
    }
    public bool ButtonNonCoffeeBreakEnabled
    {
        get => _buttonNonCoffeeBreakEnabled;
        set
        {
            _buttonNonCoffeeBreakEnabled = value;
            OnPropertyChanged(nameof(ButtonNonCoffeeBreakEnabled));
        }
    }

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ResumeCommand { get; }
    public ICommand CoffeeBreakCommand { get; }
    public ICommand NonCoffeeBreakCommand { get; }
    public ICommand ExitCommand { get; }

    public TimerViewModel(TimerStore timerStore)
    {
        _timerStore = timerStore;
        StartCommand = new StartCommand(this, _timerStore);
        StopCommand = new StopCommand(this, _timerStore);
        ResumeCommand = new ResumeCommand(this, _timerStore);
        CoffeeBreakCommand = new CoffeeBreakCommand(this, _timerStore);
        NonCoffeeBreakCommand = new NonCoffeeBreakCommand(this, _timerStore);
        ExitCommand = new ExitCommand(this, _timerStore);

        _timerStore.TotalTimeElapsedChanged += TimerStore_TimeRunnedChanged;
    }

    public TimerViewModel()
    {
        _timerStore = new TimerStore();
        StartCommand = new StartCommand(this, _timerStore);
        StopCommand = new StopCommand(this, _timerStore);
        ResumeCommand = new ResumeCommand(this, _timerStore);
        CoffeeBreakCommand = new CoffeeBreakCommand(this, _timerStore);
        NonCoffeeBreakCommand = new NonCoffeeBreakCommand(this, _timerStore);
        ExitCommand = new ExitCommand(this, _timerStore);

        _timerStore.TotalTimeElapsedChanged += TimerStore_TimeRunnedChanged;
    }
    public static TimerViewModel GetInstance()
    {
        return _instance;
    }

    private void TimerStore_TimeRunnedChanged()
    {
        TimeRunned = _timerStore.GetTimeRunned();
        var task = Task.Run(async () => await ((App)App.Current).UpdateTimeRunned(TimeRunned));
        ChangeColor(TimeRunned.TotalSeconds);
        OnPropertyChanged(nameof(TimeRunned));
    }

    public void ChangeColor(double totalSeconds)
    {
        byte fracaoDaCor = (((int)(totalSeconds / 112)) > 255) ? (byte)0 : (byte)(255 - ((int)(totalSeconds / 112)));
        var theme = _paletteHelper.GetTheme();
        theme.SetPrimaryColor(System.Windows.Media.Color.FromArgb(255, fracaoDaCor, 255, fracaoDaCor));
        _paletteHelper.SetTheme(theme);
    }

    public void Dispose()
    {
        _timerStore.TotalTimeElapsedChanged -= TimerStore_TimeRunnedChanged;
    }
}

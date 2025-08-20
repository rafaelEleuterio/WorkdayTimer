using System.Timers;
using WorkdayTimerDesktopApp.Services;
using Forms = System.Windows.Forms;

namespace WorkdayTimerDesktopApp.Stores;
public class TimerStore : IDisposable
{
    private static TimerStore instance = new TimerStore();
    private readonly INotificationService _notificationService;
    private readonly System.Timers.Timer _timer;

    private DateTime _lastTimePaused;
    private TimeSpan _timeRunned;
    private bool _wasRunning;
    public bool IsOnCoffeeBreak { get; set; } = false;
    public bool IsOnNonCoffeeBreak { get; set; } = false;
    public bool HasPaused { get; set; } = false;
    public bool HasResumed { get; set; } = false;
    public bool HasStopped { get; set; } = false;
    public bool HasStarted { get; set; } = false;
    public bool WasOnCoffeeBreak { get; set; } = false;
    public bool WasOnNonCoffeeBreak { get; set; } = false;

    public Dictionary<DateTime, TimeSpan> DictionaryOfTimesPaused { get; set; } = new Dictionary<DateTime, TimeSpan>();

    public bool IsRunning;

    public event Action TotalTimeElapsedChanged;

    public TimerStore()
    {
        _timer = new System.Timers.Timer(333);
        _timer.Elapsed += Timer_Elapsed;
    }
    public TimerStore(INotificationService notificationService)
    {
        _notificationService = notificationService;
        _notificationService.NotificationAccepted += NotificationService_NotificationAccepted;

        _timer = new System.Timers.Timer(333);
        _timer.Elapsed += Timer_Elapsed;
    }
    public static TimerStore GetInstance()
    {
        return instance;
    }

    public void Start()
    {
        IsOnCoffeeBreak = false;
        IsOnNonCoffeeBreak = false;
        HasPaused = false;
        HasResumed = false;
        HasStopped = false;
        HasStarted = true;
        WasOnCoffeeBreak = false;
        WasOnNonCoffeeBreak = false;
        IsRunning = true;

        DictionaryOfTimesPaused = new Dictionary<DateTime, TimeSpan>();
        _timeRunned = TimeSpan.Zero;
        _lastTimePaused = DateTime.Now;
        DictionaryOfTimesPaused.Add(_lastTimePaused, new TimeSpan(0, 0, 0));

        _timer.Start();

        OnTotalTimeElapsedChanged();
    }

    public void Stop()
    {
        IsOnCoffeeBreak = false;
        IsOnNonCoffeeBreak = false;
        HasPaused = false;
        HasResumed = false;
        HasStopped = true;
        HasStarted = true;
        IsRunning = false;
        WasOnCoffeeBreak = false;
        WasOnNonCoffeeBreak = false;

        TimePaused();

        _timer.Stop();

        OnTotalTimeElapsedChanged();
    }

    public void Resume()
    {
        HasPaused = false;
        HasResumed = true;
        HasStopped = false;
        HasStarted = false;
        IsRunning = true;

        TimeResumed(WasOnCoffeeBreak ? TypeOfBreak.CoffeeBreak : TypeOfBreak.NonCoffeeBreak);

        _timer.Start();

        OnTotalTimeElapsedChanged();
    }

    public void CoffeeBreak()
    {
        IsOnCoffeeBreak = true;
        IsOnNonCoffeeBreak = false;
        HasPaused = true;
        HasResumed = false;
        HasStopped = false;
        HasStarted = true;
        WasOnCoffeeBreak = true;
        WasOnNonCoffeeBreak = false;
        IsRunning = true;

        TimePaused();

        _timer.Start();

        OnTotalTimeElapsedChanged();
    }

    public void NonCoffeeBreak()
    {
        IsOnCoffeeBreak = false;
        IsOnNonCoffeeBreak = true;
        HasPaused = true;
        HasResumed = false;
        HasStopped = false;
        HasStarted = true;
        WasOnCoffeeBreak = false;
        WasOnNonCoffeeBreak = true;
        IsRunning = false;

        TimePaused();

        _timer.Stop();

        OnTotalTimeElapsedChanged();
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _timeRunned = GetTimeRunned();
        OnTotalTimeElapsedChanged();

        if (_wasRunning && !IsRunning)
        {
            _notificationService.Notify("WorkTimer!", "Você já trabalhou 8 horas hoje. Clique aqui para encerrar o dia!",
                3000, NotificationType.StopTimer, Forms.ToolTipIcon.Info);
        }

        _wasRunning = IsRunning;
    }

    private void NotificationService_NotificationAccepted(NotificationType lastNotificationType)
    {
        if (lastNotificationType == NotificationType.StopTimer)
        {
            Stop();
        }
        else if (lastNotificationType == NotificationType.PauseOnCoffeeBreakTimer)
        {
            CoffeeBreak();
        }
        else if (lastNotificationType == NotificationType.PauseNonOnCoffeeBreakTimer)
        {
            NonCoffeeBreak();
        }
    }

    public TimeSpan GetTimeRunned()
    {
        if (HasResumed && WasOnCoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = GetTimeRunnedOnThisBreak();
        }
        else if (IsOnCoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = GetTimeRunnedOnThisBreak();
        }
        else if (HasStopped && WasOnCoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = GetTimeRunnedOnThisBreak();
        }
        else if (HasResumed && WasOnNonCoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = GetTimeRunnedOnThisBreak();
        }
        else if (IsOnNonCoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = TimeSpan.Zero;
        }
        else if (HasStopped && WasOnNonCoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = TimeSpan.Zero;
        }
        else if (IsRunning)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = GetTimeRunnedOnThisBreak();
        }
        else if (HasStopped)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = TimeSpan.Zero;
        }
        else
        {
            throw new ArgumentException("Uma situação não prevista aconteceu!");
        }

        _timeRunned = new TimeSpan(0, 0, DictionaryOfTimesPaused.Values.Select(c => c.TotalSeconds).ToList().Sum(c => (int)c));
        return _timeRunned;
    }

    public TimeSpan GetTimeRunnedOnThisBreak()
    {
        TimeSpan timeRunnedUntilThisBreak = DateTime.Now - _lastTimePaused;

        return timeRunnedUntilThisBreak;
    }

    public void TimePaused()
    {
        _lastTimePaused = DateTime.Now;
        DictionaryOfTimesPaused.Add(_lastTimePaused, new TimeSpan(0, 0, 0));

        return;
    }

    public void TimeResumed(TypeOfBreak typeOfBreak)
    {
        if (typeOfBreak == TypeOfBreak.CoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = GetTimeRunnedOnThisBreak();
        }
        else if (typeOfBreak == TypeOfBreak.NonCoffeeBreak)
        {
            DictionaryOfTimesPaused[_lastTimePaused] = new TimeSpan(0, 0, 0);
        }

        _lastTimePaused = DateTime.Now;
        DictionaryOfTimesPaused.Add(_lastTimePaused, new TimeSpan(0, 0, 0));
        return;
    }

    private void OnTotalTimeElapsedChanged()
    {
        TotalTimeElapsedChanged?.Invoke();
    }

    public void Dispose()
    {
        if (_notificationService is not null)
        {
            _notificationService.NotificationAccepted -= NotificationService_NotificationAccepted;
        }
        _timer.Dispose();
    }
}
public enum TypeOfBreak
{
    CoffeeBreak,
    NonCoffeeBreak
}
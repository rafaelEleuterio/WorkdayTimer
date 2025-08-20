using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkdayTimerDesktopApp.Services;
public enum NotificationType
{
    TimerComplete,
    StopTimer,
    PauseOnCoffeeBreakTimer,
    PauseNonOnCoffeeBreakTimer
}

public interface INotificationService
{
    event Action<NotificationType> NotificationAccepted;

    void Notify(string title, string message, int timeout, NotificationType notificationType, ToolTipIcon icon);
}

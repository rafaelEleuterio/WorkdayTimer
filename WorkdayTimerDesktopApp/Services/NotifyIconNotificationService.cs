using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkdayTimerDesktopApp.Services;
public class NotifyIconNotificationService : INotificationService
{
    private readonly NotifyIcon _notifyIcon;

    private NotificationType _lastNotificationType;

    public event Action<NotificationType> NotificationAccepted;

    public NotifyIconNotificationService(NotifyIcon notifyIcon)
    {
        _notifyIcon = notifyIcon;

        _notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;
    }

    public void Notify(string title, string message, int timeout, NotificationType notificationType, ToolTipIcon icon)
    {
        _lastNotificationType = notificationType;
        _notifyIcon.ShowBalloonTip(timeout, title, message, icon);
    }

    private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
    {
        NotificationAccepted?.Invoke(_lastNotificationType);
    }
}

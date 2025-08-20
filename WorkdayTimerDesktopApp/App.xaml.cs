
using System.Windows;
using Forms = System.Windows.Forms;
using System.Drawing;
using WorkdayTimerDesktopApp.Stores;
using WorkdayTimerDesktopApp.ViewModels;
using WorkdayTimerDesktopApp.Commands;

namespace WorkdayTimerDesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        private TimerStore _timerStore;
        private TimerViewModel _timerViewModel;
        private static App _instance = new App();

        public static App GetInstance()
        {
            return _instance;
        }

        public App()
        {
            _timerStore = TimerStore.GetInstance();
            _timerViewModel = TimerViewModel.GetInstance();

            _notifyIcon = new Forms.NotifyIcon();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _notifyIcon.Visible = true;
            _notifyIcon.Icon = new Icon("Resources/timer-alert-green-gradient.ico");
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.MouseDoubleClick += NotifyIcon_DoubleClick;

            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();

            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripLabel($"Você está trabalhando há 00 horas 00 minutos e 00 segundos!"));
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripButton("Começar a trampar!",
                Image.FromFile("Resources/timer.jpg"),
                Start_Click));
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripButton("Encerrar o dia!",
                Image.FromFile("Resources/timer-stop.jpg"),
                Stop_Click));
            DisableButton(StopCommand.NotifyIconIndex);
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripButton("Voltar a trampar!",
                Image.FromFile("Resources/timer-play.jpg"),
                Resume_Click));
            DisableButton(ResumeCommand.NotifyIconIndex);
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripButton("Pausa remunerada",
                Image.FromFile("Resources/coffee.jpg"),
                CoffeeBreak_Click));
            DisableButton(CoffeeBreakCommand.NotifyIconIndex);
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripButton("Pausa não remunerada",
                Image.FromFile("Resources/coffee-off.jpg"),
                NonCoffeeBreak_Click));
            DisableButton(NonCoffeeBreakCommand.NotifyIconIndex);
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripButton("Fechar aplicativo!",
                Image.FromFile("Resources/exit-run.jpg"),
                CloseApp));

            base.OnStartup(e);
        }

        private void NonCoffeeBreak_Click(object? sender, EventArgs e)
        {
            _timerViewModel.NonCoffeeBreakCommand.Execute(null);
        }

        private void CoffeeBreak_Click(object? sender, EventArgs e)
        {
            _timerViewModel.CoffeeBreakCommand.Execute(null);
        }

        private void Resume_Click(object? sender, EventArgs e)
        {
            _timerViewModel.ResumeCommand.Execute(null);
        }

        private void Stop_Click(object? sender, EventArgs e)
        {
            _timerViewModel.StopCommand.Execute(null);
        }

        private void Start_Click(object? sender, EventArgs e)
        {
            _timerViewModel.StartCommand.Execute(null);
        }

        private void NotifyIcon_MouseClick(object? sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                ShowMainWindow();
                return;
            }
        }

        private void NotifyIcon_BackColorChanged(object? sender, EventArgs e)
        {
            foreach (Forms.ToolStripItem item in _notifyIcon.ContextMenuStrip.Items)
            {

                if (item.Selected)
                {
                    item.BackColor = Color.FromArgb(255, 63, 64, 66);
                }

                if (item is Forms.ToolStripButton)
                {
                    var button = (Forms.ToolStripButton)item;
                    button.BackColor = Color.FromArgb(255, 63, 64, 66);
                }
            }
        }

        public async Task UpdateTimeRunned(TimeSpan timeRunned)
        {
            _notifyIcon.ContextMenuStrip.Items[0].Text = $"Você está trabalhando há {timeRunned.Hours.ToString("00")} horas, " +
                $"{timeRunned.Minutes.ToString("00")} minutos " +
                $"e {timeRunned.Seconds.ToString("00")} segundos!";
        }

        public void DisableButton(int index)
        {
            _notifyIcon.ContextMenuStrip.Items[index].Enabled = false;
        }

        public void EnableButton(int index)
        {
            _notifyIcon.ContextMenuStrip.Items[index].Enabled = true;
        }

        private void ShowMainWindow()
        {
            if (MainWindow is null)
            {
                MainWindow = new MainWindow();
                MainWindow.Show();
            }

            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }

        private void CloseApp(object? sender, EventArgs e)
        {
            _timerViewModel.Dispose();
            _timerStore.Dispose();
            MainWindow?.Close();
            this.Shutdown();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();

            base.OnExit(e);
        }

        public void Exit()
        {
            _notifyIcon.Dispose();
            if (MainWindow is not null)
            {
                MainWindow.Close();
            }
            this.Shutdown();
        }
    }

}

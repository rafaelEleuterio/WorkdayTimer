using WorkdayTimerDesktopApp.Stores;
using WorkdayTimerDesktopApp.ViewModels;

namespace WorkdayTimerDesktopApp.Commands;
public class StartCommand : BaseCommand
{
    private readonly TimerViewModel _viewModel;
    private readonly TimerStore _timerStore;
    public static int NotifyIconIndex { get; set; } = 2;

    public StartCommand(TimerViewModel viewModel, TimerStore timerStore)
    {
        _viewModel = viewModel;
        _timerStore = timerStore;
    }

    public override void Execute(object parameter)
    {
        _viewModel.ButtonStartEnabled = false;
        ((App)App.Current).DisableButton(NotifyIconIndex);
        _viewModel.ButtonStopEnabled = true;
        ((App)App.Current).EnableButton(StopCommand.NotifyIconIndex);
        _viewModel.ButtonResumeEnabled = false;
        ((App)App.Current).DisableButton(ResumeCommand.NotifyIconIndex);
        _viewModel.ButtonCoffeeBreakEnabled = true;
        ((App)App.Current).EnableButton(CoffeeBreakCommand.NotifyIconIndex);
        _viewModel.ButtonNonCoffeeBreakEnabled = true;
        ((App)App.Current).EnableButton(NonCoffeeBreakCommand.NotifyIconIndex);

        _timerStore.Start();
    }
}

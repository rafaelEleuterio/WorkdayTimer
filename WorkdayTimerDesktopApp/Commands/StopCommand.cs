using WorkdayTimerDesktopApp.Stores;
using WorkdayTimerDesktopApp.ViewModels;

namespace WorkdayTimerDesktopApp.Commands;
public class StopCommand : BaseCommand
{
    private readonly TimerViewModel _viewModel;
    private readonly TimerStore _timerStore;
    public static int NotifyIconIndex { get; set; } = 3;


    public StopCommand(TimerViewModel viewModel, TimerStore timerStore)
    {
        _viewModel = viewModel;
        _timerStore = timerStore;
    }

    public override void Execute(object parameter)
    {
        _viewModel.ButtonStartEnabled = true;
        ((App)App.Current).EnableButton(StartCommand.NotifyIconIndex);
        _viewModel.ButtonStopEnabled = false;
        ((App)App.Current).DisableButton(NotifyIconIndex);
        _viewModel.ButtonResumeEnabled = false;
        ((App)App.Current).DisableButton(ResumeCommand.NotifyIconIndex);
        _viewModel.ButtonCoffeeBreakEnabled = false;
        ((App)App.Current).DisableButton(CoffeeBreakCommand.NotifyIconIndex);
        _viewModel.ButtonNonCoffeeBreakEnabled = false;
        ((App)App.Current).DisableButton(NonCoffeeBreakCommand.NotifyIconIndex);

        _timerStore.Stop();
    }
}

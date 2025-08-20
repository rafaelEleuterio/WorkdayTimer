using WorkdayTimerDesktopApp.Stores;
using WorkdayTimerDesktopApp.ViewModels;

namespace WorkdayTimerDesktopApp.Commands;
public class NonCoffeeBreakCommand : BaseCommand
{
    private readonly TimerViewModel _viewModel;
    private readonly TimerStore _timerStore;
    public static int NotifyIconIndex { get; set; } = 6;


    public NonCoffeeBreakCommand(TimerViewModel viewModel, TimerStore timerStore)
    {
        _viewModel = viewModel;
        _timerStore = timerStore;
    }

    public override void Execute(object parameter)
    {
        _viewModel.ButtonStartEnabled = false;
        ((App)App.Current).DisableButton(StartCommand.NotifyIconIndex);
        _viewModel.ButtonStopEnabled = true;
        ((App)App.Current).EnableButton(StopCommand.NotifyIconIndex);
        _viewModel.ButtonResumeEnabled = true;
        ((App)App.Current).EnableButton(ResumeCommand.NotifyIconIndex);
        _viewModel.ButtonCoffeeBreakEnabled = true;
        ((App)App.Current).EnableButton(CoffeeBreakCommand.NotifyIconIndex);
        _viewModel.ButtonNonCoffeeBreakEnabled = false;
        ((App)App.Current).DisableButton(NotifyIconIndex);

        _timerStore.NonCoffeeBreak();
    }
}

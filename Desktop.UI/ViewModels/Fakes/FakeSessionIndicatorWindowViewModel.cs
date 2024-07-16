namespace Immense.RemoteControl.Desktop.UI.ViewModels.Fakes;
internal class FakeSessionIndicatorWindowViewModel : FakeBrandedViewModelBase, ISessionIndicatorWindowViewModel
{
    public Task PromptForExit()
    {
        return Task.CompletedTask;
    }
}

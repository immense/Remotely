using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Diagnostics;
using Immense.RemoteControl.Desktop.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Immense.RemoteControl.Desktop.UI.Controls.Dialogs;

public partial class MessageBox : Window
{
    public MessageBox()
    {
        InitializeComponent();
    }

    public static async Task<MessageBoxResult> Show(string message, string caption, MessageBoxType type)
    {
        Guard.IsNotNull(StaticServiceProvider.Instance, nameof(StaticServiceProvider.Instance));

        var dispatcher = StaticServiceProvider.Instance.GetRequiredService<IUiDispatcher>();

        return await dispatcher.InvokeAsync(async () =>
        {
            var viewModel = StaticServiceProvider.Instance.GetRequiredService<IMessageBoxViewModel>();
            var messageBox = new MessageBox()
            {
                DataContext = viewModel
            };
            viewModel.Caption = caption;
            viewModel.Message = message;

            switch (type)
            {
                case MessageBoxType.OK:
                    viewModel.IsOkButtonVisible = true;
                    break;
                case MessageBoxType.YesNo:
                    viewModel.AreYesNoButtonsVisible = true;
                    break;
                default:
                    break;
            }

            await dispatcher.ShowDialog(messageBox);

            return viewModel.Result;
        });
    }
}
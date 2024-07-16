using Remotely.Desktop.UI.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.UI.Services;

public interface IDialogProvider
{
    Task<MessageBoxResult> Show(string message, string caption, MessageBoxType type);
}

internal class DialogProvider : IDialogProvider
{
    public async Task<MessageBoxResult> Show(string message, string caption, MessageBoxType type)
    {
        return await MessageBox.Show(message, caption, type);
    }
}

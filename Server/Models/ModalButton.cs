using System;

namespace Remotely.Server.Models;

public class ModalButton
{
    public string Class { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;

    public required Action OnClick { get; init; }
}

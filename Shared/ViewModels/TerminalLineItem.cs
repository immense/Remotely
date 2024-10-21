namespace Remotely.Shared.ViewModels;

public class TerminalLineItem
{
    public Guid Id { get; } = Guid.NewGuid();
    public string? Text { get; set; }
    public string? ClassName { get; set; }
    public string? Title { get; set; }
}

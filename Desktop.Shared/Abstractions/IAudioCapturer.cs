namespace Immense.RemoteControl.Desktop.Shared.Abstractions;

public interface IAudioCapturer
{
    event EventHandler<byte[]> AudioSampleReady;
    void ToggleAudio(bool toggleOn);
}

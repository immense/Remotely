using Remotely.Desktop.Shared.Abstractions;
using NAudio.Wave;
using System.IO;

namespace Remotely.Desktop.Win.Services;

public class AudioCapturerWin : IAudioCapturer
{
    private readonly ILogger<AudioCapturerWin> _logger;
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private WasapiLoopbackCapture? _capturer;
    private WaveFormat? _targetFormat;
    public AudioCapturerWin(ILogger<AudioCapturerWin> logger)
    {
        _logger = logger;
    }

    public event EventHandler<byte[]>? AudioSampleReady;

    public void ToggleAudio(bool toggleOn)
    {
        if (toggleOn)
        {
            Start();
        }
        else
        {
            Stop();
        }
    }

    private async void Capturer_DataAvailable(object? sender, WaveInEventArgs args)
    {
        if (args.Buffer.All(x => x == 0))
        {
            return;
        }

        try
        {
            await _sendLock.WaitAsync();

            if (args.BytesRecorded > 0)
            {
                await SendTempBuffer(args.Buffer);
            }
        }
        catch { }
        finally
        {
            _sendLock.Release();
        }
    }

    private async Task SendTempBuffer(byte[] buffer)
    {
        if (_capturer is null)
        {
            _logger.LogWarning("Audio capturer is unexpectedly null.");
            return;
        }

        using var ms1 = new MemoryStream();
        using (var wfw = new WaveFileWriter(ms1, _capturer.WaveFormat))
        {
            await wfw.WriteAsync(buffer);
        }

        // Resample to 16-bit.
        using var ms2 = new MemoryStream(ms1.ToArray());
        using var wfr = new WaveFileReader(ms2);
        using var ms3 = new MemoryStream();
        using (var resampler = new MediaFoundationResampler(wfr, _targetFormat))
        {
            WaveFileWriter.WriteWavFileToStream(ms3, resampler);
        }
        AudioSampleReady?.Invoke(this, ms3.ToArray());
    }

    private void Start()
    {
        try
        {
            _capturer?.Dispose();
            _capturer = new WasapiLoopbackCapture();
            _targetFormat ??= new WaveFormat(16000, 8, 1);
            _capturer.DataAvailable += Capturer_DataAvailable;

            _capturer.StartRecording();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating audio capturer.  Make sure a sound device is installed and working.");
        }
    }

    private void Stop()
    {
        _capturer?.StopRecording();
    }
}

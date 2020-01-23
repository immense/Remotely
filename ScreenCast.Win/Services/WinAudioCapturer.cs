using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Interfaces;

namespace Remotely.ScreenCast.Win.Services
{
    public class WinAudioCapturer : IAudioCapturer
    {
        private WasapiLoopbackCapture Capturer { get; set; }
        private Stopwatch SendTimer { get; set; }
        private WaveFormat TargetFormat { get; set; }
        private List<byte> TempBuffer { get; set; } = new List<byte>();
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

        private async void SendTempBuffer()
        {
            if (TempBuffer.Count == 0)
            {
                return;
            }

            using (var ms1 = new MemoryStream())
            {
                using (var wfw = new WaveFileWriter(ms1, Capturer.WaveFormat))
                {
                    wfw.Write(TempBuffer.ToArray(), 0, TempBuffer.Count);
                }
                TempBuffer.Clear();

                // Resample to 16-bit so Firefox will play it.
                using (var ms2 = new MemoryStream(ms1.ToArray()))
                using (var wfr = new WaveFileReader(ms2))
                using (var ms3 = new MemoryStream())
                {
                    using (var resampler = new MediaFoundationResampler(wfr, TargetFormat))
                    {
                        WaveFileWriter.WriteWavFileToStream(ms3, resampler);
                    }
                    var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
                    await conductor.CasterSocket.SendAudioSample(ms3.ToArray(), Program.Conductor.Viewers.Keys.ToList());
                }
            }
        }

        private void Start()
        {
            try
            {
                Capturer = new WasapiLoopbackCapture();
                TargetFormat = new WaveFormat(16000, 8, 1);
                SendTimer = Stopwatch.StartNew();
                Capturer.DataAvailable += (aud, args) =>
                {
                    try
                    {
                        if (args.BytesRecorded > 0)
                        {
                            lock (TempBuffer)
                            {
                                if (!SendTimer.IsRunning)
                                {
                                    SendTimer.Restart();
                                }
                                TempBuffer.AddRange(args.Buffer.Take(args.BytesRecorded));
                                if (TempBuffer.Count > 200000)
                                {
                                    SendTimer.Reset();
                                    SendTempBuffer();
                                }
                                else if (SendTimer.Elapsed.TotalMilliseconds > 1000)
                                {
                                    SendTimer.Reset();
                                    SendTempBuffer();
                                }
                            }
                        }
                    }
                    catch { }
                };
                Capturer.StartRecording();
            }
            catch { }
        }
        private void Stop()
        {
            Capturer.StopRecording();
            SendTimer.Reset();
        }
    }
}

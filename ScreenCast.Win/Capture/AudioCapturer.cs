using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Remotely.ScreenCast.Core;

namespace Remotely.ScreenCast.Win.Capture
{
    public class AudioCapturer
    {
        public AudioCapturer(Conductor conductor)
        {
            Conductor = conductor;
        }
        private WasapiLoopbackCapture Capturer { get; set; }
        private WaveFormat TargetFormat { get; set; }
        private Conductor Conductor { get; set; }

        public void Start()
        {
            try
            {
                Capturer = new WasapiLoopbackCapture();
                TargetFormat = new WaveFormat(16000, 8, 1);
                Capturer.DataAvailable += async (aud, args) =>
                {
                    try
                    {
                        if (args.BytesRecorded > 0)
                        {
                            using (var ms1 = new MemoryStream())
                            {
                                using (var wfw = new WaveFileWriter(ms1, Capturer.WaveFormat))
                                {
                                    wfw.Write(args.Buffer, 0, args.BytesRecorded);
                                }
                                // Resample to 16-bit so Firefox will play it.
                                using (var ms2 = new MemoryStream(ms1.ToArray()))
                                using (var wfr = new WaveFileReader(ms2))
                                using (var ms3 = new MemoryStream())
                                {
                                    using (var resampler = new MediaFoundationResampler(wfr, TargetFormat))
                                    {
                                        WaveFileWriter.WriteWavFileToStream(ms3, resampler);
                                    }
                                    await Conductor.CasterSocket.SendAudioSample(ms3.ToArray(), Conductor.Viewers.Keys.ToList());
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
    }
}

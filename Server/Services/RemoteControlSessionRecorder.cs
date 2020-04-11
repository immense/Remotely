using Microsoft.AspNetCore.Hosting;
using Remotely.Server.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class RemoteControlSessionRecorder
    {
        public RemoteControlSessionRecorder(IWebHostEnvironment hostingEnv, DataService dataService)
        {
            HostingEnv = hostingEnv;
            DataService = dataService;
        }

        private static ConcurrentQueue<RemoteControlFrame> FrameQueue { get; } = new ConcurrentQueue<RemoteControlFrame>();
        private static object LockObject { get; } = new object();
        private static Task ProcessingTask { get; set; }
        private static ConcurrentDictionary<string, RecordingSessionState> SessionStates { get; } = new ConcurrentDictionary<string, RecordingSessionState>();
        private DataService DataService { get; }
        private IWebHostEnvironment HostingEnv { get; }
        internal void SaveFrame(byte[] frameBytes, int left, int top, int width, int height, string viewerID, string machineName, DateTimeOffset startTime)
        {
            var rcFrame = new RemoteControlFrame(frameBytes, left, top, width, height, viewerID, machineName, startTime);
            FrameQueue.Enqueue(rcFrame);

            lock (LockObject)
            {
                if (ProcessingTask?.IsCompleted ?? true)
                {
                    ProcessingTask = Task.Run(new Action(StartProcessing));
                }
            }
        }

        internal void StartProcessing()
        {
            try
            {
                while (FrameQueue.Count > 0)
                {
                    if (FrameQueue.TryDequeue(out var frame))
                    {
                        var saveDir = Directory.CreateDirectory(GetSaveFolder(frame));

                        var saveFile = Path.Combine(saveDir.FullName, $"Recording.mp4");

                        if (!SessionStates.ContainsKey(frame.ViewerID))
                        {
                            SessionStates[frame.ViewerID] = new RecordingSessionState()
                            {
                                CumulativeFrame = new Bitmap(frame.Width, frame.Height)
                            };
                            var ffmpegProc = new Process();
                            SessionStates[frame.ViewerID].FfmpegProcess = ffmpegProc;

                            switch (EnvironmentHelper.Platform)
                            {
                                case Shared.Enums.Platform.Windows:
                                    ffmpegProc.StartInfo.FileName = "ffmpeg.exe";
                                    break;
                                case Shared.Enums.Platform.Linux:
                                    ffmpegProc.StartInfo.FileName = "ffmpeg";
                                    break;
                                case Shared.Enums.Platform.OSX:
                                case Shared.Enums.Platform.Unknown:
                                default:
                                    return;
                            }

                            ffmpegProc.StartInfo.Arguments = $"-y -f image2pipe -i pipe:.jpg -r 5 \"{saveFile}\"";
                            ffmpegProc.StartInfo.UseShellExecute = false;
                            ffmpegProc.StartInfo.RedirectStandardInput = true;

                            ffmpegProc.Start();
                        }

                        var bitmap = SessionStates[frame.ViewerID].CumulativeFrame;
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            using (var ms = new MemoryStream(frame.FrameBytes))
                            {
                                using (var saveImage = Image.FromStream(ms))
                                {
                                    graphics.DrawImage(saveImage, frame.Left, frame.Top);
                                }
                            }
                        }
                        using (var ms = new MemoryStream())
                        {
                            bitmap.Save(ms, ImageFormat.Jpeg);
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.CopyTo(SessionStates[frame.ViewerID].FfmpegProcess.StandardInput.BaseStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DataService.WriteEvent(ex, null);
            }
        }

        internal void StopProcessing(string viewerID)
        {
            SessionStates[viewerID].FfmpegProcess.StandardInput.Flush();
            SessionStates[viewerID].FfmpegProcess.StandardInput.Close();
            SessionStates[viewerID].FfmpegProcess.Close();
            SessionStates.TryRemove(viewerID, out _);
        }

        private string GetSaveFolder(RemoteControlFrame frame)
        {
            return Path.Combine(
                        HostingEnv.ContentRootPath,
                        "Recordings",
                        frame.StartTime.Year.ToString().PadLeft(4, '0'),
                        frame.StartTime.Month.ToString().PadLeft(2, '0'),
                        frame.StartTime.Day.ToString().PadLeft(2, '0'),
                        frame.MachineName,
                        frame.ViewerID,
                        frame.StartTime.ToString("HH.mm.ss.fff"));
        }
    }
}

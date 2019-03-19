using Microsoft.AspNetCore.Hosting;
using Remotely_Server.Data;
using Remotely_Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely_Server.Services
{
    public class RemoteControlSessionRecorder
    {
        private static bool IsProcessing { get; set; }
        private static ConcurrentQueue<RemoteControlFrame> FrameQueue { get; } = new ConcurrentQueue<RemoteControlFrame>();
        private static ConcurrentDictionary<string, Bitmap> CumulativeFrames { get; } = new ConcurrentDictionary<string, Bitmap>();
        private static object LockObject { get; } = new object();

        private IHostingEnvironment HostingEnv { get; }
        private DataService DataService { get; }

        public RemoteControlSessionRecorder(IHostingEnvironment hostingEnv, DataService dataService)
        {
            HostingEnv = hostingEnv;
            DataService = dataService;
        }
        internal void SaveFrame(byte[] frameBytes, int left, int top, int width, int height, string viewerID, string machineName, DateTime startTime)
        {
            var rcFrame = new RemoteControlFrame(frameBytes, left, top, width, height, viewerID, machineName, startTime);
            FrameQueue.Enqueue(rcFrame);

            lock (LockObject)
            {
                if (!IsProcessing)
                {
                    IsProcessing = true;
                    Task.Run(new Action(StartProcessing));
                }
            }
        }

        internal void StartProcessing()
        {
            lock (LockObject)
            {
                try
                {
                    while (FrameQueue.Count > 0)
                    {
                        if (FrameQueue.TryDequeue(out var frame))
                        {
                            if (!CumulativeFrames.ContainsKey(frame.ViewerID))
                            {
                                CumulativeFrames[frame.ViewerID] = new Bitmap(frame.Width, frame.Height);
                            }

                            var saveDir = Directory.CreateDirectory(GetSaveFolder(frame));

                            var saveFile = Path.Combine(
                                saveDir.FullName,
                                $"frame-{(Directory.GetFiles(saveDir.FullName).Length + 1).ToString()}.jpg");

                            var bitmap = CumulativeFrames[frame.ViewerID] as Bitmap;
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
                            bitmap.Save(saveFile, ImageFormat.Jpeg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DataService.WriteEvent(ex);
                }
                finally
                {
                    IsProcessing = false;
                }
            }
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

        internal void EncodeFrames(string viewerID)
        {
            var recordingDirs = Directory.GetDirectories(Path.Combine(
                                    HostingEnv.ContentRootPath,
                                    "Recordings",
                                    DateTime.Now.Year.ToString().PadLeft(4, '0'),
                                    DateTime.Now.Month.ToString().PadLeft(2, '0')),
                                viewerID,
                                SearchOption.AllDirectories);

            foreach (var dir in recordingDirs)
            {
                foreach (var subDir in Directory.GetDirectories(dir))
                {
                    try
                    {
                        System.Diagnostics.Process.Start("ffmpeg", $"-y -i \"{Path.Combine(subDir, "frame-%d.jpg")}\" \"{Path.Combine(subDir, "Recording.mp4")}\"").WaitForExit();
                        foreach (var file in Directory.GetFiles(subDir, "*.jpg"))
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        DataService.WriteEvent(ex);
                    }
                }
            }
        }
    }
}

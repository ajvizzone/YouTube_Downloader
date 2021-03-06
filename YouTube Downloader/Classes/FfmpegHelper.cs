﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace YouTube_Downloader.Classes
{
    public enum FileType { Audio, Error, Video }

    public class FfmpegHelper
    {
        private const string Command_Convert = " -y -i \"{0}\" -vn -f mp3 -ab 192k \"{1}\"";
        private const string Command_Crop_From = " -y -ss {0} -i \"{1}\" -acodec copy{2} \"{3}\"";
        private const string Command_Crop_From_To = " -y -ss {0} -i \"{1}\" -to {2} -acodec copy{3} \"{4}\"";

        public static void Convert(BackgroundWorker bw, string input, string output)
        {
            bool deleteInput = false;

            if (input == output)
            {
                string dest = Path.Combine(Path.GetDirectoryName(input), System.Guid.NewGuid().ToString());
                dest += Path.GetExtension(input);

                File.Move(input, dest);

                input = dest;
                deleteInput = true;
            }

            string[] args = new string[] { input, output };
            string arguments = string.Format(FfmpegHelper.Command_Convert, args);

            Process process = FfmpegHelper.StartProcess(arguments);

            bw.ReportProgress(0, process);

            bool started = false;
            double milliseconds = 0;
            string line = "";

            while ((line = process.StandardError.ReadLine()) != null)
            {
                // 'bw' is null, don't report any progress
                if (bw != null && bw.WorkerReportsProgress)
                {
                    line = line.Trim();

                    if (line.StartsWith("Duration: "))
                    {
                        int start = "Duration: ".Length;
                        int length = "00:00:00.00".Length;

                        string time = line.Substring(start, length);

                        milliseconds = TimeSpan.Parse(time).TotalMilliseconds;
                    }
                    else if (line == "Press [q] to stop, [?] for help")
                    {
                        started = true;

                        bw.ReportProgress(0);
                    }
                    else if (started && line.StartsWith("size="))
                    {
                        int start = line.IndexOf("time=") + 5;
                        int length = "00:00:00.00".Length;

                        string time = line.Substring(start, length);

                        double currentMilli = TimeSpan.Parse(time).TotalMilliseconds;
                        double percentage = (currentMilli / milliseconds) * 100;

                        bw.ReportProgress(System.Convert.ToInt32(percentage));
                    }
                    else if (started && line == string.Empty)
                    {
                        started = false;

                        bw.ReportProgress(100);
                    }
                }
            }

            process.WaitForExit();

            if (!process.HasExited)
                process.Kill();

            if (deleteInput)
            {
                MainForm.DeleteFiles(input);
            }
        }

        public static void Crop(BackgroundWorker bw, string input, string output, string start)
        {
            bool deleteInput = false;

            if (input == output)
            {
                string dest = Path.Combine(Path.GetDirectoryName(input), System.Guid.NewGuid().ToString());
                dest += Path.GetExtension(input);

                File.Move(input, dest);

                input = dest;
                deleteInput = true;
            }

            TimeSpan from = TimeSpan.Parse(start);

            string[] args = new string[]
            {
                string.Format("{0:00}:{1:00}:{2:00}.{3:000}", from.Hours, from.Minutes, from.Seconds, from.Milliseconds),
                input,
                GetFileType(input) == FileType.Video ? " -vcodec copy" : "",
                output
            };

            string arguments = string.Format(Command_Crop_From, args);

            Process process = FfmpegHelper.StartProcess(arguments);

            bw.ReportProgress(0, process);

            bool started = false;
            double milliseconds = 0;
            string line = "";

            while ((line = process.StandardError.ReadLine()) != null)
            {
                // 'bw' is null, don't report any progress
                if (bw != null && bw.WorkerReportsProgress)
                {
                    line = line.Trim();

                    if (line.StartsWith("Duration: "))
                    {
                        int lineStart = "Duration: ".Length;
                        int length = "00:00:00.00".Length;

                        string time = line.Substring(lineStart, length);

                        milliseconds = TimeSpan.Parse(time).TotalMilliseconds;
                    }
                    else if (line == "Press [q] to stop, [?] for help")
                    {
                        started = true;

                        bw.ReportProgress(0);
                    }
                    else if (started && line.StartsWith("size="))
                    {
                        int lineStart = line.IndexOf("time=") + 5;
                        int length = "00:00:00.00".Length;

                        string time = line.Substring(lineStart, length);

                        double currentMilli = TimeSpan.Parse(time).TotalMilliseconds;
                        double percentage = (currentMilli / milliseconds) * 100;

                        bw.ReportProgress(System.Convert.ToInt32(percentage));
                    }
                    else if (started && line == string.Empty)
                    {
                        started = false;

                        bw.ReportProgress(100);
                    }
                }
            }

            process.WaitForExit();

            if (!process.HasExited)
                process.Kill();

            if (deleteInput)
            {
                MainForm.DeleteFiles(input);
            }
        }

        public static void Crop(BackgroundWorker bw, string input, string output, string start, string end)
        {
            bool deleteInput = false;

            if (input == output)
            {
                string dest = Path.Combine(Path.GetDirectoryName(input), System.Guid.NewGuid().ToString());
                dest += Path.GetExtension(input);

                File.Move(input, dest);

                input = dest;
                deleteInput = true;
            }

            TimeSpan from = TimeSpan.Parse(start);
            TimeSpan to = TimeSpan.Parse(end);
            TimeSpan length = new TimeSpan((long)Math.Abs(from.Ticks - to.Ticks));

            string[] args = new string[]
            {
                string.Format("{0:00}:{1:00}:{2:00}.{3:000}", from.Hours, from.Minutes, from.Seconds, from.Milliseconds),
                input,
                string.Format("{0:00}:{1:00}:{2:00}.{3:000}", length.Hours, length.Minutes, length.Seconds, length.Milliseconds),
                GetFileType(input) == FileType.Video ? " -vcodec copy" : "",
                output
            };

            string arguments = string.Format(Command_Crop_From_To, args);

            Process process = FfmpegHelper.StartProcess(arguments);

            bw.ReportProgress(0, process);

            bool started = false;
            double milliseconds = 0;
            string line = "";

            while ((line = process.StandardError.ReadLine()) != null)
            {
                // 'bw' is null, don't report any progress
                if (bw != null && bw.WorkerReportsProgress)
                {
                    line = line.Trim();

                    milliseconds = TimeSpan.Parse(end).TotalMilliseconds;

                    if (line == "Press [q] to stop, [?] for help")
                    {
                        started = true;

                        bw.ReportProgress(0);
                    }
                    else if (started && line.StartsWith("size="))
                    {
                        int lineStart = line.IndexOf("time=") + 5;
                        int lineLength = "00:00:00.00".Length;

                        string time = line.Substring(lineStart, lineLength);

                        double currentMilli = TimeSpan.Parse(time).TotalMilliseconds;
                        double percentage = (currentMilli / milliseconds) * 100;

                        bw.ReportProgress(System.Convert.ToInt32(percentage));
                    }
                    else if (started && line == string.Empty)
                    {
                        started = false;

                        bw.ReportProgress(100);
                    }
                }
            }

            process.WaitForExit();

            if (!process.HasExited)
                process.Kill();

            if (deleteInput)
            {
                MainForm.DeleteFiles(input);
            }
        }

        public static TimeSpan GetDuration(string input)
        {
            TimeSpan result = TimeSpan.Zero;
            string arguments = string.Format(" -i \"{0}\"", input);

            Process process = StartProcess(arguments);

            List<string> lines = new List<string>();

            while (!process.StandardError.EndOfStream)
            {
                lines.Add(process.StandardError.ReadLine().Trim());
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("Duration"))
                {
                    string[] split = line.Split(' ', ',');

                    result = TimeSpan.Parse(split[1]);
                    break;
                }
            }

            process.WaitForExit();

            if (!process.HasExited)
                process.Kill();

            return result;
        }

        public static FileType GetFileType(string input)
        {
            FileType result = FileType.Error;
            string arguments = string.Format(" -i \"{0}\"", input);

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = Application.StartupPath + "\\ffmpeg";
            process.StartInfo.Arguments = arguments;
            process.Start();

            List<string> lines = new List<string>();

            while (!process.StandardError.EndOfStream)
            {
                lines.Add(process.StandardError.ReadLine().Trim());
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("Stream #"))
                {
                    if (line.Contains("Video: "))
                    {
                        result = FileType.Video;
                        break;
                    }
                    else if (line.Contains("Audio: "))
                    {
                        // File contains audio stream, so if a video stream
                        // is not found it's a audio file.
                        result = FileType.Audio;
                    }
                }
            }

            process.WaitForExit();

            if (!process.HasExited)
                process.Kill();

            return result;
        }

        /// <summary>
        /// Creates a Process with the given arguments, then returns
        /// it after it has started.
        /// </summary>
        private static Process StartProcess(string arguments)
        {
            Process process = new Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = Application.StartupPath + "\\ffmpeg";
            process.StartInfo.Arguments = arguments;
            process.Start();

            return process;
        }
    }
}
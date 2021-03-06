﻿//Copy rights are reserved for Akram kamal qassas
//Email me, Akramnet4u@hotmail.com
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace YouTube_Downloader
{
    /// <summary>
    /// Contains information about the video url.
    /// </summary>
    public class YouTubeVideoQuality
    {
        /// <summary>
        /// Gets or sets the video title & file name.
        /// </summary>
        public string VideoTitle { get; set; }
        /// <summary>
        /// Gets or sets the file extention.
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Gets or sets the file download url.
        /// </summary>
        public string DownloadUrl { get; set; }
        /// <summary>
        /// Gets or sets the YouTube video url.
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// Gets or sets the YouTube video size.
        /// </summary>
        public long VideoSize { get; set; }
        /// <summary>
        /// Gets or sets the YouTube video dimension.
        /// </summary>
        public Size Dimension { get; set; }
        /// <summary>
        /// Gets the YouTube video length in seconds.
        /// </summary>
        public long Length { get; set; }

        private string FormatSize(Size value)
        {
            string s = value.Height >= 720 ? " HD" : "";

            return ((Size)value).Width + " x " + value.Height + s;
        }

        public void SetSize(long size)
        {
            this.VideoSize = size;
        }

        public void SetQuality(string extension, Size dimension)
        {
            this.Extension = extension;
            this.Dimension = dimension;
        }

        public override string ToString()
        {
            string videoExtension = this.Extension;
            string videoDimension = FormatSize(this.Dimension);
            string videoSize = string.Format(new FileSizeFormatProvider(), "{0:fs}", this.VideoSize);

            return string.Format("{0} ({1}) - {2}", videoExtension.ToUpper(), videoDimension, videoSize);
        }
    }

    /// <summary>
    /// Contains methods for getting YouTube video download urls.
    /// </summary>
    public class YouTubeDownloader
    {
        private static List<string> ExtractUrls(string html)
        {
            List<string> urls = new List<string>();
            string DataBlockStart = "\"url_encoded_fmt_stream_map\":\\s+\"(.+?)&";  // Marks start of Javascript Data Block

            html = Uri.UnescapeDataString(Regex.Match(html, DataBlockStart, RegexOptions.Singleline).Groups[1].ToString());

            string firstPatren = html.Substring(0, html.IndexOf('=') + 1);
            var matchs = Regex.Split(html, firstPatren);

            for (int i = 0; i < matchs.Length; i++)
                matchs[i] = firstPatren + matchs[i];

            foreach (var match in matchs)
            {
                if (!match.Contains("url="))
                    continue;

                string url = Helper.GetTxtBtwn(match, "url=", "\\u0026", 0);

                if (url == "")
                    url = Helper.GetTxtBtwn(match, "url=", ",url", 0);

                if (url == "")
                    url = Helper.GetTxtBtwn(match, "url=", "\",", 0);

                string sig = Helper.GetTxtBtwn(match, "sig=", "\\u0026", 0);

                if (sig == "")
                    sig = Helper.GetTxtBtwn(match, "sig=", ",sig", 0);

                if (sig == "")
                    sig = Helper.GetTxtBtwn(match, "sig=", "\",", 0);

                while ((url.EndsWith(",")) || (url.EndsWith(".")) || (url.EndsWith("\"")))
                    url = url.Remove(url.Length - 1, 1);

                while ((sig.EndsWith(",")) || (sig.EndsWith(".")) || (sig.EndsWith("\"")))
                    sig = sig.Remove(sig.Length - 1, 1);

                if (string.IsNullOrEmpty(url))
                    continue;

                if (!string.IsNullOrEmpty(sig))
                    url += "&signature=" + sig;

                urls.Add(url);
            }

            return urls;
        }

        private static bool GetSize(YouTubeVideoQuality q)
        {
            try
            {
                HttpWebRequest fileInfoRequest = (HttpWebRequest)HttpWebRequest.Create(q.DownloadUrl);
                fileInfoRequest.Proxy = Helper.InitialProxy();
                HttpWebResponse fileInfoResponse = (HttpWebResponse)fileInfoRequest.GetResponse();
                long bytesLength = fileInfoResponse.ContentLength;

                fileInfoRequest.Abort();

                if (bytesLength != -1)
                {
                    q.SetSize(bytesLength);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetTitle(string rssDoc)
        {
            string str14 = Helper.GetTxtBtwn(rssDoc, "'VIDEO_TITLE': '", "'", 0);

            if (str14 == "")
                str14 = Helper.GetTxtBtwn(rssDoc, "\"title\" content=\"", "\"", 0);

            if (str14 == "")
                str14 = Helper.GetTxtBtwn(rssDoc, "&title=", "&", 0);

            str14 = str14.Replace(@"\", "").Replace("'", "&#39;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("+", " ");

            return str14;
        }

        private static bool GetQuality(YouTubeVideoQuality q, bool wide)
        {
            int iTagValue;
            string itag = Regex.Match(q.DownloadUrl, @"itag=([1-9]?[0-9]?[0-9])", RegexOptions.Singleline).Groups[1].ToString();

            if (itag != "")
            {
                if (int.TryParse(itag, out iTagValue) == false)
                    iTagValue = 0;

                switch (iTagValue)
                {
                    case 5: q.SetQuality("flv", new Size(320, (wide ? 180 : 240))); break;
                    case 6: q.SetQuality("flv", new Size(480, (wide ? 270 : 360))); break;
                    case 17: q.SetQuality("3gp", new Size(176, (wide ? 99 : 144))); break;
                    case 18: q.SetQuality("mp4", new Size(640, (wide ? 360 : 480))); break;
                    case 22: q.SetQuality("mp4", new Size(1280, (wide ? 720 : 960))); break;
                    case 34: q.SetQuality("flv", new Size(640, (wide ? 360 : 480))); break;
                    case 35: q.SetQuality("flv", new Size(854, (wide ? 480 : 640))); break;
                    case 36: q.SetQuality("3gp", new Size(320, (wide ? 180 : 240))); break;
                    case 37: q.SetQuality("mp4", new Size(1920, (wide ? 1080 : 1440))); break;
                    case 38: q.SetQuality("mp4", new Size(2048, (wide ? 1152 : 1536))); break;
                    case 43: q.SetQuality("webm", new Size(640, (wide ? 360 : 480))); break;
                    case 44: q.SetQuality("webm", new Size(854, (wide ? 480 : 640))); break;
                    case 45: q.SetQuality("webm", new Size(1280, (wide ? 720 : 960))); break;
                    case 46: q.SetQuality("webm", new Size(1920, (wide ? 1080 : 1440))); break;
                    case 82: q.SetQuality("3D.mp4", new Size(480, (wide ? 270 : 360))); break;        // 3D
                    case 83: q.SetQuality("3D.mp4", new Size(640, (wide ? 360 : 480))); break;        // 3D
                    case 84: q.SetQuality("3D.mp4", new Size(1280, (wide ? 720 : 960))); break;       // 3D
                    case 85: q.SetQuality("3D.mp4", new Size(1920, (wide ? 1080 : 1440))); break;     // 3D
                    case 100: q.SetQuality("3D.webm", new Size(640, (wide ? 360 : 480))); break;      // 3D
                    case 101: q.SetQuality("3D.webm", new Size(640, (wide ? 360 : 480))); break;      // 3D
                    case 102: q.SetQuality("3D.webm", new Size(1280, (wide ? 720 : 960))); break;     // 3D
                    case 120: q.SetQuality("live.flv", new Size(1280, (wide ? 720 : 960))); break;    // Live-streaming - should be ignored?
                    default: q.SetQuality("itag-" + itag, new Size(0, 0)); break;       // unknown or parse error
                }

                return true;
            }

            return false;
        }

        public static List<YouTubeVideoQuality> GetYouTubeVideoUrls(params string[] videoUrls)
        {
            List<YouTubeVideoQuality> urls = new List<YouTubeVideoQuality>();
            foreach (var VideoUrl in videoUrls)
            {
                string html = Helper.DownloadWebPage(VideoUrl);
                string title = GetTitle(html);
                // foreach (var videoLink in ExtractUrls(html))
                foreach (var videoLink in NewMethod.GetUrl(VideoUrl))
                {
                    YouTubeVideoQuality q = new YouTubeVideoQuality();
                    q.VideoUrl = VideoUrl;
                    q.VideoTitle = title;
                    q.DownloadUrl = videoLink + "&title=" + title;

                    if (!GetSize(q))
                        continue;

                    q.Length = long.Parse(Regex.Match(html, "\"length_seconds\":(.+?),", RegexOptions.Singleline).Groups[1].ToString());
                    bool IsWide = IsWideScreen(html);

                    if (GetQuality(q, IsWide))
                        urls.Add(q);
                }
            }
            return urls;
        }

        /// <summary>
        /// Check whether the video is in widescreen format.
        /// </summary>
        public static Boolean IsWideScreen(string html)
        {
            bool res = false;
            string match = Regex.Match(html, @"'IS_WIDESCREEN':\s+(.+?)\s+", RegexOptions.Singleline).Groups[1].ToString().ToLower().Trim();

            res = ((match == "true") || (match == "true,"));

            return res;
        }

        /// <summary>
        /// Contains a new method for getting download urls.
        /// </summary>
        class NewMethod
        {
            public static List<string> GetUrl(string pageURL)
            {
                string HTML = new WebClient().DownloadString(pageURL);
                string temp = "\"\\\\/\\\\/s.ytimg.com\\\\/yts\\\\/jsbin\\\\/html5player-(.+?)\\.js\"";
                Match m = Regex.Match(HTML, temp);
                Group g = m.Groups[1];
                string Player_Version = g.ToString();
                temp = "http://s.ytimg.com/yts/jsbin/" + "html5player-" + Player_Version + ".js";
                string Player_Code = new WebClient().DownloadString(temp);
                temp = "\"url_encoded_fmt_stream_map\":\\s+\"(.+?)\"";
                HTML = Uri.UnescapeDataString(Regex.Match(HTML, temp, RegexOptions.Singleline).Groups[1].ToString());

                MatchCollection Streams = Regex.Matches(HTML, "(^url=|(\\\\u0026url=|,url=))(.+?)(\\\\u0026|,|$)");
                MatchCollection Signatures = Regex.Matches(HTML, "(^s=|(\\\\u0026s=|,s=))(.+?)(\\\\u0026|,|$)");
                List<string> URLs = new List<string>();

                for (int i = 0; i < Streams.Count - 1; i++)
                {
                    string URL = Streams[i].Groups[3].ToString();

                    if (Signatures.Count > 0)
                    {
                        string sign = Sign_Decipher(Signatures[i].Groups[3].ToString(), Player_Code);
                        URL += "&signature=" + sign;
                    }

                    URLs.Add(URL.Trim());
                }

                return URLs;
            }

            private static string Sign_Decipher(string s, string code)
            {
                string Function_Name = Regex.Match(code, "signature=(\\w+)\\(\\w+\\)").Groups[1].ToString();
                Match Function_Match = Regex.Match(code, "function " + Function_Name + "\\((\\w+)\\)\\{(.+?)\\}", RegexOptions.Singleline);
                string var = Function_Match.Groups[1].ToString();
                string Decipher = Function_Match.Groups[2].ToString();
                string[] Lines = Decipher.Split(';');

                for (int i = 0; i < Lines.Length; i++)
                {
                    string Line = Lines[i].Trim();

                    if (Regex.IsMatch(Line, var + "=" + var + "\\.reverse\\(\\)"))
                    {
                        char[] charArray = s.ToCharArray();
                        Array.Reverse(charArray);
                        s = new string(charArray);
                    }
                    else if (Regex.IsMatch(Line, var + "=" + var + "\\.slice\\(\\d+\\)"))
                    {
                        s = Slice(s, Convert.ToInt32(Regex.Match(Line, var + "=" + var + "\\.slice\\((\\d+)\\)").Groups[1].ToString()));
                    }
                    else if (Regex.IsMatch(Line, var + "=\\w+\\(" + var + ",\\d+\\)"))
                    {
                        s = Swap(s, Convert.ToInt32(Regex.Match(Line, var + "=\\w+\\(" + var + ",(\\d+)\\)").Groups[1].ToString()));
                    }
                    else if (Regex.IsMatch(Line, var + "\\[0\\]=" + var + "\\[\\d+%" + var + "\\.length\\]"))
                    {
                        s = Swap(s, Convert.ToInt32(Regex.Match(Line, var + "\\[0\\]=" + var + "\\[(\\d+)%" + var + "\\.length\\]").Groups[1].ToString()));
                    }
                }

                return s;
            }

            private static string Slice(string input, int length)
            {
                return input.Substring(length);
            }

            private static string Swap(string input, int position)
            {
                StringBuilder Str = new StringBuilder(input);
                char SwapChar = Str[position];

                Str[position] = Str[0];
                Str[0] = SwapChar;

                return Str.ToString();
            }
        }
    }
}
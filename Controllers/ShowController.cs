using HomeShow.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;
using DateTimeX;
namespace HomeShow.Controllers
{
    public class ShowController : Controller
    {

        public ActionResult Details(string Id, string ShowName, int ShowType = 0, int OrderBy = 0)
        {
            var show = Mapping.GetMappings().FirstOrDefault(x => x.Id == Id);
            ViewBag.ShowCategory = show.Name;
            ViewBag.Id = Id;
            var showPath = $"{show.Path}{ShowName}/";
            var model = new ShowModel()
            {
                Name = ShowName,
                ShowType = ShowType,
                ImagePath = $"{showPath}logo.png"
            };
            var allowedExt = new string[] { ".mp4", ".mkv", ".ts" };
            var localPath = Server.MapPath(showPath);
            var HasSeasons = Directory.GetDirectories(localPath).Length > 0;
            var files = Directory.GetFiles(localPath, "*.*", SearchOption.AllDirectories).ToList();
            if (OrderBy == 0 && ShowType > 0)
            {
                files = files.OrderByDescending(x => new FileInfo(x).LastWriteTime).ToList();
                ViewBag.OrderBy = 1;
            }
            else
                ViewBag.OrderBy = 0;
            var padding = string.Join("", Enumerable.Repeat("0", files.Count.ToString().Length));
            foreach (var item in files)
            {
                var ext = Path.GetExtension(item).ToLower();
                if (allowedExt.Contains(ext))
                {
                    var itemPath = item.Replace(localPath, "").Replace("\\", "/");
                    int epNum;
                    var src = $"{showPath}{itemPath}";
                    var subtitlePath = Path.ChangeExtension(item, ".srt");
                    model.Episodes.Add(new Ep()
                    {
                        src = src,
                        HasSubtitle = System.IO.File.Exists(subtitlePath),
                        Group = HasSeasons ? Path.GetFileName(Path.GetDirectoryName(item)) : "",
                        title = Path.GetFileNameWithoutExtension(item)
                    });
                    if (int.TryParse(model.Episodes.Last().title, out epNum))
                    {
                        model.Episodes.Last().title = epNum.ToString(padding);
                    }
                }
            }

            if (!HasSeasons)
                model.Episodes = model.Episodes.OrderBy(x => x.title).ToList();
            return View(model);
        }
        public ActionResult List(string Id, int OrderBy = 0)
        {
            var show = Mapping.GetMappings().FirstOrDefault(x => x.Id == Id);
            ViewBag.Id = Id;
            var model = new List<ShowModel>();
            var showDirectories = Directory.GetDirectories(Server.MapPath(show.Path)).ToList();
            if (OrderBy == 0)
            {
                showDirectories = showDirectories.OrderByDescending(x => new DirectoryInfo(x).LastWriteTime).ToList();
                ViewBag.OrderBy = 1;
            }
            else
                ViewBag.OrderBy = 0;
            foreach (var item in showDirectories)
            {
                var showName = Path.GetFileName(item);
                model.Add(new ShowModel()
                {
                    ImagePath = $"{show.Path}/{showName}/logo.png",
                    Name = showName
                });
            }
            return View(model);
        }
        #region Subtitles
        public string Subtitles(string src)
        {
            var SubTitleItems = LoadSubtitles(src);
            if (SubTitleItems.Any())
            {
                var vtt = "WEBVTT\n\n";
                vtt += string.Join("\r\n", SubTitleItems.Select(x => x.ToVttLine()));
                return vtt;
            }
            return "";
        }
        List<SubTitleItem> LoadSubtitles(string src)
        {
            var subTitlePath = Path.ChangeExtension(Server.MapPath(src), ".srt");
            var SubTitleItems = new List<SubTitleItem>();
            if (System.IO.File.Exists(subTitlePath))
            {
                var lines = System.IO.File.ReadAllLines(subTitlePath, Encoding.UTF8);
                bool ReadText = false;
                TimeSpan start = new TimeSpan(), end = new TimeSpan(), ts1, ts2;
                string Text = "";
                foreach (var line in lines)
                {
                    if (IsTimeLine(line, out ts1, out ts2))
                    {
                        start = ts1;
                        end = ts2;
                        ReadText = true;
                    }
                    else if (IsSubtitleCounter(line) || string.IsNullOrEmpty(line))
                    {
                        if (!string.IsNullOrEmpty(Text))
                        {
                            SubTitleItems.Add(new SubTitleItem() { Start = start, End = end, Text = Text });
                            ReadText = false;
                            Text = "";
                        }
                    }
                    else if (ReadText)
                    {
                        Text += line + Environment.NewLine;
                    }
                }
            }
            return SubTitleItems;
        }
        bool IsSubtitleCounter(string str)
        {
            if (str.Contains(" "))
                str = str.Replace(" ", "");
            int counter = 0;
            return int.TryParse(str, out counter);
        }
        bool IsTimeLine(string str, out TimeSpan start, out TimeSpan end)
        {
            if (str.Contains(" "))
                str = str.Replace(" ", "");
            Regex regex = new Regex(@"(?<time>(?<startHour>\d+):(?<startMinut>\d+):(?<startSecond>\d+),(?<startMiliSecond>\d+)--\>(?<endHour>\d+):(?<endMinut>\d+):(?<endSecond>\d+),(?<endMiliSecond>\d+))");

            Match match = regex.Match(str);
            if (match.Success)
            {
                int startHour = int.Parse(match.Groups["startHour"].Value);
                int startMinut = int.Parse(match.Groups["startMinut"].Value);
                int startSecond = int.Parse(match.Groups["startSecond"].Value);
                int startMiliSecond = int.Parse(match.Groups["startMiliSecond"].Value);
                int endHour = int.Parse(match.Groups["endHour"].Value);
                int endMinut = int.Parse(match.Groups["endMinut"].Value);
                int endSecond = int.Parse(match.Groups["endSecond"].Value);
                int endMiliSecond = int.Parse(match.Groups["endMiliSecond"].Value);
                start = new TimeSpan(0, startHour, startMinut, startSecond, startMiliSecond);
                end = new TimeSpan(0, endHour, endMinut, endSecond, endMiliSecond);
            }
            else
            {
                start = new TimeSpan();
                end = new TimeSpan();
            }
            return match.Success;

        }
        public ActionResult GetClosestSubTitles(string src, TimeSpan? time)
        {
            var SubTitleItems = LoadSubtitles(src);
            var result = new List<SubTitleItem>();
            var ClosestTime = time.ClosestTime(SubTitleItems.Select(x => x.Start as TimeSpan?).ToArray());
            var ClosestSubtitle = SubTitleItems.FirstOrDefault(x => x.Start == ClosestTime);
            var index = SubTitleItems.IndexOf(ClosestSubtitle);
            if (index != -1)
            {
                var startIndex = Math.Max(0, index - 5);
                result = SubTitleItems.Skip(startIndex).Take(10).ToList();
                return PartialView("_SubtitlesSelector", new SubTitleSyncModel()
                {
                    StartIndex = startIndex,
                    src = src,
                    Index = result.IndexOf(ClosestSubtitle),
                    Subtitles = result
                });
            }
            return Content("");
        }
        public ActionResult SyncNextSubtitles(string src, int currentIndex, TimeSpan currentTime)
        {
            var SubTitleItems = LoadSubtitles(src);
            var SrtFilePath = Path.ChangeExtension(Server.MapPath(src), ".srt");
            var SelectedSubtitles = SubTitleItems[currentIndex];
            var Shift = currentTime - SelectedSubtitles.Start;
            ShiftAllAfterIndex(SubTitleItems, Shift, currentIndex);
            WriteToFile(SubTitleItems, SrtFilePath);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        static void ShiftAllAfterIndex(List<SubTitleItem> SubTitleItems, TimeSpan Shift, int index)
        {
            for (int i = index; i < SubTitleItems.Count; i++)
            {
                var item = SubTitleItems[i];
                item.Shift(Shift);
            }
        }
        static void WriteToFile(List<SubTitleItem> SubTitleItems, string FilePath)
        {
            string NewSubTitleFile = "";
            int Counter = 1;
            foreach (var item in SubTitleItems)
            {
                item.Text = item.Text.Trim();
                NewSubTitleFile += Counter++ + Environment.NewLine + item.ToStr();
            }
            System.IO.File.WriteAllText(FilePath, NewSubTitleFile, GetEncoding(FilePath));
        }
        static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default;
        }
        #endregion
    }
}
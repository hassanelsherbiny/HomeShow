using HomeShow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
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
            var allowedExt = new string[] { ".mp4", ".mkv" };
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
                    model.Episodes.Add(new Ep()
                    {
                        src = $"{showPath}{itemPath}",
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
    }
}
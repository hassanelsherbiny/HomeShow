using System;
using System.Collections.Generic;
namespace HomeShow.Models
{
    public class SubTitleSyncModel
    {
        public int Index { get; set; }
        public int StartIndex { get; set; }
        public string src { get; set; }
        public List<SubTitleItem> Subtitles { get; set; }
    }
}
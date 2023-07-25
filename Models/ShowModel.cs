using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeShow.Models
{
    public class ShowModel
    {
        public string ImagePath { get; set; }
        public string Name { get; set; }
        public List<Ep> Episodes { get; set; }
        public int ShowType { get; set; }
        public ShowModel()
        {
            Episodes = new List<Ep>();
        }
    }
    public class Ep
    {
        public string src { get; set; }
        public string title { get; set; }
        public string Group { get; set; }
    }
}
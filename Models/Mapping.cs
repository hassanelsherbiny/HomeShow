using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace HomeShow.Models
{
    [Serializable]
    public class Mapping
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int MapTypeId { get; set; }
        public bool Visible { get; set; }
        public MapType MapType { get { return (MapType)MapTypeId; } }
        public static List<Mapping> GetMappings()
        {
            var result = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/mapping.json");
            return new JavaScriptSerializer().Deserialize<List<Mapping>>(result);
        }
    }
    public enum MapType
    {
        EpContainer,
        MainContainer
    }
}
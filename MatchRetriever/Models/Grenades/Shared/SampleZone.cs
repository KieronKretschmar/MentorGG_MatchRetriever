using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.Grenades
{
    public class SampleZone
    {
        public int ZoneId { get; set; }
        public string Name { get; set; }
        public bool IsCtZone { get; set; }
        public float CenterXPixel { get; set; }
        public float CenterYPixel { get; set; }
        public List<int> PolygonPointsX { get; set; } = new List<int>();
        public List<int> PolygonPointsY { get; set; } = new List<int>();
        public int ParentZoneId { get; set; }
        public int Depth { get; set; }
    }
}
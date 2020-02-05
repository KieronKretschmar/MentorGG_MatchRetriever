using MatchRetriever.Helpers.Trajectories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DvFireNade
    {
        [JsonProperty]
        public string PlayerId { get; set; }

        [JsonProperty]
        public List<TrajectoryPoint> Trajectory { get; set; }
    }
}
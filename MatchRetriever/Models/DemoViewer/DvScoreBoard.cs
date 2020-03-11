using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer
{
    //[JsonObject(MemberSerialization.OptIn)]
    public partial class DvScoreboard
    {
        public int CtRounds { get; set; }
        public int TerroristRounds { get; set; }
        public bool OriginalSide { get; set; }

        public Dictionary<long, PlayerScoreboardEntry> PlayerScores { get; set; }

    }
}
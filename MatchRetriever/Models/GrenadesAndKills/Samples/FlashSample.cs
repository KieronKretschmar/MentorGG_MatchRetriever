using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database;
using MatchRetriever.Models.GrenadesAndKills;
using System.Numerics;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FlashSample : GrenadeSample
    {
        public string Id => $"Flash-{MatchId}-{GrenadeId}";
        public int ZoneId { get; set; }

        public List<FlashedSample> Flasheds { get; set; }
        public struct FlashedSample
        {
            public Vector3 VictimPos { get; set; }
            public int TimeFlashed { get; set; }
            public bool FlashAssist { get; set; }
            public bool TeamAttack { get; set; }
            public bool VictimIsAttacker { get; set; }
        }
    }
}

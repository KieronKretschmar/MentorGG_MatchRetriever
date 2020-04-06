using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class KillSample : ISample
    {
        public string Id => $"Kill-{MatchId}-{KillId}";
        public long MatchId { get; set; }
        public long KillId { get; set; }
        public short Round { get; set; }
        public long Time { get; set;}
        public long SteamId { get; set; }
        public string PlayerName { get; set; }
        public long VictimId { get; set; }
        public string VictimName { get; set; }
        public bool UserIsCt { get; set; }

        public int PlayerZoneId { get; set; }
        public int VictimZoneId { get; set; }

        public Vector3 PlayerPos { get; set; }
        public Vector3 VictimPos { get; set; }

        public bool UserWinner { get; set; }

        public KillFilterSetting FilterSettings { get; set; }

    }
}

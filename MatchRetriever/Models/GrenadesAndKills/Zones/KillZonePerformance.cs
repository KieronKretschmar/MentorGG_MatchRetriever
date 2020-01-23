using System;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class KillZonePerformance : IZonePerformance<KillZonePerformance>
    {
        public int? ZoneId { get; set; }
        public bool IsCtZone { get; set; }
        public int SampleCount { get; set; }

        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Damage { get; set; } = 0;

        public KillZonePerformance Absorb(KillZonePerformance k2)
        {
            var res = new KillZonePerformance();

            res.ZoneId = ZoneId;

            res.Kills = Kills + k2.Kills;
            res.Deaths = Deaths + k2.Deaths;
            res.Damage = Damage + k2.Damage;

            return res;
        }
    }

}

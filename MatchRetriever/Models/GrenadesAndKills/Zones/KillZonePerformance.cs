using System;
using ZoneReader.Enums;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class KillZonePerformance : ZonePerformance<KillZonePerformance>
    {
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Damage { get; set; } = 0;

        public override void Absorb(KillZonePerformance k2)
        {
            ZoneId = ZoneId;
            Kills = Kills + k2.Kills;
            Deaths = Deaths + k2.Deaths;
            Damage = Damage + k2.Damage;
        }
    }

}

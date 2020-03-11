using System;
using ZoneReader.Enums;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FireNadeZonePerformance : ZonePerformance<FireNadeZonePerformance>
    {
        public int DamagingNadesCount { get; set; } = 0;
        public int AmountHealth { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int MaxDamage { get; set; } = 0;

        /// <summary>
        /// NOT IMPLEMENTED YET
        /// </summary>
        public int TaggedAssists { get; set; } = 0;

        public override void Absorb(FireNadeZonePerformance other)
        {
            ZoneId = ZoneId;
            IsCtZone = IsCtZone;
            SampleCount = SampleCount + other.SampleCount;
            DamagingNadesCount = DamagingNadesCount + other.DamagingNadesCount;
            AmountHealth = AmountHealth + other.AmountHealth;
            TaggedAssists = TaggedAssists + other.TaggedAssists;
            MaxDamage = Math.Max(MaxDamage, other.MaxDamage);
        }
    }

}

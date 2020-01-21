using System;

namespace MatchRetriever.Models.Grenades
{
    public class FireNadeZonePerformance : IZonePerformance<FireNadeZonePerformance>
    {
        public int? ZoneId { get; set; }
        public bool IsCtZone { get; set; }
        public int SampleCount { get; set; } = 0;
        public int DamagingNadesCount { get; set; } = 0;

        public int AmountHealth { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int MaxDamage { get; set; } = 0;

        /// <summary>
        /// NOT IMPLEMENTED YET
        /// </summary>
        public int TaggedAssists { get; set; } = 0;

        public FireNadeZonePerformance Absorb(FireNadeZonePerformance f2)
        {
            var res = new FireNadeZonePerformance();

            res.ZoneId = ZoneId;
            res.IsCtZone = IsCtZone;

            res.SampleCount = SampleCount + f2.SampleCount;
            res.DamagingNadesCount = DamagingNadesCount + f2.DamagingNadesCount;
            res.AmountHealth = AmountHealth + f2.AmountHealth;
            res.TaggedAssists = TaggedAssists + f2.TaggedAssists;
        

            res.MaxDamage = Math.Max(MaxDamage, f2.MaxDamage);

            return res;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader.Enums;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class HeZonePerformance : ZonePerformance<HeZonePerformance>
    {
        public int DamagingNadesCount { get; set; } = 0;
        public int VictimCount { get; set; } = 0;
        public int AmountHealth { get; set; } = 0;
        public int AmountDamagePotential { get; set; } = 0;
        public int AmountArmor { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int MaxDamage { get; set; } = 0;

        public override void Absorb(HeZonePerformance h2)
        {
            ZoneId = ZoneId;
            IsCtZone = IsCtZone;
            SampleCount = SampleCount + h2.SampleCount;
            DamagingNadesCount = DamagingNadesCount + h2.DamagingNadesCount;
            VictimCount = VictimCount + h2.VictimCount;
            AmountHealth = AmountHealth + h2.AmountHealth;
            AmountDamagePotential = AmountDamagePotential + h2.AmountDamagePotential;
            AmountArmor = AmountArmor + h2.AmountArmor;
            MaxDamage = Math.Max(MaxDamage, h2.MaxDamage);
        }
    }
}

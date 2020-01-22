using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{
    public class HeZonePerformance : IZonePerformance<HeZonePerformance>
    {
        public int? ZoneId { get; set; }
        public bool IsCtZone { get; set; }
        public int SampleCount { get; set; } = 0;
        public int DamagingNadesCount { get; set; } = 0;
        public int VictimCount { get; set; } = 0;
        public int AmountHealth { get; set; } = 0;
        public int AmountDamagePotential { get; set; } = 0;
        public int AmountArmor { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int MaxDamage { get; set; } = 0;

        public HeZonePerformance Absorb(HeZonePerformance h2)
        {
            var res = new HeZonePerformance();

            res.ZoneId = ZoneId;
            res.IsCtZone = IsCtZone;

            res.SampleCount = SampleCount + h2.SampleCount;
            res.DamagingNadesCount = DamagingNadesCount + h2.DamagingNadesCount;
            res.VictimCount = VictimCount + h2.VictimCount;
            res.AmountHealth = AmountHealth + h2.AmountHealth;
            res.AmountDamagePotential = AmountDamagePotential + h2.AmountDamagePotential;
            res.AmountArmor = AmountArmor + h2.AmountArmor;


            res.MaxDamage = Math.Max(MaxDamage, h2.MaxDamage);

            return res;
        }
    }
}

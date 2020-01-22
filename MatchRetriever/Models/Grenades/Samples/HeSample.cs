using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{
    public class HeSample : GrenadeSample
    {
        public string Id => "HE" + "-" + MatchId + "-" + GrenadeId;
        public int ZoneId { get; set; }
        public List<HeHit> Hits { get; set; }

        public struct HeHit
        {
            public Vector3 VictimPos { get; set; }
            public int AmountHealth { get; set; }
            public int AmountArmor { get; set; }
            public bool Kill { get; set; }
            public bool TeamAttack { get; set; }
            public bool VictimIsAttacker { get; set; }
        }
    }
}

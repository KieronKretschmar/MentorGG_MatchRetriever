using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatchRetriever.Models.Grenades
{
    public class FireNadeSample : GrenadeSample
    {
        public string Id => "FireNade" + "-" + MatchId + "-" + GrenadeId;
        public int ZoneId { get; set; }
        public List<FireNadeVictim> Victims { get; set; }

        public int KilledEnemies => Victims.Where(x=>!x.TeamAttack).Count(x => x.Fatal);
        public int EnemyAmountHealth => Victims.Where(x => !x.TeamAttack).Sum(x => x.AmountHealth);
        public int EnemyAmountArmor => Victims.Where(x => !x.TeamAttack).Sum(x => x.AmountArmor);


        public class FireNadeVictim
        {
            public long VictimId { get; set; }
            public bool TeamAttack { get; set; }
            public bool VictimIsAttacker { get; set; }
            public List<FireNadeHit> Hits { get; set; }
            public bool Fatal => Hits.Last().Fatal;
            public int AmountHealth => Hits.Sum(x => x.AmountHealth);
            public int AmountArmor => Hits.Sum(x => x.AmountArmor);

            public struct FireNadeHit
            {
                public Vector3 VictimPos { get; set; }
                public int AmountHealth { get; set; }
                public int AmountArmor { get; set; }
                public bool Fatal { get; set; }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{

    public class SmokeSample : GrenadeSample
    {
        public string Id => $"Smoke-{MatchId}-{GrenadeId}";
        public MatchEntities.Enums.TargetResult Result { get; set; }
        public int TargetId { get; set; }
        public int LineupId { get; set; }
        public Vector2 PlayerView { get; set; }
    }
}

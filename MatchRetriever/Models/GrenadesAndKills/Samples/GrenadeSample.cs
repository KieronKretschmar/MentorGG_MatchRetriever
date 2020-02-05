using MatchRetriever.Helpers.Trajectories;
using System.Collections.Generic;
using System.Numerics;

namespace MatchRetriever.Models.GrenadesAndKills
{

    /// <summary>
    /// Stores info the webapp requires about a grenade
    /// </summary>
    public abstract class GrenadeSample : ISample
    {
        public long MatchId { get; set; }
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public long GrenadeId { get; set; }
        public short Round { get; set; }
        public bool UserIsCt { get; set; }
        public Vector3 ReleasePos { get; set; }
        public Vector3 DetonationPos { get; set; }
        public List<TrajectoryPoint> Trajectory { get; set; }
    }
}

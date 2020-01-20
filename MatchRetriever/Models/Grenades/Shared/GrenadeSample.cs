using MatchRetriever.Helpers.Trajectories;
using System.Collections.Generic;
using System.Numerics;

namespace MatchRetriever.Models.Grenades
{
    public class GrenadeSample
    {
        public long MatchId { get; set; }
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
        public long GrenadeId { get; set; }
        public short Round { get; set; }
        public bool UserIsCt { get; set; }
        public Vector3 Release { get; set; }
        public Vector3 Detonation { get; set; }
        public List<TrajectoryPoint> Trajectory { get; set; }
    }
}

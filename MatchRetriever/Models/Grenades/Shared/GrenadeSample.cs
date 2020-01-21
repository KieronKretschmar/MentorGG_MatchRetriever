using MatchRetriever.Helpers.Trajectories;
using System.Collections.Generic;
using System.Numerics;

namespace MatchRetriever.Models.Grenades
{
    public interface IGrenadeSample
    {
        Vector3 Detonation { get; set; }
        long GrenadeId { get; set; }
        long MatchId { get; set; }
        long PlayerId { get; set; }
        string PlayerName { get; set; }
        Vector3 Release { get; set; }
        short Round { get; set; }
        List<TrajectoryPoint> Trajectory { get; set; }
        bool UserIsCt { get; set; }
    }

    public class GrenadeSample : IGrenadeSample
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Helpers.Trajectories
{
    public static class TrajectoryExtensions
    {
        public static bool IsAlmostEqual(this List<TrajectoryPoint> points, List<TrajectoryPoint> otherPoints)
        {
            if (otherPoints == null || points.Count != otherPoints.Count)
                return false;

            return points
                .Zip(otherPoints, (thisPoint, otherPoint) => thisPoint.IsAlmostEqual(otherPoint))
                .All(x => x);
        }
    }
}

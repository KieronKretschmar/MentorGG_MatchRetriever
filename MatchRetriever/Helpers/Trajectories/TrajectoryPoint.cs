using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Helpers.Trajectories
{
    public struct TrajectoryPoint
    {
        public int Time { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public TrajectoryPoint(int time, double x, double y, double z)
        {
            Time = time;
            X = x;
            Y = y;
            Z = z;
        }

        public bool IsAlmostEqual(TrajectoryPoint other)
        {
            return Math.Abs(Time / 1000 - other.Time / 1000) < 0.5
                && Math.Abs(X - other.X) < 0.5
                && Math.Abs(Y - other.Y) < 0.5
                && Math.Abs(Z - other.Z) < 0.5;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds data about a player's performance (regarding a particular aspect of the game) in a Zone.
    /// </summary>

    public abstract class ZonePerformance<TZonePerformance> where TZonePerformance: ZonePerformance<TZonePerformance>
    {
        public int? ZoneId { get; set; }
        public bool IsCtZone { get; set; }
        public int SampleCount { get; set; }

        /// <summary>
        /// Adds performances of the other zone to this zone.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract TZonePerformance Absorb(TZonePerformance other);
    }
}

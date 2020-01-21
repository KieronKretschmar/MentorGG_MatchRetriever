using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{
    /// <summary>
    /// 
    /// 
    /// This "where TZonePerformance : IZonePerformance<TZonePerformance>" looks a bit weird and may be written in a cleaner fashion.
    /// It just means that TZonePerformance should implement IZonePerformance.
    /// </summary>
    /// <typeparam name="TZonePerformance"></typeparam>
    public interface IZonePerformance<TZonePerformance> where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        public int? ZoneId { get; set; }
        public bool IsCtZone { get; set; }
        public int SampleCount { get; set; }

        /// <summary>
        /// Adds performances of the other zone to this zone.
        /// </summary>
        /// <param name="f2"></param>
        /// <returns></returns>
        public abstract TZonePerformance Absorb(TZonePerformance other);
    }
}

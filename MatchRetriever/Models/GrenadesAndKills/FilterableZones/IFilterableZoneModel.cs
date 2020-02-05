using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public interface IFilterableZoneModel<TZonePerformance, TFilterSetting>
        where TZonePerformance : ZonePerformance<TZonePerformance>
        where TFilterSetting : IFilterSetting<TFilterSetting>
    {
        public List<FilterableZonePerformance<TZonePerformance, TFilterSetting>> FilterableZonePerformanceData { get; set; }
    }
}

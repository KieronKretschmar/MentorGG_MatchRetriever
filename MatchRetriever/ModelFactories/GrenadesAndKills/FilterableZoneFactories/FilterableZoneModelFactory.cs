using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public interface IFilterableZoneModelFactory<TSample, TZonePerformance, TFilterSetting>
        where TSample : ISample
        where TZonePerformance : IZonePerformance<TZonePerformance>
        where TFilterSetting : IFilterSetting<TFilterSetting>
    {
        Task<List<FilterableZonePerformance<TZonePerformance, TFilterSetting>>> GetFilterableZoneData(long steamId, List<TSample> samples, string map, List<long> matchIds);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class KillModel : BaseSampleModel<KillSample>, IFilterableZoneModel<KillZonePerformance, KillFilterSetting>
    {
        public List<FilterableZonePerformance<KillZonePerformance, KillFilterSetting>> FilterableZonePerformanceData { get; set; }
    }
}

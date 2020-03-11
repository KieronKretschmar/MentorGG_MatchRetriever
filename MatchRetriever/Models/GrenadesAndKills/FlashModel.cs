using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FlashModel : BaseSampleModel<FlashSample>, IZoneModel<FlashZonePerformance>
    {
        public ZonePerformanceSummary<FlashZonePerformance> ZonePerformanceSummary { get; set; }
        public List<Zone> ZoneInfos { get; set; }
    }
}

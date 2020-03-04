using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class HeModel : BaseSampleModel<HeSample>, IZoneModel<HeZonePerformance>
    {
        public ZonePerformanceSummary<HeZonePerformance> ZonePerformanceSummary { get; set; }
        public List<Zone> ZoneInfos { get; set; }
    }
}

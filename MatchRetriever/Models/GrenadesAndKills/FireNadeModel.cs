using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FireNadeModel : BaseSampleModel<FireNadeSample>, IZoneModel<FireNadeZonePerformance>
    {
        public ZonePerformanceSummary<FireNadeZonePerformance> ZonePerformanceSummary { get; set; }
        public List<Zone> ZoneInfos { get; set; }
    }
}

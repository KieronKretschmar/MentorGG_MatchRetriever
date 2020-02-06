using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class HeModel : BaseSampleModel<HeSample>, IZoneModel<HeZonePerformance>
    {
        public ZonePerformanceSummary<HeZonePerformance> ZoneData { get; set; }

    }
}

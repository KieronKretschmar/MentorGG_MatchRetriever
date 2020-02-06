using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FlashModel : BaseSampleModel<FlashSample>, IZoneModel<FlashZonePerformance>
    {
        public ZonePerformanceSummary<FlashZonePerformance> ZoneData { get; set; }

    }
}

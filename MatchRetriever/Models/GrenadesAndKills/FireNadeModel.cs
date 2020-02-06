using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FireNadeModel : BaseSampleModel<FireNadeSample>, IZoneModel<FireNadeZonePerformance>
    {
        public ZonePerformanceSummary<FireNadeZonePerformance> ZoneData { get; set; }

    }
}

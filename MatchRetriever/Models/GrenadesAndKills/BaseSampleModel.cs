using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public abstract class BaseSampleModel<TSample>
    {
        public long PlayerId { get; set; }
        public string Map { get; set; }
        public List<TSample> Samples { get; set; }
        public int RecentMatchesAnalyzedCount { get; set; }
    }
}

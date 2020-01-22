using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{
    public interface ISampleModel<TSample> where TSample : IGrenadeSample
    {
        string Map { get; set; }
        int RecentMatchesAnalyzedCount { get; set; }
        List<TSample> Samples { get; set; }
        long SteamId { get; set; }
    }

    public class SampleModel<TSample> : ISampleModel<TSample> where TSample : IGrenadeSample
    {
        public long SteamId { get; set; }
        public string Map { get; set; }
        public List<TSample> Samples { get; set; }
        public int RecentMatchesAnalyzedCount { get; set; }
    }
}

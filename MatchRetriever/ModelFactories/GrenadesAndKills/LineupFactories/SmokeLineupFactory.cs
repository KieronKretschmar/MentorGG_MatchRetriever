using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class SmokeLineupFactory : ModelFactoryBase, ILineupPerformanceFactory<SmokeSample, SmokeLineupPerformance>
    {
        public SmokeLineupFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<LineupPerformanceSummary<SmokeLineupPerformance>> LineupPerformanceSummary(long steamId, List<SmokeSample> samples, string map, List<long> matchIds)
        {
            var performance = new LineupPerformanceSummary<SmokeLineupPerformance>();

            var rounds = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => x.IsCt)
                .ToList();
            performance.CtRounds = rounds.Count(x => x);
            performance.TerroristRounds = rounds.Count(x => !x);

            performance.LineupPerformances = samples.GroupBy(x => x.LineupId)
                .ToDictionary(x => x.Key, x => new SmokeLineupPerformance
                {
                    CategoryId = x.Key,
                    Insides = x.Count(),// TODO: Filter by Result when enum is in matchentities
                    Misses = x.Count(),// TODO: Filter by Result when enum is in matchentities
                });
            return performance;
        }
    }
}

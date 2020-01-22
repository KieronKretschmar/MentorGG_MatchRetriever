using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.Grenades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public class SmokeModelFactory : LineupModelFactory<SmokeSample, SmokeLineupPerformance>
    {
        public SmokeModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected override async Task<LineupPerformanceSummary<SmokeLineupPerformance>> PlayerPerformance(long steamId, List<SmokeSample> samples, string map, List<long> matchIds)
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

        protected override async Task<List<SmokeSample>> PlayerSamples(long steamId, string map, List<long> matchIds)
        {
            var playerName = (await _steamUserOperator.GetUser(steamId)).SteamName;
            var recentAttempts = _context.Smoke.Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(smoke => new SmokeSample
                {
                    MatchId = smoke.MatchId,
                    GrenadeId = smoke.GrenadeId,
                    PlayerId = smoke.PlayerId,
                    PlayerName = playerName,
                    Round = smoke.Round,
                    UserIsCt = smoke.IsCt,
                    TargetId = smoke.Target,
                    Result = smoke.Result,
                    Detonation = smoke.GrenadePos,
                    Release = smoke.PlayerPos,
                    LineupId = smoke.Category,
                    PlayerViewX = smoke.PlayerViewX,
                    Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(smoke.Trajectory),
                })
                .ToList();
        
            return recentAttempts;
        }
    }
}

using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class SmokeSampleFactory : ModelFactoryBase, ISampleFactory<SmokeSample>
    {
        public SmokeSampleFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<List<SmokeSample>> LoadPlayerSamples(long steamId, string map, List<long> matchIds)
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
                    Detonation = smoke.DetonationPos,
                    Release = smoke.PlayerPos,
                    LineupId = smoke.LineUp,
                    PlayerViewX = smoke.PlayerView.X,
                    Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(smoke.Trajectory),
                })
                .ToList();

            return recentAttempts;
        }
    }
}

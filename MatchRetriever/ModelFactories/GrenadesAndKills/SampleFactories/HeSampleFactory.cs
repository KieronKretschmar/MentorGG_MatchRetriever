using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class HeSampleFactory : ModelFactoryBase, ISampleFactory<HeSample>
    {
        public HeSampleFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<List<HeSample>> LoadPlayerSamples(long steamId, string map, List<long> matchIds)
        {
            var playerName = (await _steamUserOperator.GetUser(steamId)).SteamName;
            var samples = _context.He
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(he => new HeSample
                {
                    MatchId = he.MatchId,
                    GrenadeId = he.GrenadeId,
                    Round = he.Round,
                    PlayerId = he.PlayerId,
                    PlayerName = playerName,
                    UserIsCt = he.IsCt,
                    ZoneId = he.DetonationZoneByTeam,
                    Hits = he.Damage.Select(damage => new HeSample.HeHit
                    {
                        VictimPos = damage.VictimPos,
                        AmountArmor = damage.AmountArmor,
                        AmountHealth = damage.AmountHealth,
                        TeamAttack = damage.TeamAttack,
                        VictimIsAttacker = damage.PlayerId == damage.VictimId,
                        Kill = damage.Fatal,
                    }).ToList(),
                    Release = he.PlayerPos,
                    Detonation = he.GrenadePos,
                    Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(he.Trajectory),
                }).ToList();
            return samples;
        }
    }
}

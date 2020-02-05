using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class FireNadeSampleFactory : ModelFactoryBase, ISampleFactory<FireNadeSample>
    {
        public FireNadeSampleFactory(IServiceProvider sp) : base(sp)
        {
        }


        public async Task<List<FireNadeSample>> LoadPlayerSamples(long steamId, string map, List<long> matchIds)
        {
            var recentAttempts = new List<FireNadeSample>();
            var playerName = (await _steamUserOperator.GetUser(steamId)).SteamName;

            var debug = _context.FireNade
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .ToList();

            recentAttempts = _context.FireNade
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .ToList()
                .Select(grenade => new FireNadeSample
                {
                    MatchId = grenade.MatchId,
                    PlayerId = steamId,
                    PlayerName = playerName,
                    GrenadeId = grenade.GrenadeId,
                    Round = grenade.Round,
                    UserIsCt = grenade.IsCt,
                    ZoneId = grenade.DetonationZoneByTeam,
                    Victims = grenade.Damage.GroupBy(dmg => dmg.VictimId).Select(g => new FireNadeSample.FireNadeVictim
                    {
                        VictimId = g.Key,
                        TeamAttack = g.First().TeamAttack,
                        VictimIsAttacker = g.Key == steamId,
                        Hits = g.Select(damage => new FireNadeSample.FireNadeVictim.FireNadeHit
                        {
                            VictimPos = damage.VictimPos,
                            AmountArmor = damage.AmountArmor,
                            AmountHealth = damage.AmountHealth,
                            Fatal = damage.Fatal,
                        }).ToList()
                    }).ToList(),
                    Release = grenade.PlayerPos,
                    Detonation = grenade.DetonationPos,
                    Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(grenade.Trajectory),
                }).ToList();
            return recentAttempts;
        }
    }
}

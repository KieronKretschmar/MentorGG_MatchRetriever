using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class KillSampleFactory : ModelFactoryBase, ISampleFactory<KillSample>
    {
        public KillSampleFactory(IServiceProvider sp) : base(sp)
        {
        }


        public async Task<List<KillSample>> LoadPlayerSamples(long steamId, string map, List<long> matchIds)
        {
            var samples = _context.Kill
                .Where(x => (x.PlayerId == steamId || x.VictimId == steamId) && matchIds.Contains(x.MatchId))
                .Select(x => new KillSample
                {
                    MatchId = x.MatchId,
                    KillId = x.KillId,
                    Round = x.Round,
                    SteamId = steamId,
                    VictimId = x.VictimId,
                    PlayerZoneId = x.PlayerZoneByTeam ?? 0,
                    VictimZoneId = x.VictimZoneByTeam ?? 0,
                    PlayerPos = x.PlayerPos,
                    VictimPos = x.VictimPos,

                    FilterSettings = new KillFilterSetting(x.Time < x.RoundStats.BombPlant.Time ? KillFilterSetting.PlantFilterStatus.AfterPlant : KillFilterSetting.PlantFilterStatus.NotYetPlanted),
                    UserWinner = x.VictimId != steamId, // False if suicide
                    UserIsCt = (x.PlayerId == steamId) ? x.IsCt : x.IsCt == x.TeamKill, // (x.IsCT == x.TeamKill) <=> VictimIsCT
                })
                .ToList();

            // Add PlayerNames and VictimNames with only one call of GetUsers
            var distinctSteamIds = samples.SelectMany(x => new List<long> { x.SteamId, x.VictimId })
                .Distinct()
                .ToList();
            var steamUserInfos = await _steamUserOperator.GetUsers(distinctSteamIds);
            var steamNameDict = distinctSteamIds.ToDictionary(x => x, x => steamUserInfos.SingleOrDefault(userinfo => userinfo.SteamId == x).SteamName);
            samples.ForEach(x => x.VictimName = steamNameDict[x.VictimId]);
            samples.ForEach(x => x.PlayerName = steamNameDict[x.SteamId]);


            return samples;
        }
    }
}

﻿using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class FlashSampleFactory : ModelFactoryBase, ISampleFactory<FlashSample>
    {
        public FlashSampleFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<List<FlashSample>> LoadPlayerSamples(long steamId, List<long> matchIds)
        {
            var recentAttempts = new List<FlashSample>();
            var playerName = (await _steamUserOperator.GetUser(steamId)).SteamName;

            var samples = _context.Flash.Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(flash => new FlashSample
                {
                    MatchId = flash.MatchId,
                    GrenadeId = flash.GrenadeId,
                    SteamId = flash.PlayerId,
                    PlayerName = playerName,
                    Round = flash.Round,
                    Time = flash.Time,
                    UserIsCt = flash.IsCt,
                    ZoneId = flash.DetonationZoneByTeam,
                    Flasheds = flash.Flashed.Select(flashed => new FlashSample.FlashedSample
                    {
                        VictimPos = flashed.VictimPos,
                        TimeFlashed = flashed.TimeFlashed,
                        TeamAttack = flashed.TeamAttack,
                        VictimIsAttacker = flash.PlayerId == flashed.VictimId,
                        FlashAssist = flashed.AssistedKillId != null
                    }).ToList(),
                    ReleasePos = flash.PlayerPos,
                    DetonationPos = flash.DetonationPos,
                    Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(flash.Trajectory),
                }).ToList();
            return samples;
        }
    }
}

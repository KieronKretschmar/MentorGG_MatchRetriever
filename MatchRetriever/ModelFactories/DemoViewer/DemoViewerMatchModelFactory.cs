using EquipmentLib;
using MatchRetriever.Models;
using MatchRetriever.Models.DemoViewer;
using MatchRetriever.Models.DemoViewer.Objects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.DemoViewer
{
    public interface IDemoViewerMatchModelFactory
    {
        Task<DemoViewerMatchModel> GetModel(long matchId);
    }

    public class DemoViewerMatchModelFactory : ModelFactoryBase, IDemoViewerMatchModelFactory
    {
        private readonly IEquipmentProvider _equipmentProvider;

        public DemoViewerMatchModelFactory(IServiceProvider sp) : base(sp)
        {
            _equipmentProvider = sp.GetRequiredService<EquipmentLib.IEquipmentProvider>();
        }

        public async Task<DemoViewerMatchModel> GetModel(long matchId)
        {
            var model = new DemoViewerMatchModel();

            var matchStats = _context.MatchStats.Single(x => x.MatchId == matchId);
            var playerMatchStats = matchStats.PlayerMatchStats.ToList();


            model.EquipmentInfo = _equipmentProvider.GetEquipmentDict((EquipmentLib.Enums.Source) matchStats.Source, matchStats.MatchDate);

            model.PlayerStats = (await _steamUserOperator.GetUsers(playerMatchStats.Select(x => x.SteamId).ToList()))
                .ToDictionary(x => x.SteamId.ToString(), x => x);


            // Match Stats
            model.MatchStats = new DvMatchStats
            {
                Map = matchStats.Map,
                MatchDate = matchStats.MatchDate,
                WinnerTeam = matchStats.WinnerTeam,
                BombTimer = matchStats.BombTimer,
                AVGRank = matchStats.AvgRank ?? 0,
                RoundTimer = matchStats.RoundTimer,
                TerroristStarterTeamStats = new DvTeamMatchStats
                {
                    Score = matchStats.TeamStats[MatchEntities.Enums.StartingFaction.TerroristStarter].RealScore,
                    NumRoundsCt = matchStats.TeamStats[MatchEntities.Enums.StartingFaction.TerroristStarter].NumRoundsCt,
                    NumRoundsTerrorist = matchStats.TeamStats[MatchEntities.Enums.StartingFaction.TerroristStarter].NumRoundsT,
                },
                CtStarterTeamStats = new DvTeamMatchStats
                {
                    Score = matchStats.TeamStats[MatchEntities.Enums.StartingFaction.CtStarter].RealScore,
                    NumRoundsCt = matchStats.TeamStats[MatchEntities.Enums.StartingFaction.CtStarter].NumRoundsCt,
                    NumRoundsTerrorist = matchStats.TeamStats[MatchEntities.Enums.StartingFaction.CtStarter].NumRoundsT,
                },
                Source = matchStats.Source.ToString(),
            };

            var overTimeStats = matchStats.OverTimeStats;
            if (overTimeStats != null)
            {
                model.OverTimeStats = new DvOverTimeStats
                {
                    NumRounds = overTimeStats.NumRounds,
                    StartCT = overTimeStats.StartCt,
                    StartT = overTimeStats.StartT
                };
            }


            model.PlayerMatchStats = playerMatchStats.ToDictionary(x => x.SteamId.ToString(), x => new DvPlayerMatchStats
            {
                Team = x.Team,
                Score = x.Score,
                Kills = x.RealKills,
                Deaths = x.RealDeaths,
                Assists = x.RealAssists,
                MVPs = x.RealMvps
            });

            model.RoundSummaries = _context.RoundStats.Where(x => x.MatchId == matchId)
                .Select(x => new
                {
                    Round = x.Round,
                    WinnerTeam = x.WinnerTeam,
                    WinType = (byte)x.WinType,
                    WinnerIsCT = x.OriginalSide == (x.WinnerTeam == MatchEntities.Enums.StartingFaction.CtStarter),
                    //Kills = x.Kills.Select(k => new
                    //{
                    //    k.PlayerId,
                    //    k.VictimId,
                    //    k.AssisterId
                    //}).ToList()
                })
                .OrderBy(x => x.Round)
                .ToList()
                .Select(x => new DemoViewerMatchModel.RoundSummary
                {
                    Round = x.Round,
                    WinnerTeam = x.WinnerTeam,
                    WinType = x.WinType,
                    WinnerIsCT = x.WinnerIsCT
                    //PlayerRoundSummaries = playerIds.Select(p => new RoundSummary.PlayerRoundSummary
                    //{
                    //    Kills = x.Kills.Count(k => k.PlayerId == p),
                    //    Deaths = x.Kills.Count(k => k.VictimId == p),
                    //    Assists = x.Kills.Count(k => k.AssisterId == p),
                    //}).ToList()
                })
                .ToList();

            return model;
        }
    }
}

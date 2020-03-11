using MatchRetriever.Enumerals;
using MatchRetriever.Helpers;
using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models;
using MatchRetriever.Models.DemoViewer;
using MatchRetriever.Models.DemoViewer.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.DemoViewer
{
    public interface IDemoViewerRoundModelFactory
    {
        Task<DemoViewerRoundModel> GetModel(long matchId, short roundNumber, DemoViewerQuality requestedQuality);
    }

    public class DemoViewerRoundModelFactory : ModelFactoryBase, IDemoViewerRoundModelFactory
    {
        private readonly IDemoViewerConfigProvider _demoViewerConfigProvider;

        public DemoViewerRoundModelFactory(IServiceProvider sp) : base(sp)
        {
            _demoViewerConfigProvider = sp.GetRequiredService<IDemoViewerConfigProvider>();
        }

        public async Task<DemoViewerRoundModel> GetModel(long matchId, short roundNumber, DemoViewerQuality requestedQuality)
        {
            var model = new DemoViewerRoundModel();

            // Take the lower quality of [availableQuality, requestedQuality]
            var availableFramesPerSecond = _context.MatchStats.Single(x => x.MatchId == matchId).Config.FramesPerSecond;
            model.Config = _demoViewerConfigProvider.GetHighestAvailableConfig(requestedQuality, availableFramesPerSecond);

            if(model.Config.Quality < requestedQuality)
            {
                _logger.LogWarning(
                    $"Requested quality [ {requestedQuality} ] was not available for match #[ {matchId} ] and round [ {roundNumber} ], " +
                    $"Probably because only [ {availableFramesPerSecond} ] FPS were available. Using quality [ {model.Config.Quality} ] instead.");
            }

            // Match Stats
            var roundStats = _context.RoundStats.Single(x => x.MatchId == matchId && x.Round == roundNumber);
            var map = roundStats.MatchStats.Map;
            var matchDate = roundStats.MatchStats.MatchDate;

            #region General tables


            model.RoundStats = new DvRoundStats
            {
                Round = roundStats.Round,
                WinnerTeam = roundStats.WinnerTeam,
                RoundTime = roundStats.RoundTime,
                WinType = roundStats.WinType, // TODO: Check if db can be changed to NOT NULL
                BombPlanted = roundStats.BombPlanted,
                EndTime = roundStats.EndTime,
                RealEndTime = roundStats.RealEndTime,
                StartTime = roundStats.StartTime,
            };

            model.PlayerRoundStats = roundStats.PlayerRoundStats.Select(x => new DvPlayerRoundStats
            {
                IsCT = x.IsCt,
                PlayerId = x.PlayerId.ToString(),
                MoneyInitial = x.MoneyInitial
            }).ToList();


            model.BotTakeOvers = roundStats.BotTakeOver
                .Select(x => new DvBotTakeOver
                {
                    BotId = x.BotId.ToString(),
                    PlayerId = x.PlayerId.ToString(),
                    Round = x.Round,
                    Time = x.Time,
                }).ToList();

            #endregion


            #region Gun-related ingame events

            model.Damages = roundStats.Damage.Select(x => new DvDamage
            {
                Time = x.Time,
                VictimId = x.VictimId.ToString(),
                PlayerId = x.PlayerId.ToString(),
                AmountHealth = x.AmountHealth,
                AmountArmor = x.AmountHealth,
                Weapon = x.Weapon,
                PlayerPos = x.PlayerPos,
                VictimPos = x.VictimPos,
            }).ToList();

            model.Kills = roundStats.Kills.Select(x => new DvKill
            {
                VictimId = x.VictimId.ToString(),
                Time = x.Time,
                PlayerId = x.PlayerId.ToString(),
                PlayerPos = x.PlayerPos,
                VictimPos = x.VictimPos,
                AssisterId = x.AssisterId.ToString(),
                AssistByFlash = x.AssistByFlash,
                KillType = x.KillType,
                Weapon = x.Weapon,
            }).ToList();

            model.WeaponFireds = roundStats.WeaponFired.Select(x => new DvWeaponFired
            {
                Time = x.Time,
                PlayerId = x.PlayerId.ToString(),
                Weapon = x.Weapon,
                PlayerPos = x.PlayerPos,
                PlayerView = x.PlayerView.X,
            }).ToList();

            model.WeaponReloads = roundStats.WeaponReload.GroupBy(x => x.PlayerId)
                        .ToDictionary(x => x.Key.ToString(), g => g.Select(x => new DvWeaponReload
                        {
                            Time = x.Time,
                            AmmoAfter = x.AmmoAfter
                        }).ToList());

            #endregion

            #region Other ingame events

            // Other Match Stats
            var bombPlant = roundStats.BombPlant;
            if (bombPlant != null)
            {
                model.BombPlant = new DvBombPlant
                {
                    Time = bombPlant.Time,
                    PlayerId = bombPlant.Time.ToString(),
                    Site = bombPlant.Site,
                    Pos = bombPlant.Pos,
                };

                var bombDefused = roundStats.BombDefused;
                if (bombDefused != null)
                {
                    model.BombDefused = new DvBombDefused
                    {
                        Time = bombDefused.Time,
                        PlayerId = bombDefused.PlayerId.ToString(),
                    };
                }

                var bombExplosion = roundStats.BombExplosion;
                if (bombExplosion != null)
                {
                    model.BombExplosion = new DvBombExplosion
                    {
                        Time = bombExplosion.Time,
                    };
                }
            }

            model.ItemSaveds = roundStats.ItemSaved
                        // Filter out entries with Equipment=0. Does not make sense, seems to be a bug in DemoAnalyzer
                        .Where(x => x.Equipment != 0)
                        .Select(x=> new
                        {
                            x.PlayerId,
                            x.Equipment,
                        })
                        .ToList()
                        .GroupBy(x => x.PlayerId)
                        .ToDictionary(x => x.Key.ToString(), g => g.Select(x => new DvItemSaved
                        {
                            Equipment = x.Equipment
                        })
                        .ToList());

            model.ItemDroppeds = roundStats.ItemDropped
                        .Select(x => new
                        {
                            x.PlayerId,
                            x.Time,
                            x.Equipment,
                            x.ItemId,
                        })
                        .ToList()
                        .GroupBy(x => x.PlayerId)
                        .ToDictionary(x => x.Key.ToString(), g => g.Select(x => new DvItemDropped
                        {
                            Time = x.Time,
                            Equipment = x.Equipment,
                            ItemId = x.ItemId,
                        })
                        .ToList());


            model.ItemPickedUps = roundStats.ItemPickedUp
                        .Select(x => new
                        {
                            x.PlayerId,
                            x.Time,
                            x.Equipment,
                            x.ItemId,
                            x.Buy,
                        })
                        .ToList()
                        .GroupBy(x => x.PlayerId)
                        .ToDictionary(x => x.Key.ToString(), g => g.Select(x => new DvItemPickedUp
                        {
                            Time = x.Time,
                            Equipment = x.Equipment,
                            ItemId = x.ItemId,
                            Buy = x.Buy,
                        })
                        .ToList());

            model.PlayerPositions = roundStats.PlayerPosition
                        .Select(x => new
                        {
                            x.PlayerId,
                            x.Time,
                            x.Weapon,
                            PlayerViewX = x.PlayerView.X,
                            x.PlayerPos,
                        })
                        .ToList()
                        .GroupBy(x => x.PlayerId)
                        // Apply Filtering for quality
                        .ToDictionary(
                            x => x.Key.ToString(), 
                            // Reduce frames to the first of each frame every (1/FPS) seconds
                            g => g
                            .GroupBy(x=>x.Time % (1000 / model.Config.FramesPerSecond))
                            .Select(x=>x.First())
                            .Select(x => new DvPlayerPosition()
                            {
                                Time = x.Time,
                                Weapon = x.Weapon,
                                PlayerView = x.PlayerViewX,
                                PlayerPos = x.PlayerPos,
                            })
                            .ToList()
                        );

            #endregion

            #region Grenades

            // Grenades
            model.Decoys = roundStats.Decoy.Select(x => new DvDecoy
            {
                PlayerId = x.PlayerId.ToString(),
                Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(x.Trajectory),
            }).ToList();

            model.FireNades = roundStats.FireNade.Select(x => new DvFireNade
            {
                PlayerId = x.PlayerId.ToString(),
                Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(x.Trajectory),
            }).ToList();

            model.Flashes = roundStats.Flash.Select(x => new DvFlash
            {
                PlayerId = x.PlayerId.ToString(),
                Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(x.Trajectory),
            }).ToList();

            model.Flasheds = roundStats.Flashed
                        .Select(x => new
                        {
                            x.VictimId,
                            x.VictimPos,
                            x.TimeFlashed,
                            DetonationTime = x.Flash.Time, // TODO (Perf. Optim.): Use Add Flashed.Time and use it instead of joining
                        })
                        .ToList()
                        .GroupBy(x => x.VictimId)
                        .ToDictionary(x => x.Key.ToString(), g => g
                        .Select(x => new DvFlashed
                        {
                            Time = x.DetonationTime,
                            TimeFlashed = x.TimeFlashed,
                            VictimPos = x.VictimPos,
                        })
                        .ToList());

            model.HEs = roundStats.He.Select(x => new DvHE
            {
                PlayerId = x.PlayerId.ToString(),
                Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(x.Trajectory),
            }).ToList();

            model.Smokes = roundStats.Smoke.Select(x => new DvSmoke
            {
                PlayerId = x.PlayerId.ToString(),
                Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(x.Trajectory),
            }).ToList();

            #endregion

            model.PartialScoreboard = await GetScoreboard(matchId, roundNumber);

            return model;
        }


        /// <summary>
        /// Creates a partial scoreboard up until, but excluding the cutoffRound
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="cutoffRound">Only rounds before the cutoffRound are regarded</param>
        private async Task<DvScoreboard> GetScoreboard(long matchId, short cutoffRound)
        {
            var scoreboard = new DvScoreboard();

            var roundResults = _context.RoundStats.Where(x => x.MatchId == matchId && x.Round < cutoffRound)
                .Select(x => new
                {
                    Round = x.Round,
                    WinnerTeam = x.WinnerTeam,
                    WinType = (byte)x.WinType,
                })
                .ToList();
            scoreboard.OriginalSide = _context.RoundStats.Single(x => x.MatchId == matchId && x.Round == cutoffRound).OriginalSide;

            var terroristStarterRounds = roundResults.Count(x => x.WinnerTeam == MatchEntities.Enums.StartingFaction.TerroristStarter);
            var ctStarterRounds = roundResults.Count(x => x.WinnerTeam == MatchEntities.Enums.StartingFaction.CtStarter);
            scoreboard.TerroristRounds = scoreboard.OriginalSide ? terroristStarterRounds : ctStarterRounds;
            scoreboard.CtRounds = scoreboard.OriginalSide ? ctStarterRounds : terroristStarterRounds;
            
            // Load scores of all Players who were active this round, including bots.
            scoreboard.PlayerScores = _context.PlayerRoundStats
                .Where(x => x.MatchId == matchId && x.Round == cutoffRound)
                .Select(x => new PlayerScoreboardEntry
                {
                    SteamId = x.PlayerId,
                    Kills = x.RoundStartKills,
                    Deaths = x.RoundStartDeaths,
                    Assists = x.RoundStartAssists,
                    MVPs = x.RoundStartMvps,
                    DamageDealt = x.RoundStartDamage,
                    Score = x.RoundStartScore,
                    RankBeforeMatch = x.PlayerMatchStats.RankBeforeMatch,
                    RankAfterMatch = x.PlayerMatchStats.RankAfterMatch,
                })
                .ToDictionary(x=> x.SteamId, x=>x);

            // Add Profiles to PlayerScores with only one call of GetUsers
            var distinctSteamIds = scoreboard.PlayerScores.Select(x=>x.Key).ToList();
            var steamUserInfos = await _steamUserOperator.GetUsers(distinctSteamIds);
            foreach (var playerEntry in scoreboard.PlayerScores)
            {
                playerEntry.Value.Profile = steamUserInfos.Single(x => x.SteamId == playerEntry.Key);
            }

            return scoreboard;
        }
    }
}

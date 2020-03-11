using Database;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public interface IBadBombDropDetector
    {
        ISituationCollection ComputeMisplays(long steamId, long matchId);
    }

    public class BadBombDropDetector : Subdetector<BadBombDrop>, IBadBombDropDetector
    {
        public BadBombDropDetector(IServiceProvider sp) : base(sp)
        {
        }

        public override ISituationCollection ComputeMisplays(long steamId, long matchId)
        {

            var badBombDrops = _context.ItemDropped
                .Where(x => x.MatchId == matchId && x.PlayerId == steamId && x.Equipment == EquipmentElement.Bomb)
                // ItemDropped.ByDeath is wrong in database as of 13.09.2019. Remove the '!' when DemoAnalyzer is fixed
                .Where(x => !x.ByDeath)
                .Select(x => new BadBombDrop
                {
                    MatchId = matchId,
                    Map = x.MatchStats.Map,
                    MatchDate = x.MatchStats.MatchDate,
                    Round = x.Round,
                    Time = x.Time,
                    PlayerId = steamId,
                })
                .ToList();



            // Compute data required for ClosestTeammateDistance
            var playerTeam = _context.PlayerMatchStats
                .Single(x => x.MatchId == matchId && x.SteamId == steamId)
                .Team;

            var teammateSteamIds = _context.PlayerMatchStats
                .Where(x => x.MatchId == matchId && x.Team == playerTeam && x.SteamId != steamId)
                .Select(x => x.SteamId)
                .ToList();

            // Assign TeamMatesAlive, WasPickedUpAfter
            foreach (var bombDrop in badBombDrops)
            {
                bombDrop.PickedUpAfter = _context.ItemPickedUp
                    .FirstOrDefault(x =>
                    x.Equipment == EquipmentElement.Bomb
                    && x.MatchId == bombDrop.MatchId
                    && x.Round == bombDrop.Round
                    && x.Time > bombDrop.Time)
                    ?.Time - bombDrop.Time ?? -1;

                var lastPlayerPosition = _context.PlayerPosition
                        .Where(x =>
                        x.PlayerId == bombDrop.PlayerId
                        && x.MatchId == bombDrop.MatchId
                        && x.Round == bombDrop.Round)
                        .OrderByDescending(x => x.Time)
                        .First();

                var teamMateDistances = _detectorHelpers.TeammateDistanceToPos(
                    bombDrop.MatchId, bombDrop.Round, bombDrop.Time,
                    teammateSteamIds, lastPlayerPosition.PlayerPos);

                bombDrop.TeammatesAlive = teamMateDistances.Count();
                bombDrop.ClosestTeammateDistance = teamMateDistances.Select(x => (int?) x).Min() ?? -1; // -1 if none alive
            }

            // Ignore bombdrops where no teammates were alive
            // This should always be done because otherwise it can not be a bad bombdrop
            // Filtering by MinTeammatesAlive is settings-related and may be abstracted to another method
            badBombDrops = badBombDrops.Where(x => x.TeammatesAlive > 0).ToList();

            // Applying (potentially varying) filters
            var collection = new SituationCollection<BadBombDrop>();
            badBombDrops
               .Select(x => x as Misplay).ToList();

            return collection;
        }

        public override ISituationCollection FilterOutByConfig(ISituationCollection collection)
        {
            /// <summary>
            /// BombDrops with fewer alive teammates at bombdrop are ignored. Excluding the player himself
            /// </summary>
            int MinTeammatesAlive = 2;

            /// <summary>
            /// BombDrops with teammates closer than this value are ignored.
            /// </summary>
            int MinDistanceToClosestTeammate = 600;

            /// <summary>
            /// Whether to ignore bombdrops where the bomb was picked up afterwards
            /// </summary>
            bool IgnorePickedUps = true;

            var misplays = collection.Misplays;

            var bombDrops = misplays.Select(x => x as BadBombDrop);

            if (IgnorePickedUps)
            {
                bombDrops = bombDrops.Where(x => !x.WasPickedUp);
            }

            bombDrops = bombDrops
                .Where(x => x.TeammatesAlive >= MinTeammatesAlive)
                .Where(x => x.ClosestTeammateDistance >= MinDistanceToClosestTeammate);

            collection.Misplays = bombDrops.Select(x => x as Misplay).ToList();
            return collection;
        }
    }
}


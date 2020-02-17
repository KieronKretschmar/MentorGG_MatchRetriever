using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public class TeamFlashDetector : Subdetector<TeamFlash>
    {
        public TeamFlashDetector(IServiceProvider sp) : base(sp)
        {
        }

        public override ISituationCollection ComputeMisplays(long steamId, long matchId)
        {
            var flashes = _context.Flash
                .Where(x => x.MatchId == matchId && x.PlayerId == steamId)
                .Select(x => new
                {
                    MatchId = matchId,
                    Map = x.MatchStats.Map,
                    MatchDate = x.MatchStats.MatchDate,
                    Round = x.Round,
                    Time = x.Time,
                    PlayerId = steamId,
                    TeamFlasheds = x.Flashed.Where(y => y.TeamAttack && y.VictimId != steamId && y.AngleToCrosshair < 90).Select(y => new { y.VictimId, y.TimeFlashed })
                })
                .Where(x => x.TeamFlasheds.Count() != 0)
                .ToList();

            var misplays = new List<TeamFlash>();
            foreach (var flash in flashes)
            {
                var misplay = new TeamFlash
                {
                    MatchId = matchId,
                    Map = flash.Map,
                    MatchDate = flash.MatchDate,
                    Round = flash.Round,
                    Time = flash.Time,
                    PlayerId = steamId,
                };

                misplay.TimeFlashed = flash.TeamFlasheds.Select(x => (int?) x.TimeFlashed).Sum() ?? 0;

                misplay.TeammatesFlashed = flash.TeamFlasheds.Count();

                misplay.DiedBlindCount = 0;
                foreach (var flashed in flash.TeamFlasheds)
                {
                    var deathTime = _detectorHelpers.DeathTime(flashed.VictimId, misplay.MatchId, misplay.Round);
                    if (misplay.Time <= deathTime && deathTime < misplay.Time + flashed.TimeFlashed)
                    {
                        misplay.DiedBlindCount++;
                    }
                }

                misplays.Add(misplay);
            }

            var collection = new SituationCollection<TeamFlash>();
            collection.Misplays = misplays
                .Select(x => x as Misplay)
                .ToList();

            return collection;
        }


        public override ISituationCollection FilterOutByConfig(ISituationCollection collection)
        {
            int MinTimeFlashed = 1000;

            var misplays = collection.Misplays.Select(x => x as TeamFlash);

            misplays = misplays
                .Where(x => x.TimeFlashed > MinTimeFlashed);

            collection.Misplays = misplays.Select(x => x as Misplay).ToList();
            return collection;
        }
    }
}

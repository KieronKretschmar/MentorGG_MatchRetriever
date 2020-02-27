using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public interface ISelfFlashDetector
    {
        ISituationCollection ComputeMisplays(long steamId, long matchId);
        ISituationCollection FilterOutByConfig(ISituationCollection collection);
    }

    public class SelfFlashDetector : Subdetector<SelfFlash>, ISelfFlashDetector
    {
        //TODO OPTIONAL Move to configuration database

        private static int MaxAngleToCrosshair = 90;

        public SelfFlashDetector(IServiceProvider sp) : base(sp)
        {

        }

        public override ISituationCollection ComputeMisplays(long steamId, long matchId)
        {


            var data = _context.Flash
                .Where(x => x.MatchId == matchId && x.PlayerId == steamId)
                .Select(x => new
                {
                    MatchId = matchId,
                    Map = x.MatchStats.Map,
                    MatchDate = x.MatchStats.MatchDate,
                    Round = x.Round,
                    Time = x.Time,
                    PlayerId = steamId,
                    Flashed = x.Flashed.Select(y => new { y.TimeFlashed, y.AngleToCrosshair, y.VictimId }).SingleOrDefault(y => y.VictimId == steamId)
                })
                .ToList();

            var flashes = data
                .Select(x => new SelfFlash
                {
                    MatchId = matchId,
                    Map = x.Map,
                    MatchDate = x.MatchDate,
                    Round = x.Round,
                    Time = x.Time,
                    PlayerId = steamId,
                    TimeFlashed = x.Flashed?.TimeFlashed ?? 0,
                    AngleToCrosshair = x.Flashed?.AngleToCrosshair ?? 0,
                    DeathTime = _detectorHelpers.DeathTime(steamId, matchId, x.Round)
                })
                .Where(x => x.TimeFlashed != 0 && x.AngleToCrosshair < MaxAngleToCrosshair)
                .ToList();



            var collection = new SituationCollection<SelfFlash>();
            collection.Misplays = flashes
                    .Select(x => x as Misplay)
                    .ToList();

            return collection;

        }


        public override ISituationCollection FilterOutByConfig(ISituationCollection collection)
        {
            int MinTimeFlashed = 500;

            var misplays = collection.Misplays.Select(x => x as SelfFlash);

            misplays = misplays
                .Where(x => x.TimeFlashed > MinTimeFlashed);

            collection.Misplays = misplays.Select(x => x as Misplay).ToList();
            return collection;
        }
    }
}

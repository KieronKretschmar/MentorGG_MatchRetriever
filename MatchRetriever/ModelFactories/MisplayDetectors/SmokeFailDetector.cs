using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using ZoneReader;

namespace MatchRetriever.Misplays
{
    public interface ISmokeFailDetector
    {
        ISituationCollection ComputeMisplays(long steamId, long matchId);
        ISituationCollection FilterOutByConfig(ISituationCollection misplays);
    }

    public class SmokeFailDetector : Subdetector<SmokeFail>, ISmokeFailDetector
    {
        private IZoneReader _zoneReader;

        public SmokeFailDetector(IServiceProvider sp) : base(sp)
        {
            _zoneReader = sp.GetRequiredService<IZoneReader>();
        }

        public override ISituationCollection ComputeMisplays(long steamId, long matchId)
        {
            var map = _context.MatchStats.Single(x => x.MatchId == matchId).Map;

            //TODO OPTIMIZATION Fix zone loading for each match
            var smokeZones = _zoneReader.GetLineups(ZoneReader.Enums.LineupType.Smoke, Enum.Parse<ZoneReader.Enums.Map>(map, true));
            var lineups = smokeZones.Lineups;

            var failedSmokes = _context.Smoke
                .Where(x => x.MatchId == matchId && x.PlayerId == steamId && x.Result == MatchEntities.Enums.TargetResult.Miss)
                .Select(x => new SmokeFail
                {
                    MatchId = matchId,
                    Map = x.MatchStats.Map,
                    MatchDate = x.MatchStats.MatchDate,
                    Round = x.Round,
                    Time = x.Time,
                    PlayerId = steamId,
                    LineupId = x.LineUp,
                    LineupName = lineups[x.LineUp].Name
                })
                .Select(x => x as Misplay)
                .ToList();

            SituationCollection<SmokeFail> situationCollection = new SituationCollection<SmokeFail>();
            situationCollection.Misplays = failedSmokes;
            return situationCollection;
        }



        public override ISituationCollection FilterOutByConfig(ISituationCollection misplays)
        {
            return misplays;
        }
    }
}

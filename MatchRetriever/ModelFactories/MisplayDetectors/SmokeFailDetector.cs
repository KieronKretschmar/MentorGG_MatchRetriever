using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private ILogger<SmokeFailDetector> _logger;

        public SmokeFailDetector(IServiceProvider sp) : base(sp)
        {
            _zoneReader = sp.GetRequiredService<IZoneReader>();
            _logger = sp.GetRequiredService<ILogger<SmokeFailDetector>>();
        }

        public override ISituationCollection ComputeMisplays(long steamId, long matchId)
        {
            var map = _context.MatchStats.Single(x => x.MatchId == matchId).Map;

            //If the map could not be parsed to the enum, return an empty collection.
            //This is the case if no lineup or zones are defined for a map
            if (!Enum.TryParse(map, true, out ZoneReader.Enums.Map mapFromEnum))
            {
                _logger.LogInformation($"Received SmokeFailDetector request for map {map} and match {matchId}. No zones are defined for this map," +
                    $"\n returning empty misplay colllection");
                return new SituationCollection<SmokeFail>();
            }

            //TODO OPTIMIZATION Fix zone loading for each match
            var smokeZones = _zoneReader.GetLineups(ZoneReader.Enums.LineupType.Smoke, mapFromEnum);
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

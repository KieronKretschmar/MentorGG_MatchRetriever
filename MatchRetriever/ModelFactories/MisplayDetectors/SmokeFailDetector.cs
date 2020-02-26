﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using ZoneReader;

namespace MatchRetriever.Misplays
{
    public class SmokeFailDetector : Subdetector<SmokeFail>
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
            var smokeZones = _zoneReader.GetSmokeZones(Enum.Parse<ZoneReader.Enums.ZoneMap>(map, true));
            var idLineups = smokeZones.IdLineUps;

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
                    LineupName = idLineups[x.LineUp].Name
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

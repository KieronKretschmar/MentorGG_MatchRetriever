using Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatchRetriever.Misplays
{
    public class DetectorHelpers
    {
        private MatchContext _context;

        public DetectorHelpers(IServiceProvider sp)
        {
            _context = sp.GetRequiredService<MatchContext>();
        }

        public List<float> TeammateDistanceToPos(long matchId, short round, int time, List<long> teammateSteamIds, Vector3 pos)
        {
            var teammatePositionsAtBombDrop = _context.PlayerPosition
                .Where(x =>
                teammateSteamIds.Contains(x.PlayerId)
                && x.MatchId == matchId
                && x.Round == round
                //TODO OPTIMIZATION check if == works
                && x.Time > time)
                .Select(x => new
                {
                    x.PlayerId,
                    x.Time,
                    x.PlayerPos
                })
                .ToList()
                .GroupBy(x => x.PlayerId)
                .Select(x => x.OrderBy(y => y.Time).First())
                .ToList();

            return teammatePositionsAtBombDrop
                .Select(x => Vector3.Distance(x.PlayerPos, pos))
                .ToList();

        }
    }
}
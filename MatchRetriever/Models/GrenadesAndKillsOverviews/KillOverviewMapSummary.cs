using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Database;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Models.GrenadesAndKillsOverviews
{
    public class KillOverviewMapSummary : IOverviewMapSummary
    {
        public string Map { get; set; }
        public string Name { get; set; } = "NAME";

        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }

        public int RoundsWonAsCt { get; set; }
        public int RoundsLostAsCt { get; set; }
        public int KillsAsCt { get; set; }
        public int DeathsAsCt { get; set; }

        public int RoundsWonAsTerrorist { get; set; }
        public int RoundsLostAsTerrorist { get; set; }
        public int KillsAsTerrorist { get; set; }
        public int DeathsAsTerrorist { get; set; }

        public double MatchWinFraction { get { return (double)MatchesWon / Math.Max(1, MatchesWon + MatchesLost); } }
        public double RoundWinFractionAsCt { get { return (double)RoundsWonAsCt / Math.Max(1, RoundsWonAsCt + RoundsLostAsCt); } }
        public double RoundWinFractionAsTerrorist { get { return (double)RoundsWonAsTerrorist / Math.Max(1, RoundsWonAsTerrorist + RoundsLostAsTerrorist); } }
        public double KDAsTerrorist { get { return (double)KillsAsTerrorist / Math.Max(1, DeathsAsTerrorist); } }
        public double KDAsCt { get { return (double)KillsAsCt / Math.Max(1, DeathsAsCt); } }
        public double KillChanceAsTerrorist { get { return (double)KillsAsTerrorist / Math.Max(1, KillsAsTerrorist + DeathsAsTerrorist); } }
        public double KillChanceAsCt { get { return (double)KillsAsCt / Math.Max(1, KillsAsCt + DeathsAsCt); } }
    }
}
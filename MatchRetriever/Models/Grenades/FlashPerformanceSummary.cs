using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Database;
using MatchRetriever.Helpers;

namespace MatchRetriever.Models.Grenades
{
    public class FlashPerformanceSummary : IPerformanceSummary
    {
        public string Map { get; set; }

        public int RoundsAsTerrorist { get; set; }
        public int BuysAsTerrorist { get; set; }
        public int UsagesAsTerrorist { get; set; }
        public int EnemiesFlashedAsTerrorist { get; set; }
        public int EnemiesTimeFlashedAsTerrorist { get; set; }
        public int KillAssistsAsTerrorist { get; set; }
        public int RoundsAsCt { get; set; }
        public int BuysAsCt { get; set; }
        public int UsagesAsCt { get; set; }
        public int EnemiesFlashedAsCt { get; set; }
        public int EnemiesTimeFlashedAsCt { get; set; }
        public int KillAssistsAsCt { get; set; }

        //public double BuyRatioAsTerrorist { get { return (double)BuysAsTerrorist / Math.Max(1, RoundsAsTerrorist); } }
        //public double BuyRatioAsCt { get { return (double)BuysAsCt / Math.Max(1, RoundsAsCt); } }
        public double UsageRatioAsTerrorist { get { return (double)UsagesAsTerrorist / Math.Max(1, BuysAsTerrorist); } }
        public double UsageRatioAsCt { get { return (double)UsagesAsCt / Math.Max(1, BuysAsCt); } }
        public double AverageEnemiesFlashedAsTerrorist { get { return (double)EnemiesFlashedAsTerrorist / Math.Max(1, UsagesAsTerrorist); } }
        public double AverageEnemiesFlashedAsCt { get { return (double)EnemiesFlashedAsCt / Math.Max(1, UsagesAsCt); } }
        public double AverageEnemiesTimeFlashedAsTerrorist { get { return (double)EnemiesTimeFlashedAsTerrorist / Math.Max(1, UsagesAsTerrorist); } }
        public double AverageEnemiesTimeFlashedAsCt { get { return (double)EnemiesTimeFlashedAsCt / Math.Max(1, UsagesAsCt); } }
        public double KillAssistChanceAsTerrorist { get { return (double)KillAssistsAsTerrorist / Math.Max(1, UsagesAsTerrorist); } }
        public double KillAssistChanceAsCt { get { return (double)KillAssistsAsCt / Math.Max(1, UsagesAsCt); } }
    }
}
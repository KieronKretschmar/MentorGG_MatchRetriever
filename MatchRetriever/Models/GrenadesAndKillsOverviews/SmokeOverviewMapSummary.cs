using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Database;
using MatchRetriever.Helpers;

namespace MatchRetriever.Models.GrenadesAndKillsOverviews
{
    public class SmokeOverviewMapSummary : IOverviewMapSummary
    {
        public string Map { get; set; } //TODO: Remove or set value

        public int TotalCategories { get; set; }
        public int CompletedCategories { get; set; }
        //public int ReliableCategories { get; set; }

        public double CategorizedSmokesAccuracy => (double)SuccessfulLineupAttempts / (SuccessfulLineupAttempts + FailedLineupAttempts);

        public int SuccessfulLineupAttempts { get; set; }
        public int FailedLineupAttempts { get; set; }

        //public int TotalTargets { get; set; }
        //public int CompletedTargets { get; set; }        

        public int BuysAsTerrorist { get; set; }
        public int UsagesAsTerrorist { get; set; }

        public int BuysAsCt { get; set; }
        public int UsagesAsCt { get; set; }

        public double UsageRatioAsTerrorist { get { return (double)UsagesAsTerrorist / Math.Max(1, BuysAsTerrorist); } }
        public double UsageRatioAsCt { get { return (double)UsagesAsCt / Math.Max(1, BuysAsCt); } }
    }
}
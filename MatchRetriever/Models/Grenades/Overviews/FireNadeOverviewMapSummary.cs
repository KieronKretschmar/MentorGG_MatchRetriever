using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Database;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Models.Grenades
{
    public class FireNadeOverviewMapSummary : IOverviewSummary
    {
        public string Map { get; set; }

        public int RoundsAsTerrorist { get; set; }
        public int BuysAsTerrorist { get; set; }
        public int UsagesAsTerrorist { get; set; }
        public int HitsAsTerrorist { get; set; }
        public int DamageAsTerrorist { get; set; }
        public int KillsAsTerrorist { get; set; }

        public int RoundsAsCt { get; set; }
        public int BuysAsCt { get; set; }
        public int UsagesAsCt { get; set; }
        public int HitsAsCt { get; set; }
        public int DamageAsCt { get; set; }
        public int KillsAsCt { get; set; }

        //public double BuyRatioAsTerrorist { get { return (double)BuysAsTerrorist / Math.Max(1, RoundsAsTerrorist); } }
        //public double BuyRatioAsCt { get { return (double)BuysAsCt / Math.Max(1, RoundsAsCt); } }
        public double UsageRatioAsTerrorist { get { return (double)UsagesAsTerrorist / Math.Max(1, BuysAsTerrorist); } }
        public double UsageRatioAsCt { get { return (double)UsagesAsCt / Math.Max(1, BuysAsCt); } }
        public double HitChanceAsTerrorist { get { return (double)HitsAsTerrorist / Math.Max(1, UsagesAsTerrorist); } }
        public double HitChanceAsCt { get { return (double)HitsAsCt / Math.Max(1, UsagesAsCt); } }
        public double AverageDamageAsTerrorist { get { return (double)DamageAsTerrorist / Math.Max(1, UsagesAsTerrorist); } }
        public double AverageDamageAsCt { get { return (double)DamageAsCt / Math.Max(1, UsagesAsCt); } }
        public double KillChanceAsTerrorist { get { return (double)KillsAsTerrorist / Math.Max(1, UsagesAsTerrorist); } }
        public double KillChanceAsCt { get { return (double)KillsAsCt / Math.Max(1, UsagesAsCt); } }
    }
}
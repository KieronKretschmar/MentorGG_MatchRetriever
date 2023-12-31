﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKillsOverviews
{
    /// <summary>
    /// Interface for storing data relating to the performance of a player regarding a grenadetype or kills on a particular map.
    /// </summary>
    public interface IOverviewMapSummary
    {
        public string Map { get; set; }
    }
}

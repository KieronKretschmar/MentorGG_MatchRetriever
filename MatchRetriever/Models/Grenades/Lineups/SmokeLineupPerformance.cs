﻿using System;

namespace MatchRetriever.Models.Grenades
{
    public class SmokeLineupPerformance : ILineupPerformance
    {
        public int CategoryId { get; set; }
        public int Attempts => Misses + Insides;
        public int Misses { get; set; }
        public int Insides { get; set; }
    }
}
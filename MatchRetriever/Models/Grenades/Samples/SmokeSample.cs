﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{

    public class SmokeSample : GrenadeSample
    {
        public string Id => "Smoke" + "-" + MatchId + "-" + GrenadeId;
        //public bool UserWonRound { get; set; }
        //public byte PlantSite { get; set; }

        public int Result { get; set; }
        public int TargetId { get; set; }
        public int LineupId { get; set; }

        public double PlayerViewX { get; set; }
    }
}

﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database;
using MatchRetriever.Models.Grenades;
using System.Numerics;

namespace MatchRetriever.Models.Grenades
{
    public class FlashSample : GrenadeSample
    {
        public string Id => "Flash" + "-" + MatchId + "-" + GrenadeId;
        public int ZoneId { get; set; }

        public List<Flashed> Flasheds { get; set; }
        public struct Flashed
        {
            public Vector3 VictimPos { get; set; }
            public int TimeFlashed { get; set; }
            public bool FlashAssist { get; set; }
            public bool TeamAttack { get; set; }
            public bool VictimIsAttacker { get; set; }
        }
    }
}

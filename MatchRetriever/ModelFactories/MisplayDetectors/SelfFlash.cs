using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public class SelfFlash : Misplay
    {
        public int TimeFlashed { get; set; }
        public int DeathTime { get;  set; }
        public int AngleToCrosshair { get; set; }
    }
}

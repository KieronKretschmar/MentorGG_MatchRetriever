using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public class TeamFlash : Misplay
    {
        public int TeammatesFlashed { get; set; }
        public int TimeFlashed { get; set; }
        public int DiedBlindCount { get; set; }
    }
}

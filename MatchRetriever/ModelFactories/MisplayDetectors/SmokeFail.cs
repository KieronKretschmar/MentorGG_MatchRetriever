using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public class SmokeFail : Misplay
    {
        public int LineupId { get; internal set; }
        public string LineupName { get; internal set; }
    }
}

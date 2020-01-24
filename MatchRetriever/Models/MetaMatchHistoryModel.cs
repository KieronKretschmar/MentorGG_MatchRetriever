using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models
{
    public class MetaMatchHistoryModel
    {
        public List<Match> Matches{ get; set; }
        public struct Match
        {
            public long MatchId { get; set; }
            //public Source Source { get; set; } //TODO: Source enum
            public string Map { get; set; }
            public DateTime MatchDate { get; set; }
        }
    }
}

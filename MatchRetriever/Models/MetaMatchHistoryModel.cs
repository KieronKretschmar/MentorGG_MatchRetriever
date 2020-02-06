using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities.Enums;

namespace MatchRetriever.Models
{
    public class MatchSelectionModel
    {
        public List<Match> Matches{ get; set; }
        public struct Match
        {
            public long MatchId { get; set; }
            public Source Source { get; set; } 
            public string Map { get; set; }
            public DateTime MatchDate { get; set; }
        }
    }
}

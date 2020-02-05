using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models
{
    public class PlayerInfoModel
    {
        public string SteamName { get; set; }
        public string FaceitName { get; set; }
        public string IconPath { get; set; }
        public byte Rank { get; set; }

        public string RankIcon
        {
            get
            {
                var prefixedRank = Rank > 9 ? Rank.ToString() : "0" + Rank;
                return "~/Content/Images/Ranks/" + prefixedRank + ".png";
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models
{
    public class PlayerSummaryModel
    {
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int GamesTied { get; set; }
        public double AverageHltvRating { get; set; }
        public long Kills { get; set; }
        public long HsKills { get; set; }
        public long Deaths { get; set; }

        // MatchmakingRank related
        public int GamesWonAfterRankChange { get; set; }
        public int GamesLostAfterRankChange { get; set; }
        public int GamesTiedAfterRankChange { get; set; }
        public bool RecentRankChangeWasUprank { get; set; }
        public List<int> MatchMakingResults { get; set; }
        public List<int> MatchMakingResultsAfterRankChange { get; set; }

        public int GamesTotal => GamesWon + GamesTied + GamesLost;

        /// <summary>
        /// W/L, with draws counting as half a win
        /// </summary>
        /// <returns></returns>
        public float MatchWinChance => (float)(GamesWon + 0.5 * GamesTied) / Math.Max(1, GamesTotal);


        /// <summary>
        /// W/L, with draws counting as half a win
        /// </summary>
        /// <returns></returns>
        public float KillDeathRatio => (float)(Kills) / Math.Max(1, Deaths);
    }
}

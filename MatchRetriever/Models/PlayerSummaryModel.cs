using MatchEntities.Enums;
using MatchRetriever.Enumerals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models
{
    public class PlayerSummaryModel
    {
        /// <summary>
        /// List of all the users matches in the database, ordered by MatchDate.
        /// </summary>
        public List<PlayerMatchSummary> Matches { get; set; }
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



        public class PlayerMatchSummary
        {
            public DateTime MatchDate { get; set; }
            public Source Source { get; set; }
            public string Map { get; set; }
            public MatchMakingRank RankBeforeMatch { get; set; }
            public MatchMakingRank RankAfterMatch { get; set; }
            public float HltvRating1 { get; set; }
            public short Deaths { get; set; }
            public short Kills { get; set; }
            public short HsKills { get; set; }
            public WinTieLose PlayerMatchOutcome { get; set; }
        }
    }
}

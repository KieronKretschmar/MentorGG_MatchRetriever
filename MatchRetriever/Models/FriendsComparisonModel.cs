using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models
{
    public class FriendsComparisonModel
    {
        public List<ComparisonRowData> Comparisons { get; set; }
    }

    #region FriendDescription classes
    public struct ComparisonRowData
    {
        public PlayerInfoModel OtherPlayerInfo { get; set; }

        public BriefComparisonPlayerData UserData { get; set; }
        public BriefComparisonPlayerData OtherData { get; set; }

        // Data that is equal for both players
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MatchesTied { get; set; }

        public string MostPlayedMap { get; set; }
        public int MostPlayedMapMatchesPlayed { get; set; }
        public int MostPlayedMapMatchesWon { get; set; }
        public int MostPlayedMapMatchesLost { get; set; }
        public int MostPlayedMapMatchesTied { get; set; }

        public int CtRounds { get; set; }
        public int CtRoundsWon { get; set; }
        public int TerroristRounds { get; set; }
        public int TerroristRoundsWon { get; set; }
    }


    /// <summary>
    /// Contains stats for each player, mostly regardless of CT/T side.
    /// </summary>
    public class BriefComparisonPlayerData
    {
        public int Score { get; set; }
        public int MVPs { get; set; }

        public int TerroristKills { get; set; }
        public int TerroristDeaths { get; set; }

        public int CtKills { get; set; }
        public int CtDeaths { get; set; }

        public int HSKills { get; set; }

        public int CtDamage { get; set; }
        public int TerroristDamage { get; set; }

        public int FlashesThrown { get; set; }
        public int EnemiesFlashed { get; set; }
        public int TeammatesFlashed { get; set; } // Does not include selfflashes
        public int FireNadesThrown { get; set; }
        public int HEsThrown { get; set; }
        public int SmokesThrown { get; set; }


    }

    public struct FriendsHistory
    {
        public long PlayerId { get; set; }
        public long OtherSteamId { get; set; }
        public List<long> MatchIds { get; set; }

        public FriendsHistory(long id, long friendId, List<long> matchIds)
        {
            this.PlayerId = id;
            this.OtherSteamId = friendId;
            this.MatchIds = matchIds;
        }
    }
    #endregion

}

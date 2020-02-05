using MatchEntities.Enums;
using MatchRetriever.Helpers;

namespace MatchRetriever.Models.DemoViewer
{
    public class DvPlayerScoreboardEntry
    {
        public SteamUserOperator.SteamUser Profile { get; set; }
        public StartingFaction Team { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int DamageDealt { get; set; }
        public short Score { get; set; }
        public short MVPs { get; set; }
        public MatchMakingRank RankBeforeMatch { get; set; }
        public MatchMakingRank RankAfterMatch { get; set; }

        public bool WasBannedAfterMatch { get; set; }
    }
}
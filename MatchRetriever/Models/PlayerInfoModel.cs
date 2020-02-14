using MatchEntities.Enums;
using static MatchRetriever.Helpers.SteamUserOperator;

namespace MatchRetriever.Models
{
    public struct PlayerInfoModel
    {
        public SteamUser SteamUser { get; set; }
        public MatchMakingRank Rank { get; set; }
    }
}

using MatchEntities.Enums;
using static MatchRetriever.Helpers.SteamUserOperator;

namespace MatchRetriever.Models
{
    public struct PlayerInfoModel
    {
        public SteamUser steamUser { get; set; }
        public MatchMakingRank Rank { get; set; }
    }
}

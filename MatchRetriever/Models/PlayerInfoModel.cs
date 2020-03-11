using MatchEntities.Enums;
using MatchRetriever.Helpers;

namespace MatchRetriever.Models
{
    public struct PlayerInfoModel
    {
        public SteamUser SteamUser { get; set; }
        public MatchMakingRank Rank { get; set; }
    }
}

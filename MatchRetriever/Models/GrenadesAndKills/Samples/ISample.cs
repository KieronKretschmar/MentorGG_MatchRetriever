using MatchRetriever.Helpers.Trajectories;
using System.Collections.Generic;
using System.Numerics;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds data about an event that can be drawn on a radar image (e.g. grenades and kills)
    /// </summary>
    public interface ISample
    {
        long PlayerId { get; set; }
        string PlayerName { get; set; }
        long MatchId { get; set; }
        short Round { get; set; }
        bool UserIsCt { get; set; }
    }
}

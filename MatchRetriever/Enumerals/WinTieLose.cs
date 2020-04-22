using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Enumerals
{    
    /// <summary>
    /// The outcome of a match from a player's perspective.
    /// </summary>
    public enum WinTieLose : byte
    {
        Win = 1,
        Tie = 2,
        Lose = 3,
    }
}

using MatchRetriever.Models;

namespace MatchRetriever.Misplays
{
    public class BadBombDrop : Misplay
    {
        public int PickedUpAfter { get; set; }
        public int TeammatesAlive { get; internal set; }
        public int ClosestTeammateDistance { get; internal set; }
        
        public bool WasPickedUp => PickedUpAfter > 0;
    }
}
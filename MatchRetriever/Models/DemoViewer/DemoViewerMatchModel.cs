using EquipmentLib;
using MatchRetriever.Models.DemoViewer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.DemoViewer
{
    public class DemoViewerMatchModel
    {
        //Global Stats
        public Dictionary<short, string> WeaponFileNames = new Dictionary<short, string>();
        public Dictionary<short, EquipmentInfo> EquipmentInfo;
        public Dictionary<string, Helpers.SteamUser> PlayerStats;
        public List<List<long>> FriendNets;
        public long? UserId;

        // Match Stats
        public DvMatchStats MatchStats;
        public DvOverTimeStats OverTimeStats;
        public Dictionary<string, DvPlayerMatchStats> PlayerMatchStats;
        public List<RoundSummary> RoundSummaries;
        public string ImageUrl;

        public struct RoundSummary
        {
            public short Round { get; set; }
            public byte WinnerTeam { get; set; }
            public byte WinType { get; set; }
            public bool WinnerIsCT { get; set; }
        }
    }
}

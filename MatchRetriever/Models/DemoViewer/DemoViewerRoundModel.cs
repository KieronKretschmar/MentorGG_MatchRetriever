using MatchRetriever.Enumerals;
using MatchRetriever.Helpers;
using MatchRetriever.Models.DemoViewer.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchRetriever.Models.DemoViewer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DemoViewerRoundModel
    {
        public DemoViewerConfig Config { get; set; }

        // Match Stats
        public DvRoundStats RoundStats;

        public List<DvPlayerRoundStats> PlayerRoundStats;

        public List<DvOverTimeStats> OverTimeStats;
        
        public DvScoreboard PartialScoreboard;

        public List<DvBotTakeOver> BotTakeOvers;


        // Gun related Match Stats
        public List<DvDamage> Damages;

        public List<DvWeaponFired> WeaponFireds;

        public Dictionary<string, List<DvWeaponReload>> WeaponReloads;

        public List<DvKill> Kills;

        // Other Match Stats
        public DvBombPlant BombPlant;

        public DvBombDefused BombDefused;

        public DvBombExplosion BombExplosion;

        public Dictionary<string, List<DvItemSaved>> ItemSaveds;

        public Dictionary<string, List<DvItemDropped>> ItemDroppeds;

        public Dictionary<string, List<DvItemPickedUp>> ItemPickedUps;

        public Dictionary<string, List<DvPlayerPosition>> PlayerPositions;

        // Grenades
        public List<DvFlash> Flashes;

        public Dictionary<string, List<DvFlashed>> Flasheds;

        public List<DvHE> HEs;

        public List<DvSmoke> Smokes;

        public List<DvDecoy> Decoys;

        public List<DvFireNade> FireNades;

    }

}
using MatchEntities;
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
    public class DemoViewerRoundModel
    {
        public DemoViewerConfig Config { get; set; }

        // Match Stats
        public DvRoundStats RoundStats { get; set; }

        public List<DvPlayerRoundStats> PlayerRoundStats { get; set; }

        public List<DvOverTimeStats> OverTimeStats { get; set; }
        
        public DvScoreboard PartialScoreboard { get; set; }

        public List<DvBotTakeOver> BotTakeOvers { get; set; }


        // Gun related Match Stats
        public List<DvDamage> Damages { get; set; }

        public List<DvWeaponFired> WeaponFireds { get; set; }

        public Dictionary<string, List<DvWeaponReload>> WeaponReloads { get; set; }

        public List<DvKill> Kills { get; set; }

        // Other Match Stats
        public List<DroppedBombPosition> DroppedBombPositions { get; set; }

        public List<BombPlant> BombPlants { get; set; }

        public List<BombDefused> BombDefuseds { get; set; }

        public BombExplosion BombExplosion { get; set; }

        public Dictionary<string, List<DvItemSaved>> ItemSaveds { get; set; }

        public Dictionary<string, List<DvItemDropped>> ItemDroppeds { get; set; }

        public Dictionary<string, List<DvItemPickedUp>> ItemPickedUps { get; set; }

        public Dictionary<string, List<DvPlayerPosition>> PlayerPositions { get; set; }

        public Dictionary<string, List<PlayerJump>> PlayerJumps { get; set; }

        // Grenades
        public List<DvFlash> Flashes { get; set; }

        public Dictionary<string, List<DvFlashed>> Flasheds { get; set; }

        public List<DvHE> HEs { get; set; }

        public List<DvSmoke> Smokes { get; set; }

        public List<DvDecoy> Decoys { get; set; }

        public List<DvFireNade> FireNades { get; set; }

        public struct DemoViewerConfig
        {
            public DemoViewerQuality Quality { get; set; }
            public double FramesPerSecond { get; set; }
        }
    }
}
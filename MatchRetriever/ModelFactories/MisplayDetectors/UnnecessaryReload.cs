using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public class UnnecessaryReload : Misplay
    {
        public int ReloadStartTime { get; set; }
        public EquipmentElement Weapon { get; set; }
        public string WeaponAsString => Weapon.ToString();
        public int AmmoBefore { get; set; }
        public int DeathTime { get;  set; }
        public int TimeToNextDamageTaken { get;  set; } // Measured from start of reload animation
        public bool WasFlashed { get;  set; }
        //public void SetProps()
        //{
        //    WasFlashed = DetectorHelpers.WasFlashed(PlayerId, MatchId, Round, ReloadStartTime);
        //    var nextDamageTaken = DetectorHelpers.NextDamageTaken(PlayerId, MatchId, Round, ReloadStartTime, ReloadStartTime + MaxTimeBeforeDamageTakenLimit);
        //    TimeToNextDamageTaken = nextDamageTaken != -1 ? nextDamageTaken - ReloadStartTime : -1;
        //}
    }
}

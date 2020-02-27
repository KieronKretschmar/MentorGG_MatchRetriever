using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities.Enums;

namespace MatchRetriever.Misplays
{
    public class ShotWhileMoving : Misplay
    {
        public EquipmentElement Weapon { get; set; }
        public string WeaponAsString => Weapon.ToString();
        public int Bullets { get; set; }
        public int InaccurateBullets { get; set; }
        public int LastBulletTime { get; set; }
        public int DeathTime { get; set; }
        public int TimeToNextFightingAction { get; set; }
    }
}

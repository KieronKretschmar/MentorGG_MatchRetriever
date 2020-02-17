using EquipmentLib;
using MatchEntities.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public class ShotWhileMovingDetector : Subdetector<ShotWhileMoving>
    {
        public ShotWhileMovingDetector(IServiceProvider sp) : base(sp)
        {
            _eqipmentProvider = sp.GetRequiredService<IEquipmentProvider>();
        }

        //TODO OPTIONAL move into config database
        //This stuff does bnot belong here but needs to be here for it to work right now
        private static List<EquipmentElement> Rifles => new List<EquipmentElement> {
                EquipmentElement.Famas,
                EquipmentElement.Gallil,
                EquipmentElement.AK47,
                EquipmentElement.M4A1,
                EquipmentElement.M4A4,
                EquipmentElement.SG556,
                EquipmentElement.AUG,
                EquipmentElement.Scar20,
                EquipmentElement.G3SG1,
                //AWP currently dows not count as we track min shots fired for a burst
                //AWP cant fire enough shots to accoutn for a burst
                //EquipmentElement.AWP,
        };
        private static int MinShots = 5;
        private static int MaxBurstTimeDelta = 200;
        private static int MaxTimeForFightingLimit = 5000;

        private IEquipmentProvider _eqipmentProvider;

        public override ISituationCollection ComputeMisplays(long steamId, long matchId)
        {

            var rifleShots = _context.WeaponFired
                .Where(x => x.MatchId == matchId && x.PlayerId == steamId && Rifles.Contains(x.Weapon))
                .Select(x => new WeaponFired
                {
                    Round = x.Round,
                    Time = x.Time,
                    Weapon = x.Weapon,
                    PlayerVelocity = x.PlayerVelo,
                })
                .ToList();

            var equipmentDict = _eqipmentProvider.GetEquipmentDict(EquipmentLib.Enums.Source.Valve, DateTime.Now);
            rifleShots.ForEach(x =>
            {
                var maxAccurateVelocity = equipmentDict[(short) x.Weapon].MaxPlayerSpeed * (1 / 3);
                x.InaccurateFromMoving = x.Velocity > maxAccurateVelocity;
            });

            // Group WeaponFired's into bursts
            var bursts = DivideIntoBursts(rifleShots, MinShots, MaxBurstTimeDelta);

            // Load matchInfos
            var matchInfos = bursts
                .Select(x => matchId)
                .Distinct()
                .ToDictionary(x => x,
                x => _context.MatchStats.Select(ms => new { ms.MatchId, ms.Map, ms.MatchDate }).Single(ms => ms.MatchId == matchId));

            // Convert bursts to misplays
            var misplays = bursts
                .Select(x => new ShotWhileMoving
                {
                    MatchId = matchId,
                    MatchDate = matchInfos[matchId].MatchDate,
                    Map = matchInfos[matchId].Map,
                    PlayerId = steamId,
                    Round = x.Round,
                    Time = x.FirstBulletTime,
                    LastBulletTime = x.LastBulletTime,

                    Weapon = x.Weapon,
                    Bullets = x.WeaponFireds.Count,
                    InaccurateBullets = x.WeaponFireds.Count(y => y.InaccurateFromMoving),

                }).ToList();

            //TODO OPTIMIZATION calculate only once per round instead of per misplay
            misplays.ForEach(x =>
            {
                x.DeathTime = _detectorHelpers.DeathTime(steamId, matchId, x.Round);
                int nextFightingAction = _detectorHelpers.NextFightingAction(steamId,matchId,x.Round,x.Time,x.Time + MaxTimeForFightingLimit );
                x.TimeToNextFightingAction = nextFightingAction != -1 ? nextFightingAction - x.Time : -1;
            });


            var collection = new SituationCollection<ShotWhileMoving>();
            collection.Misplays = misplays
                .Select(x => x as Misplay)
                .ToList();
            return collection;
        }

        private List<Burst> DivideIntoBursts(List<WeaponFired> weaponFireds, int minShots, int maxTime)
        {
            var bursts = new List<Burst>();
            var weaponFiredsByRound = weaponFireds.GroupBy(x => x.Round);

            foreach (var weaponFiredInRound in weaponFiredsByRound)
            {
                foreach (var wf in weaponFiredInRound.OrderBy(x => x.Time))
                {
                    // If there are no bursts or assigning this wf to the previous burst does not work, start a new burst. 
                    if (bursts.Count() == 0 || !bursts.Last().TryAdd(wf, maxTime))
                    {
                        bursts.Add(new Burst(wf));
                        continue;
                    }
                }
            }

            return bursts.Where(x => x.WeaponFireds.Count >= minShots).ToList();
        }

        public override ISituationCollection FilterOutByConfig(ISituationCollection collection)
        {
            // Minimum fraction of inaccurate shots in a burst so that the burst counts as inaccurate and a misplay
            double MinInaccurateShots = 1;

            // Timespan in ms after the last bullet within which the player must have died to count as "DiedShortlyAfter"
            int MaxDiedAfterTime = -1;

            // Ignore misplays where the user did not fight within this value before / after after misplay
            int MaxTimeBeforeFight = 2000; // -1 to ignore this condition

            var misplays = collection.Misplays.Select(x => x as ShotWhileMoving);

            // Filter misplays
            misplays = misplays
                .Where(x => (double) x.InaccurateBullets / x.Bullets > MinInaccurateShots).ToList();

            if (MaxDiedAfterTime != -1)
            {
                misplays
                    .Where(x => x.DeathTime != -1 && x.DeathTime < MaxDiedAfterTime)
                    .ToList();
            }

            if (MaxTimeBeforeFight != -1)
            {
                misplays
                    .Where(x => x.TimeToNextFightingAction != -1 && x.TimeToNextFightingAction < MaxTimeBeforeFight)
                    .ToList();
            }

            collection.Misplays = misplays.Select(x => x as Misplay).ToList();
            return collection;
        }


        #region private classes
        private class WeaponFired
        {
            public Vector3 PlayerVelocity { get; internal set; }
            public EquipmentElement Weapon { get; internal set; }
            public int Time { get; internal set; }
            public short Round { get; internal set; }
            public long MatchId { get; internal set; }

            public float Velocity => PlayerVelocity.Length();

            public bool InaccurateFromMoving { get; internal set; }
        }
        private class Burst
        {
            public short Round { get; set; }
            public EquipmentElement Weapon { get; set; }

            public List<WeaponFired> WeaponFireds { get; set; } = new List<WeaponFired>();

            public int FirstBulletTime => WeaponFireds.First().Time;
            public int LastBulletTime => WeaponFireds.Last().Time;

            public double InaccurateFraction => (double) WeaponFireds.Count(x => x.InaccurateFromMoving) / WeaponFireds.Count;

            public Burst(WeaponFired weaponFired)
            {
                Round = weaponFired.Round;
                Weapon = weaponFired.Weapon;
                WeaponFireds.Add(weaponFired);
            }

            public bool TryAdd(WeaponFired wf, int maxTime)
            {

                if (wf.Time < LastBulletTime)
                {
                    throw new ArgumentException("WeaponFireds have to be added in chronological order.");
                }

                var belongsToThisBurst =
                    wf.Round == Round
                    && wf.Weapon == Weapon
                    && LastBulletTime + maxTime >= wf.Time;

                if (belongsToThisBurst)
                {
                    WeaponFireds.Add(wf);
                    return true;
                }

                return false;
            }
        }
        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Misplays
{
    public class UnnecessaryReloadDetector : Subdetector<UnnecessaryReload>
    {
        //TODO Move into configurration database
        public static int MinAmmoBeforeLimit = 5;

        // It will not be possible to set MaxTimebeforeFight higher than this.
        private static int MaxTimeBeforeDamageTakenLimit = 2000;

        public UnnecessaryReloadDetector(IServiceProvider sp) : base(sp)
        {
        }

        public override ISituationCollection ComputeMisplays(long steamId, long matchId)
        {

            var reloads = _context.WeaponReload
                .Where(x => x.MatchId == matchId && x.PlayerId == steamId)
                .Where(x => x.AmmoBefore >= MinAmmoBeforeLimit)
                .Select(x => new UnnecessaryReload
                {
                    MatchId = matchId,
                    Map = x.MatchStats.Map,
                    MatchDate = x.MatchStats.MatchDate,
                    Round = x.Round,
                    Time = x.Time,
                    PlayerId = steamId,
                    Weapon = x.Weapon,
                    AmmoBefore = x.AmmoBefore
                })
                .ToList();

            reloads.ForEach(x => 
            {
                x.DeathTime = _detectorHelpers.DeathTime(steamId, matchId, x.Round);
                x.ReloadStartTime = x.Time - 500; // estimate Time of ReloadStarted because we do not have precise data for WeaponReloadDuration
                x.WasFlashed = _detectorHelpers.WasFlashed(steamId, matchId, x.Round, x.ReloadStartTime);
                int nextDamageTaken = _detectorHelpers.NextDamageTaken(steamId, matchId, x.Round, x.ReloadStartTime, x.ReloadStartTime + MaxTimeBeforeDamageTakenLimit);
                x.TimeToNextDamageTaken = nextDamageTaken != -1 ? nextDamageTaken - x.ReloadStartTime : -1;
            });

            var collection = new SituationCollection<UnnecessaryReload>();
            collection.Misplays =reloads.Select(x => x as Misplay).ToList();
            return collection;
        }

        public override ISituationCollection FilterOutByConfig(ISituationCollection collection)
        {
            int MinAmmoBefore = 10;
            int MaxTimeBeforeDamageTaken = 1000;
            bool ExcludeFlashed = true;

            var misplays = collection.Misplays.Select(x => x as UnnecessaryReload);

            misplays = misplays.Where(x => x.AmmoBefore > MinAmmoBefore).ToList();

            if (MaxTimeBeforeDamageTaken != -1)
            {
                misplays = misplays
                    .Where(x => x.TimeToNextDamageTaken != -1 && x.TimeToNextDamageTaken < MaxTimeBeforeDamageTaken)
                    .ToList();
            }

            if (ExcludeFlashed)
            {
                misplays = misplays
                    .Where(x => !x.WasFlashed)
                    .ToList();
            }

            collection.Misplays = misplays.Select(x => x as Misplay).ToList();

            return collection;
        }
    }
}

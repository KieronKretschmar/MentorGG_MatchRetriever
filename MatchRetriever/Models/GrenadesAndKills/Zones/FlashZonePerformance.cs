using System;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FlashZonePerformance : IZonePerformance<FlashZonePerformance>
    {
        public int? ZoneId { get; set; }
        public bool IsCtZone { get; set; }
        public int SampleCount { get; set; } = 0;

        public int NadesBlindingEnemiesCount { get; set; } = 0;
        public int NadesBlindingTeamCount { get; set; } = 0;

        public int TotalEnemyTimeFlashed { get; set; } = 0;
        public int TotalTeamTimeFlashed { get; set; } = 0;

        public int MaxEnemyTimeFlashed { get; set; } = 0;
        public int MaxTeamTimeFlashed { get; set; } = 0;

        public int EnemyFlashAssists { get; set; } = 0;
        public int TeamFlashAssists { get; set; } = 0;

        public FlashZonePerformance Absorb (FlashZonePerformance other)
        {
            var res = new FlashZonePerformance();

            res.ZoneId = ZoneId;
            res.IsCtZone = IsCtZone;

            res.SampleCount = SampleCount + other.SampleCount;
            res.NadesBlindingEnemiesCount = NadesBlindingEnemiesCount + other.NadesBlindingEnemiesCount;
            res.NadesBlindingTeamCount =NadesBlindingTeamCount + other.NadesBlindingTeamCount;
            res.TotalEnemyTimeFlashed = TotalEnemyTimeFlashed + other.TotalEnemyTimeFlashed;
            res.TotalTeamTimeFlashed = TotalTeamTimeFlashed + other.TotalTeamTimeFlashed;

            res.MaxEnemyTimeFlashed = Math.Max(MaxEnemyTimeFlashed, other.MaxEnemyTimeFlashed);
            res.MaxTeamTimeFlashed = Math.Max(MaxTeamTimeFlashed, MaxTeamTimeFlashed);

            res.EnemyFlashAssists = EnemyFlashAssists + other.EnemyFlashAssists;
            res.TeamFlashAssists = TeamFlashAssists + other.TeamFlashAssists;

            return res;
        }
    }
}

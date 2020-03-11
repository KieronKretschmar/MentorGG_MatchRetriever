using System;
using ZoneReader.Enums;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class FlashZonePerformance : ZonePerformance<FlashZonePerformance>
    {

        public int NadesBlindingEnemiesCount { get; set; } = 0;
        public int NadesBlindingTeamCount { get; set; } = 0;

        public int TotalEnemyTimeFlashed { get; set; } = 0;
        public int TotalTeamTimeFlashed { get; set; } = 0;

        public int MaxEnemyTimeFlashed { get; set; } = 0;
        public int MaxTeamTimeFlashed { get; set; } = 0;

        public int EnemyFlashAssists { get; set; } = 0;
        public int TeamFlashAssists { get; set; } = 0;
        public override void Absorb (FlashZonePerformance other)
        {
            ZoneId = ZoneId;
            IsCtZone = IsCtZone;
            SampleCount = SampleCount + other.SampleCount;
            NadesBlindingEnemiesCount = NadesBlindingEnemiesCount + other.NadesBlindingEnemiesCount;
            NadesBlindingTeamCount = NadesBlindingTeamCount + other.NadesBlindingTeamCount;
            TotalEnemyTimeFlashed = TotalEnemyTimeFlashed + other.TotalEnemyTimeFlashed;
            TotalTeamTimeFlashed = TotalTeamTimeFlashed + other.TotalTeamTimeFlashed;
            MaxEnemyTimeFlashed = Math.Max(MaxEnemyTimeFlashed, other.MaxEnemyTimeFlashed);
            MaxTeamTimeFlashed = Math.Max(MaxTeamTimeFlashed, MaxTeamTimeFlashed);
            EnemyFlashAssists = EnemyFlashAssists + other.EnemyFlashAssists;
            TeamFlashAssists = TeamFlashAssists + other.TeamFlashAssists;
        }
    }
}

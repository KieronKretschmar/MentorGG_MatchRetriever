﻿using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models
{
    public class MatchesModel
    {
        public List<MatchInfo> MatchInfos = new List<MatchInfo>();
    }
}

namespace MatchRetriever
{
    public struct MatchInfo
    {
        public long MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public Source Source { get; set; }
        public string Map { get; set; }
        public StartingFaction WinningStartingTeam { get; set; }
        public bool AvailableForDownload { get; set; }

        public Scoreboard Scoreboard;
    }

    public struct Scoreboard
    {
        public Dictionary<StartingFaction, TeamInfo> TeamInfo { get; set; }
    }

    public class TeamInfo
    {
        public string TeamName { get; set; }
        public string Icon { get; set; }
        public int WonRounds { get; set; }
        public List<PlayerScoreboardEntry> Players { get; set; }
    }

    public struct PlayerScoreboardEntry
    {
        public Helpers.SteamUserOperator.SteamUser Profile { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int DamageDealt { get; set; }
        public int Score { get; set; }
        public int MVPs { get; set; }
        public int EnemiesFlashed { get; set; }
        public MatchMakingRank RankBeforeMatch { get; set; }
        public MatchMakingRank RankAfterMatch { get; set; }
    }
}
}
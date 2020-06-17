﻿using Database;
using MatchRetriever.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories
{
    public interface IPlayerInfoModelFactory
    {
        Task<PlayerInfoModel> GetModel(long steamId, bool forceRefresh);
    }

    public class PlayerInfoModelFactory : ModelFactoryBase, IPlayerInfoModelFactory
    {
        public PlayerInfoModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<PlayerInfoModel> GetModel(long steamId, bool forceRefresh)
        {
            var res = new PlayerInfoModel();
            var profile = await _steamUserOperator.GetUser(steamId, forceRefresh);

            res.SteamUser = profile;
            res.Rank = _context.CurrentRank(steamId);
            return res;
        }
    }
}

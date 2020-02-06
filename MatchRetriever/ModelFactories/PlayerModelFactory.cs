using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using MatchRetriever.Models;

namespace MatchRetriever.ModelFactories
{
    public interface IPlayerModelFactory
    {
        Task<PlayerInfoModel> GetModel(long steamId);
    }

    public class PlayerModelFactory : ModelFactoryBase, IPlayerModelFactory
    {
        public PlayerModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<PlayerInfoModel> GetModel(long steamId)
        {
            var res = new PlayerInfoModel();
            var profile = (await _steamUserOperator.GetUser(steamId));

            res.SteamName =profile.SteamName;
            res.IconPath = profile.ImageUrl;

            res.Rank = _context.CurrentRank(steamId);
            return res;
        }
    }
}

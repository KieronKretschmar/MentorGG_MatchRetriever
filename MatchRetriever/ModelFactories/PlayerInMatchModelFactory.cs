using MatchRetriever.Exceptions;
using MatchRetriever.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories
{
    public interface IPlayerInMatchModelFactory
    {
        Task<PlayerInMatchModel> GetPlayersInMatch(long matchId);
    }

    public class PlayerInMatchModelFactory : ModelFactoryBase, IPlayerInMatchModelFactory
    {
        public PlayerInMatchModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<PlayerInMatchModel> GetPlayersInMatch(long matchId)
        {
            try
            {
                var match = _context.MatchStats.Single(x => x.MatchId == matchId);
                var playerIds = match.PlayerMatchStats.Select(x => x.SteamId).ToList();
                var model = new PlayerInMatchModel();
                model.Players = playerIds;
                return model;
            }
            catch (InvalidOperationException)
            {
                throw new MatchNotFoundException();
            }

        }
    }
}
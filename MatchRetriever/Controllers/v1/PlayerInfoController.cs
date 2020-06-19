using System.Threading.Tasks;
using MatchRetriever.ModelFactories;
using MatchRetriever.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Controllers.v1
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public")]
    [ApiController]
    public class PlayerInfoController : ControllerBase
    {
        private readonly ILogger<PlayerInfoController> _logger;
        private readonly IPlayerInfoModelFactory _playerinfoModelFactory;

        public PlayerInfoController(ILogger<PlayerInfoController> logger, IPlayerInfoModelFactory playerinfoModelFactory)
        {
            _logger = logger;
            _playerinfoModelFactory = playerinfoModelFactory;
        }

        /// <summary>
        /// Returns metadata about a player, e.g. his SteamName and CSGO Rank.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/playerinfo")]
        public async Task<PlayerInfoModel> GetPlayerInfo(long steamId, bool forceRefresh = false)
        {
            var model = await _playerinfoModelFactory.GetModel(steamId, forceRefresh);
            return model;
        }
    }
}

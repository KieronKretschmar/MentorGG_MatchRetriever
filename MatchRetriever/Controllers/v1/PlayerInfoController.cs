using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Helpers;
using MatchRetriever.ModelFactories;
using MatchRetriever.ModelFactories.GrenadesAndKills;
using MatchRetriever.ModelFactories.GrenadesAndKillsOverviews;
using MatchRetriever.Models;
using MatchRetriever.Models.GrenadesAndKills;
using MatchRetriever.Models.GrenadesAndKillsOverviews;
using Microsoft.AspNetCore.Http;
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
            this._logger = logger;
            this._playerinfoModelFactory = playerinfoModelFactory;
        }

        /// <summary>
        /// Returns metadata about a player, e.g. his SteamName and CSGO Rank.
        /// </summary>
        /// <param name="steamId"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/playerinfo")]
        public async Task<PlayerInfoModel> GetPlayerInfo(long steamId)
        {
            var model = await _playerinfoModelFactory.GetModel(steamId);
            return model;
        }
    }
}

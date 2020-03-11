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
    public class PlayerSummaryController : BaseApiController
    {
        private readonly ILogger<PlayerSummaryController> _logger;
        private readonly IPlayerSummaryModelFactory _playerSummaryModelFactory;

        public PlayerSummaryController(ILogger<PlayerSummaryController> logger, IPlayerSummaryModelFactory playerSummaryModelFactory)
        {
            this._logger = logger;
            this._playerSummaryModelFactory = playerSummaryModelFactory;
        }

        [HttpGet("single/{steamId}/playersummary")]
        // GET v1/public/single/76561198033880857/playersummary
        public async Task<PlayerSummaryModel> GetPlayerSummary(long steamId)
        {
            var model = await _playerSummaryModelFactory.GetModel(steamId);
            return model;
        }
    }
}

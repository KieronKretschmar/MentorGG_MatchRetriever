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
    public class KillsController : ControllerBase
    {
        private readonly ILogger<KillsController> _logger;
        private readonly IKillModelFactory _killModelFactory;
        private readonly IOverviewModelFactory<KillOverviewMapSummary> _overviewModelFactory;

        public KillsController(ILogger<KillsController> logger, IKillModelFactory killModelFactory, IOverviewModelFactory<KillOverviewMapSummary> overviewModelFactory)
        {
            this._logger = logger;
            this._killModelFactory = killModelFactory;
            this._overviewModelFactory = overviewModelFactory;
        }

        /// <summary>
        /// Returns collections of kills and deaths of the player in the given matches in multiple lists,
        /// each collection covering all kills/deaths under a certain filterable condition (e.g. after c4 plan), 
        /// as well as additional data required for displaying them in the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="map"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/filterablekills")]
        public async Task<KillModel> GetFilterableKills(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _killModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        /// <summary>
        /// Returns a summary of the players performance for each selectable map, focussing on his kills and deaths.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/killsoverview")]
        public async Task<OverviewModel<KillOverviewMapSummary>> GetKillsOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

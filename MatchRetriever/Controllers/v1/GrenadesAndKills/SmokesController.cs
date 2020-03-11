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
    public class SmokesController : ControllerBase
    {
        private readonly ILogger<SmokesController> _logger;
        private readonly ISmokeModelFactory _smokeModelFactory;
        private readonly IOverviewModelFactory<SmokeOverviewMapSummary> _overviewModelFactory;

        public SmokesController(ILogger<SmokesController> logger, ISmokeModelFactory smokeModelFactory, IOverviewModelFactory<SmokeOverviewMapSummary> overviewModelFactory)
        {
            this._logger = logger;
            this._smokeModelFactory = smokeModelFactory;
            this._overviewModelFactory = overviewModelFactory;
        }

        /// <summary>
        /// Returns all smokes the player threw in the given matches, as well as additional data required for displaying them in the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="map"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/smokes")]
        public async Task<SmokeModel> GetSmokes(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _smokeModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }


        /// <summary>
        /// Returns a summary of the players performance for each selectable map, focussing on his smokes.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/smokesoverview")]
        public async Task<OverviewModel<SmokeOverviewMapSummary>> GetSmokesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

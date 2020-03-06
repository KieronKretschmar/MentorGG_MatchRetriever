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
    public class FireNadesController : ControllerBase
    {
        private readonly ILogger<FireNadesController> _logger;
        private readonly IFireNadeModelFactory _fireNadeModelFactory;
        private readonly IOverviewModelFactory<FireNadeOverviewMapSummary> _overviewModelFactory;

        public FireNadesController(ILogger<FireNadesController> logger, IFireNadeModelFactory fireNadeModelFactory, IOverviewModelFactory<FireNadeOverviewMapSummary> overviewModelFactory)
        {
            this._logger = logger;
            this._fireNadeModelFactory = fireNadeModelFactory;
            this._overviewModelFactory = overviewModelFactory;
        }

        /// <summary>
        /// Returns all molotovs/incgrenades the player threw in the given matches, as well as additional data required for displaying them in the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="map"></param>
        /// <param name="matchIds">MatchIds of matches played on the given map</param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/firenades")]
        public async Task<FireNadeModel> GetFireNades(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _fireNadeModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        /// <summary>
        /// Returns a summary of the players performance for each selectable map, focussing on his molotovs/inc grenades.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/firenadesoverview")]
        public async Task<OverviewModel<FireNadeOverviewMapSummary>> GetFireNadesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

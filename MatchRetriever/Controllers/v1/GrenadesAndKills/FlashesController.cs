using System.Collections.Generic;
using System.Threading.Tasks;
using MatchRetriever.Helpers;
using MatchRetriever.ModelFactories.GrenadesAndKills;
using MatchRetriever.ModelFactories.GrenadesAndKillsOverviews;
using MatchRetriever.Models.GrenadesAndKills;
using MatchRetriever.Models.GrenadesAndKillsOverviews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Controllers.v1
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public")]
    [ApiController]
    public class FlashesController : ControllerBase
    {
        private readonly ILogger<FlashesController> _logger;
        private readonly IFlashModelFactory _flashModelFactory;
        private readonly IOverviewModelFactory<FlashOverviewMapSummary> _overviewModelFactory;

        public FlashesController(ILogger<FlashesController> logger, IFlashModelFactory flashModelFactory, IOverviewModelFactory<FlashOverviewMapSummary> overviewModelFactory)
        {
            this._logger = logger;
            this._flashModelFactory = flashModelFactory;
            this._overviewModelFactory = overviewModelFactory;
        }

        /// <summary>
        /// Returns all flashes the player threw in the given matches, as well as additional data required for displaying them in the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="map"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/flashes")]
        public async Task<FlashModel> GetFlashes(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _flashModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        /// <summary>
        /// Returns a summary of the players performance for each selectable map, focussing on his flashes.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/flashesoverview")]
        public async Task<OverviewModel<FlashOverviewMapSummary>> GetFlashesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

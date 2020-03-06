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
    public class ImportantPositionsController : ControllerBase
    {
        private readonly ILogger<ImportantPositionsController> _logger;
        private readonly IImportantPositionsModelFactory _importantPositionsModelFactory;

        public ImportantPositionsController(ILogger<ImportantPositionsController> logger, IImportantPositionsModelFactory importantPositionsModelFactory)
        {
            this._logger = logger;
            this._importantPositionsModelFactory = importantPositionsModelFactory;
        }

        /// <summary>
        /// Gets performances for the player's best and worst positions.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <param name="showBest"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/importantpositions")]
        public async Task<ImportantPositionsModel> GetImportantPositions(long steamId, [CsvModelBinder]List<long> matchIds, bool showBest, int count)
        {
            var model = await _importantPositionsModelFactory.GetModel(steamId, matchIds, showBest, count);
            return model;
        }
    }
}

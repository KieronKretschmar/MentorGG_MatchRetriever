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
    public class ImportantPositionsController : BaseApiController
    {
        private readonly ILogger<ImportantPositionsController> _logger;
        private readonly IImportantPositionsModelFactory _importantPositionsModelFactory;

        public ImportantPositionsController(ILogger<ImportantPositionsController> logger, IImportantPositionsModelFactory importantPositionsModelFactory)
        {
            this._logger = logger;
            this._importantPositionsModelFactory = importantPositionsModelFactory;
        }

        [HttpGet("single/{steamId}/importantpositions")]
        // GET v1/public/single/76561198033880857/importantpositions?matchIds=1,2,3&showbest=true&count=3
        public async Task<ImportantPositionsModel> GetImportantPositions(long steamId, [CsvModelBinder]List<long> matchIds, bool showBest, int count)
        {
            var model = await _importantPositionsModelFactory.GetModel(steamId, matchIds, showBest, count);
            return model;
        }
    }
}

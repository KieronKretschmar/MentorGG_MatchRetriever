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
    public class SmokesController : BaseApiController
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

        [HttpGet("single/{steamId}/smokes")]
        // GET v1/public/single/76561198033880857/smokes?map=de_mirage&matchIds=1,2,3
        public async Task<SmokeModel> GetSmokes(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _smokeModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        [HttpGet("single/{steamId}/smokesoverview")]
        // GET v1/public/single/76561198033880857/smokesoverview?matchIds=1,2,3
        public async Task<OverviewModel<SmokeOverviewMapSummary>> GetSmokesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

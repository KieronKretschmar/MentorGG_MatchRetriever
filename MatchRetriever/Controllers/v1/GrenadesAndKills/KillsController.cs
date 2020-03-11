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
    public class KillsController : BaseApiController
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

        [HttpGet("single/{steamId}/filterablekills")]
        // GET v1/public/single/76561198033880857/filterablekills?map=de_mirage&matchIds=1,2,3
        public async Task<KillModel> GetFilterableKills(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _killModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        [HttpGet("single/{steamId}/killsoverview")]
        // GET v1/public/single/76561198033880857/killsoverview?matchIds=1,2,3
        public async Task<OverviewModel<KillOverviewMapSummary>> GetKillsOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

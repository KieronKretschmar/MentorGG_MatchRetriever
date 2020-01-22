using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Helpers;
using MatchRetriever.ModelFactories;
using MatchRetriever.ModelFactories.Grenades;
using MatchRetriever.Models;
using MatchRetriever.Models.Grenades;
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
        private readonly ILineupModelFactory<SmokeSample, SmokeLineupPerformance> _sampleModelFactory;
        private readonly IOverviewModelFactory<SmokeOverviewMapSummary> _overviewModelFactory;

        public SmokesController(ILogger<SmokesController> logger, ILineupModelFactory<SmokeSample, SmokeLineupPerformance> sampleModelFactory, IOverviewModelFactory<SmokeOverviewMapSummary> overviewModelFactory)
        {
            this._logger = logger;
            this._sampleModelFactory = sampleModelFactory;
            this._overviewModelFactory = overviewModelFactory;
        }

        [Route("single/smokes")]
        // GET v1/public/single/smokes?steamId=76561198033880857&map=de_mirage&matchIds=1,2,3
        public async Task<LineupModel<SmokeSample, SmokeLineupPerformance>> GetSmokes(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _sampleModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        [Route("single/smokesoverview")]
        // GET v1/public/single/smokesoverview?steamId=76561198033880857&matchIds=1,2,3
        public async Task<OverviewModel<SmokeOverviewMapSummary>> GetSmokesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

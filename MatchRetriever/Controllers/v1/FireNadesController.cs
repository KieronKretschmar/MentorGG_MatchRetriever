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
    public class FireNadesController : BaseApiController
    {
        private readonly ILogger<FireNadesController> _logger;
        private readonly IZoneModelFactory<FireNadeSample, FireNadeZonePerformance> _sampleModelFactory;
        private readonly IOverviewModelFactory<FireNadeOverviewMapSummary> _overviewModelFactory;

        public FireNadesController(ILogger<FireNadesController> logger, IZoneModelFactory<FireNadeSample, FireNadeZonePerformance> sampleModelFactory, IOverviewModelFactory<FireNadeOverviewMapSummary> overviewModelFactory)
        {
            this._logger = logger;
            this._sampleModelFactory = sampleModelFactory;
            this._overviewModelFactory = overviewModelFactory;
        }

        [Route("single/firenades")]
        // GET v1/public/single/firenades?steamId=76561198033880857&map=de_mirage&matchIds=1,2,3
        public async Task<ZoneModel<FireNadeSample, FireNadeZonePerformance>> GetFireNades(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _sampleModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        [Route("single/firenadesoverview")]
        // GET v1/public/single/firenadesoverview?steamId=76561198033880857&matchIds=1,2,3
        public async Task<OverviewModel<FireNadeOverviewMapSummary>> GetFireNadesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

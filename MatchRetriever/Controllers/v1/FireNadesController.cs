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
    public class FireNadesController : BaseApiController
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

        [Route("single/{steamId}/firenades")]
        // GET v1/public/single/76561198033880857/firenades?map=de_mirage&matchIds=1,2,3
        public async Task<FireNadeModel> GetFireNades(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _fireNadeModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        [Route("single/{steamId}/firenadesoverview")]
        // GET v1/public/single/76561198033880857/firenadesoverview?&matchIds=1,2,3
        public async Task<OverviewModel<FireNadeOverviewMapSummary>> GetFireNadesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

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
    public class FlashesController : BaseApiController
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

        [Route("single/flashes")]
        // GET v1/public/single/flashes?steamId=76561198033880857&map=de_mirage&matchIds=1,2,3
        public async Task<FlashModel> GetFlashes(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _flashModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        [Route("single/flashesoverview")]
        // GET v1/public/single/flashesoverview?steamId=76561198033880857&matchIds=1,2,3
        public async Task<OverviewModel<FlashOverviewMapSummary>> GetFlashesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

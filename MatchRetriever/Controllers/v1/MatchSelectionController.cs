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
    public class MatchSelectionController : BaseApiController
    {
        private readonly ILogger<MatchSelectionController> _logger;
        private readonly IMatchSelectionModelFactory _modelFactory;

        public MatchSelectionController(ILogger<MatchSelectionController> logger, IMatchSelectionModelFactory modelFactory)
        {
            this._logger = logger;
            this._modelFactory = modelFactory;
        }

        [HttpGet("single/{steamId}/matchselection")]
        // GET v1/public/single/76561198033880857/matchselection?dailyLimit=3
        public async Task<MatchSelectionModel> GetMetaMatchHistory(long steamId, int dailyLimit)
        {
            var model = await _modelFactory.GetModel(steamId, dailyLimit);
            return model;
        }
    }
}

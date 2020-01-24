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
    public class MetaMatchHistoryController : BaseApiController
    {
        private readonly ILogger<MetaMatchHistoryController> _logger;
        private readonly IMetaMatchHistoryModelFactory _modelFactory;

        public MetaMatchHistoryController(ILogger<MetaMatchHistoryController> logger, IMetaMatchHistoryModelFactory modelFactory)
        {
            this._logger = logger;
            this._modelFactory = modelFactory;
        }

        [Route("single/metamatchhistory")]
        // GET v1/public/single/metamatchhistory?steamId=76561198033880857&dailyLimit=3
        public async Task<MetaMatchHistoryModel> GetMetaMatchHistory(long steamId, int dailyLimit)
        {
            var model = await _modelFactory.GetModel(steamId, dailyLimit);
            return model;
        }
    }
}

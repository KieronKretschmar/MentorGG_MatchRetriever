﻿using System.Threading.Tasks;
using MatchRetriever.Enumerals;
using MatchRetriever.ModelFactories;
using MatchRetriever.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Controllers.v1
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public")]
    [ApiController]
    public class MatchSelectionController : ControllerBase
    {
        private readonly ILogger<MatchSelectionController> _logger;
        private readonly IMatchSelectionModelFactory _modelFactory;

        public MatchSelectionController(ILogger<MatchSelectionController> logger, IMatchSelectionModelFactory modelFactory)
        {
            this._logger = logger;
            this._modelFactory = modelFactory;
        }

        /// <summary>
        /// Returns the user's matches in the database.
        /// Limiting the returned info based on the subscriptionType.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="subscriptionType"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/matchselection")]
        public async Task<MatchSelectionModel> GetMatchSelection(long steamId, SubscriptionType subscriptionType)
        {
            var model = await _modelFactory.GetModel(steamId, subscriptionType);
            return model;
        }
    }
}

﻿using System;
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
    public class HesController : BaseApiController
    {
        private readonly ILogger<HesController> _logger;
        private readonly IHeModelFactory _heModelFactory;
        private readonly IOverviewModelFactory<HeOverviewMapSummary> _overviewModelFactory;

        public HesController(ILogger<HesController> logger, IHeModelFactory sampleModelFactory, IOverviewModelFactory<HeOverviewMapSummary> overviewModelFactory)
        {
            this._logger = logger;
            this._heModelFactory = sampleModelFactory;
            this._overviewModelFactory = overviewModelFactory;
        }

        [Route("single/hes")]
        // GET v1/public/single/hes?steamId=76561198033880857&map=de_mirage&matchIds=1,2,3
        public async Task<HeModel> GetHes(long steamId, string map, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _heModelFactory.GetModel(steamId, map, matchIds);
            return model;
        }

        [Route("single/hesoverview")]
        // GET v1/public/single/hesoverview?steamId=76561198033880857&matchIds=1,2,3
        public async Task<OverviewModel<HeOverviewMapSummary>> GetHesOverview(long steamId, [CsvModelBinder]List<long> matchIds)
        {
            var model = await _overviewModelFactory.GetModel(steamId, matchIds);
            return model;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities;
using MatchRetriever.Enumerals;
using MatchRetriever.ModelFactories;
using MatchRetriever.ModelFactories.DemoViewer;
using MatchRetriever.Models.DemoViewer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Controllers.v1
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly ILogger<MatchController> _logger;
        private readonly IMatchDataSetFactory _matchDataSetFactory;

        public MatchController(ILogger<MatchController> logger, IMatchDataSetFactory matchDataSetFactory, IDemoViewerRoundModelFactory roundFactory)
        {
            this._logger = logger;
            this._matchDataSetFactory = matchDataSetFactory;
        }

        /// <summary>
        /// Loads data for the match with the given matchId into a MatchDataSet object and returns it.
        /// 
        /// Make sure to update this method when new tables are added, as I could not find a generic
        /// solution that works without having to load and assign explicitly each table's data explicitly. 
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet("match/{matchId}/matchdataset")]
        public async Task<MatchDataSet> GetMatchDataSet(long matchId)
        {
            // Try to parse quality and default to Low quality if value is invalid 
            var model = await _matchDataSetFactory.GetModel(matchId);
            return model;
        }
    }
}
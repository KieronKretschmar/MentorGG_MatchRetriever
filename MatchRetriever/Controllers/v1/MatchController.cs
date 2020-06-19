using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities;
using MatchRetriever.Enumerals;
using MatchRetriever.Exceptions;
using MatchRetriever.ModelFactories;
using MatchRetriever.ModelFactories.DemoViewer;
using MatchRetriever.Models;
using MatchRetriever.Models.DemoViewer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Controllers.v1
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public/match")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly ILogger<MatchController> _logger;
        private readonly IMatchDataSetFactory _matchDataSetFactory;
        private readonly IPlayerInMatchModelFactory _playerInMatchModelFactory;

        public MatchController(
            ILogger<MatchController> logger, 
            IMatchDataSetFactory matchDataSetFactory, 
            IDemoViewerRoundModelFactory roundFactory,
            IPlayerInMatchModelFactory playerInMatchModelFactory)
        {
            _logger = logger;
            _matchDataSetFactory = matchDataSetFactory;
            _playerInMatchModelFactory = playerInMatchModelFactory;
        }

        /// <summary>
        /// Loads data for the match with the given matchId into a MatchDataSet object and returns it.
        /// 
        /// Make sure to update this method when new tables are added, as I could not find a generic
        /// solution that works without having to load and assign explicitly each table's data explicitly. 
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet("{matchId}/matchdataset")]
        public async Task<MatchDataSet> GetMatchDataSet(long matchId)
        {
            // Try to parse quality and default to Low quality if value is invalid 
            var model = await _matchDataSetFactory.GetModel(matchId);
            return model;
        }

        /// <summary>
        /// Returns SteamIds for each participating player of the given match.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet("{matchId}/players")]
        public async Task<ActionResult<PlayerInMatchModel>> GetParticipatingPlayersInMatch(long matchId)
        {
            try
            {
                return await _playerInMatchModelFactory.GetPlayersInMatch(matchId);
            }
            catch (MatchNotFoundException)
            {
                var msg = $"Players in match request failed - Match [ {matchId} ] not found";
                _logger.LogInformation(msg);
                return NotFound(msg);
            }
        }
    }
}
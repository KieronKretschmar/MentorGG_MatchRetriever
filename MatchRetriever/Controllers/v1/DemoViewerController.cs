using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Enumerals;
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
    public class DemoViewerController : ControllerBase
    {
        private readonly ILogger<DemoViewerController> _logger;
        private readonly IDemoViewerMatchModelFactory _matchFactory;
        private readonly IDemoViewerRoundModelFactory _roundFactory;

        public DemoViewerController(ILogger<DemoViewerController> logger, IDemoViewerMatchModelFactory matchFactory, IDemoViewerRoundModelFactory roundFactory)
        {
            this._logger = logger;
            this._matchFactory = matchFactory;
            this._roundFactory = roundFactory;
        }

        /// <summary>
        /// Get MetaData for a match required to watch rounds in DemoViewer.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        [HttpGet("watch/match/{matchId}")]
        public async Task<DemoViewerMatchModel> GetMatch(long matchId)
        {
            // Try to parse quality and default to Low quality if value is invalid 
            var model = await _matchFactory.GetModel(matchId);
            return model;
        }

        /// <summary>
        /// Get required data to watch a round in DemoViewer.
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="round"></param>
        /// <param name="quality">Currently ignored, as the highest available quality is returned.</param>
        /// <returns></returns>
        [HttpGet("watch/match/{matchId}/round/{round}")]
        public async Task<DemoViewerRoundModel> GetRound(long matchId, short round, DemoViewerQuality quality)
        {
            var model = await _roundFactory.GetModel(matchId, round);
            return model;
        }
    }
}
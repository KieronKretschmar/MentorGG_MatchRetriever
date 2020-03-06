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
        /// 
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
        /// 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="round"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        [HttpGet("watch/match/{matchId}/round/{round}")]
        public async Task<DemoViewerRoundModel> GetRound(long matchId, short round, DemoViewerQuality quality)
        {
            // Try to parse quality and default to Low quality if value is invalid 
            var demoViewerQuality = Enum.IsDefined(typeof(DemoViewerQuality), quality) ? (DemoViewerQuality)quality : DemoViewerQuality.Low;
            var model = await _roundFactory.GetModel(matchId, round, demoViewerQuality);
            return model;
        }
    }
}
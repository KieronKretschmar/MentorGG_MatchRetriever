using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Models;
using MatchRetriever.ModelFactories;
using MatchRetriever.Helpers;

namespace MatchRetriever.Controllers.v1
{

    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchesModelFactory _matchesModelFactory;

        public MatchesController(IMatchesModelFactory matchesModelFactory)
        {
            _matchesModelFactory = matchesModelFactory;
        }

        /// <summary>
        /// Returns data regarding for each match, including scoreboards.
        /// Each matchId that is neither in matchIds nor in ignoredMatchIds will be treated as being above the daily limit and thus censored.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds">MatchIds for which uncensored data should be returned</param>
        /// <param name="count"></param>
        /// <param name="ignoredMatchIds">MatchIds for which no data should be returned.</param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/matches")]
        // GET v1/public/single/76561198033880857/matches?matchIds=1,2,3&count=5&offset=0
        public async Task<MatchesModel> GetMatches(long steamId, [CsvModelBinder] List<long> matchIds, int count, [CsvModelBinder] List<long> ignoredMatchIds, int offset = 0)
        {
            var model = await _matchesModelFactory.GetModel(steamId, matchIds, ignoredMatchIds, count, offset);
            return model;
        }
    }
}

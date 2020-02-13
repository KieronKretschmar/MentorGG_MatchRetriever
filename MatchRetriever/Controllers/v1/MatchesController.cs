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
    public class MatchesController : BaseApiController
    {
        private readonly IMatchesModelFactory _matchesModelFactory;

        public MatchesController(IMatchesModelFactory matchesModelFactory)
        {
            _matchesModelFactory = matchesModelFactory;
        }

        [HttpGet("single/{steamId}/matches")]
        // GET v1/public/single/76561198033880857/matches?matchIds=1,2,3&offset=0
        public async Task<MatchesModel> GetMatches(long steamId, [CsvModelBinder] List<long> matchIds, int offset = 0)
        {
            var model = await _matchesModelFactory.GetModel(steamId, matchIds, offset);
            return model;
        }
    }
}

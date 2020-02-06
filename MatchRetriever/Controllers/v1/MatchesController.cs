using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Models;
using MatchRetriever.ModelFactories;

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

        public MatchesModel GetMatches(List<long> matchIds, int offset = 0)
        {
            var steamId = long.Parse(User.Identity.Name);
            return GetMatches(steamId,matchIds,offset);
        }

        public MatchesModel GetMatches(long steamId, List<long> matchIds, int offset = 0)
        {
            var model = _matchesModelFactory.GetModel(steamId, matchIds, offset);
            return model;
        }

    }
}

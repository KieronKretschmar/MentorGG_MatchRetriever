using MatchRetriever.Helpers;
using MatchRetriever.ModelFactories;
using MatchRetriever.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Controllers.v1
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public")]
    [ApiController]
    public class MisplaysController : ControllerBase
    {
        private readonly IMisplayModelFactory _misplayModelFactory;

        public MisplaysController(IMisplayModelFactory factory)
        {
            _misplayModelFactory = factory;
        }

        [HttpGet("single/{steamId}/misplays/match/{matchId}")]
        public async Task<MisplaysModel> GetMisplaysModel(long steamId, long matchId)
        {
            var model = await _misplayModelFactory.GetModel(steamId, matchId);
            return model;
        }
    }
}

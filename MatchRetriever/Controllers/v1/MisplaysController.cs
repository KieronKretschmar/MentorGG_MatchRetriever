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
    public class MisplaysController : BaseApiController
    {
        private readonly MisplayModelFactory _misplayModelFactory;

        public MisplaysController(MisplayModelFactory factory)
        {
            _misplayModelFactory = factory;
        }

        [HttpGet("single/{steamId}/misplays")]
        public async Task<MisplaysModel> GetMisplaysModel(long steamId, long matchId)
        {
            var model = await _misplayModelFactory.GetModel(steamId, matchId);
            return model;
        }
    }
}

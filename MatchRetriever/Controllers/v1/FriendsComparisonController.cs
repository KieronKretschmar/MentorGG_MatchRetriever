using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Helpers;
using MatchRetriever.ModelFactories;
using MatchRetriever.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MatchRetriever.Controllers.v1
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/public")]
    [ApiController]
    public class FriendsComparisonController : ControllerBase
    {
        private readonly ILogger<FriendsComparisonController> _logger;
        private readonly IFriendsComparisonModelFactory _friendsComparisonModelFactory;

        public FriendsComparisonController(ILogger<FriendsComparisonController> logger, IFriendsComparisonModelFactory friendsComparisonModelFactory)
        {
            _logger = logger;
            _friendsComparisonModelFactory = friendsComparisonModelFactory;
        }

        /// <summary>
        /// Get data comparing a player's performance with other players he played with the most.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [HttpGet("single/{steamId}/friendscomparison")]
        public async Task<FriendsComparisonModel> GetFriendsComparison(long steamId, [CsvModelBinder] List<long> matchIds, int count = 3, int offset = 0)
        {
            var model = await _friendsComparisonModelFactory.GetModel(steamId, matchIds, count, offset);
            return model;
        }
    }
}

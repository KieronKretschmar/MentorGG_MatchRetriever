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
    public class FriendsComparisonController : BaseApiController
    {
        private readonly ILogger<FriendsComparisonController> _logger;
        private readonly IFriendsComparisonModelFactory _friendsComparisonModelFactory;

        public FriendsComparisonController(ILogger<FriendsComparisonController> logger, IFriendsComparisonModelFactory friendsComparisonModelFactory)
        {
            _logger = logger;
            _friendsComparisonModelFactory = friendsComparisonModelFactory;
        }
        [HttpGet("single/{steamId}/friendscomparison")]
        public async Task<FriendsComparisonModel> GetFriendsComparison(long steamId, [CsvModelBinder] List<long> matchIds, int maxFriends, int offset)
        {
            var model = await _friendsComparisonModelFactory.GetModel(steamId, maxFriends, matchIds, offset);
            return model;
        }
    }
}

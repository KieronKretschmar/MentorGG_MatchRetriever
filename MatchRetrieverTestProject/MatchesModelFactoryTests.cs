using Database;
using MatchEntities;
using MatchRetriever;
using MatchRetriever.ModelFactories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetrieverTestProject
{
    [TestClass]
    public class MatchesModelFactoryTests
    {
        [TestMethod]
        public async Task TestGetModel()
        {
            //// Arrange
            // Settings
            var allMatchIds = new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var steamId = 100;
            var team = MatchEntities.Enums.StartingFaction.CtStarter;
            var forbiddenMatchIds = allMatchIds.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
            var allowedMatchids = allMatchIds.Except(forbiddenMatchIds).ToList();

            // Create serviceProvider with inmemory context
            var services = TestHelper.GetMoqFactoryServiceCollection("TestGetModel");
            var sp = services.BuildServiceProvider();

            // Create mock matches in which the player participated and write into database
            var context = sp.GetRequiredService<MatchContext>();
            // Create MatchStats
            var matches = TestHelper.CreateMatchStats(allMatchIds);
            context.MatchStats.AddRange(matches);
            // Create PlayerMatchStats
            var playerMatchStats = allMatchIds.Select(x => new PlayerMatchStats
            {
                MatchId = x,
                SteamId = steamId,
                Team = team
            });
            context.PlayerMatchStats.AddRange(playerMatchStats);
            await context.SaveChangesAsync();

            //// Run
            var factory = new MatchesModelFactory(sp);
            var model = await factory.GetModel(steamId, allowedMatchids, 0);

            //// Assert
            var allowedMatchIdsReturned = model.MatchInfos.Select(x => x.MatchId).ToList();
            // Assert that each match the user played is returned
            Assert.AreEqual(allMatchIds.Distinct().Count(), allowedMatchIdsReturned.Distinct().Count() + model.HiddenMatchInfos.Count());

            // Assert that all hidden matches' matchids are censored
            Assert.IsTrue(model.HiddenMatchInfos.All(x => x.MatchId == -1));

            // Assert that all matches have exactly one scoreboard entry
            // Note that we can't check for correct steamId as long as
            Assert.IsTrue(model.HiddenMatchInfos.All(x => x.Scoreboard.TeamInfo[team].Players.Single().SteamId == steamId));
        }
    }
}

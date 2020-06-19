using Database;
using MatchEntities;
using MatchRetriever.ModelFactories;
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
            var allMatchIds = Enumerable.Range(1, 9).Select(x => (long)x).ToList();
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
            var model = await factory.GetModel(steamId, allowedMatchids, forbiddenMatchIds, allMatchIds.Count, 0);

            //// Assert
            var allowedMatchIdsReturned = model.MatchInfos.Select(x => x.MatchId).ToList();
            // Assert that each match the user played is returned
            Assert.AreEqual(allMatchIds.Count(), model.MatchInfos.Count());

            // Assert that all hidden matches' matchids are censored
            Assert.IsTrue(model.MatchInfos.Where(x => forbiddenMatchIds.Contains(x.MatchId)).All(x => x.MatchId == -1));

            // Assert that all matches have exactly the one scoreboard entry we created at the beginning of this test
            Assert.IsTrue(model.MatchInfos.All(x => x.Scoreboard.TeamInfos[team].Players.Single().SteamId == steamId));
        }

        [DataRow(1, 5, 2)]
        [DataRow(10, 5, 0)]
        [DataRow(15, 10, 0)]
        [DataRow(5, 7, 6)]
        [DataRow(5, 1, 8)]
        [DataTestMethod]
        public async Task TestCountAndOffset(int matchesInDb, int count, int offset)
        {
            //// Arrange
            // Settings
            var allMatchIds = Enumerable.Range(1, matchesInDb).Select(x => (long)x).ToList();
            var steamId = 100;
            var team = MatchEntities.Enums.StartingFaction.CtStarter;
            var ignoredMatchIds = new List<long>();

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
            var model = await factory.GetModel(steamId, allMatchIds, ignoredMatchIds, count, offset);

            //// Assert
            var expectedMatchesCount = Math.Min(count, Math.Max(0, matchesInDb - offset));
            Assert.AreEqual(model.MatchInfos.Count, expectedMatchesCount);
            // Assert that offset works
            if (offset <= matchesInDb)
            {
                Assert.AreEqual(allMatchIds[offset], model.MatchInfos.First().MatchId);
            }
        }
    }
}

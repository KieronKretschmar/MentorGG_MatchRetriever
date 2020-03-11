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
    public class MatchSelectionModelFactoryTests
    {
        [TestMethod]
        public async Task TestGetModel()
        {
            //// Arrange
            // Settings
            var steamId = 100;
            var team = MatchEntities.Enums.StartingFaction.CtStarter;
            var firstDay = DateTime.Parse("2020-01-01");
            var dailyLimit = 3;

            // Create serviceProvider with inmemory context
            var services = TestHelper.GetMoqFactoryServiceCollection("TestGetModel");
            var sp = services.BuildServiceProvider();

            // Create mock matches in which the user participated and write into database
            var context = sp.GetRequiredService<MatchContext>();
            // Create MatchStats with known MatchDates
            var matches = new List<MatchStats>
            {
                new MatchStats{MatchId = 1, MatchDate = firstDay.AddDays(0)},
                new MatchStats{MatchId = 2, MatchDate = firstDay.AddDays(1.1)},
                new MatchStats{MatchId = 3, MatchDate = firstDay.AddDays(1.2)},
                new MatchStats{MatchId = 4, MatchDate = firstDay.AddDays(1.3)},
                new MatchStats{MatchId = 5, MatchDate = firstDay.AddDays(1.6)}, // With dailyLimit=3, this one is supposed to be ignored
                new MatchStats{MatchId = 6, MatchDate = firstDay.AddDays(2.0)},
                new MatchStats{MatchId = 7, MatchDate = firstDay.AddDays(3.0)},
            };
            context.MatchStats.AddRange(matches);
            // Create PlayerMatchStats
            var playerMatchStats = matches.Select(x => new PlayerMatchStats
            {
                MatchId = x.MatchId,
                SteamId = steamId,
                Team = team
            });
            context.PlayerMatchStats.AddRange(playerMatchStats);
            await context.SaveChangesAsync();

            //// Run
            var factory = new MatchSelectionModelFactory(sp);
            var model = await factory.GetModel(steamId, dailyLimit);

            //// Assert

            // Check that all matches except #4 are returned
            CollectionAssert.AreEqual(new List<long> { 1, 2, 3, 4, 6, 7 }, model.Matches.Select(x => x.MatchId).ToList());
        }
    }
}

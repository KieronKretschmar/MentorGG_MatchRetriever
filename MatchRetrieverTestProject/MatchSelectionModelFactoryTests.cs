using Database;
using MatchEntities;
using MatchRetriever;
using MatchRetriever.Configuration;
using MatchRetriever.Enumerals;
using MatchRetriever.ModelFactories;
using MatchRetrieverTestProject.Mocks;
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
            // Arrange
            // Settings
            var steamId = 100;
            var team = MatchEntities.Enums.StartingFaction.CtStarter;
            var firstDay = DateTime.Parse("2020-01-01");
            
            var config = new SubscriptionConfig
            {
                Free = new SubscriptionSettings {
                    DailyMatchesLimit = 3,
                    MatchAccessDurationInDays = 14,
                    PositionFramesPerSecond = 1
                }
            };

            // Create serviceProvider with inmemory context
            var services = TestHelper.GetMoqFactoryServiceCollection("TestGetModel");
            var sp = services.BuildServiceProvider();

            // Create mock matches in which the user participated and write into database
            var context = sp.GetRequiredService<MatchContext>();
            // Create MatchStats with known MatchDates
            var matches = new List<MatchStats>
            {
                new MatchStats{MatchId = 1, MatchDate = firstDay.AddDays(0)}, // allowed (none before)
                new MatchStats{MatchId = 2, MatchDate = firstDay.AddDays(1.1)}, // allowed (none before)
                new MatchStats{MatchId = 3, MatchDate = firstDay.AddDays(1.2)}, // allowed (only 2 allowed within 24h)
                new MatchStats{MatchId = 4, MatchDate = firstDay.AddDays(1.3)}, // allowed (only 2,3 allowed within 24h)
                new MatchStats{MatchId = 5, MatchDate = firstDay.AddDays(1.6)}, // ignored (2,3,4 allowed within 24h)
                new MatchStats{MatchId = 6, MatchDate = firstDay.AddDays(2.0)}, // ignored (2,3,4 allowed within 24h)
                new MatchStats{MatchId = 7, MatchDate = firstDay.AddDays(2.15)}, // allowed (only 3,4 allowed within 24h)
                new MatchStats{MatchId = 8, MatchDate = firstDay.AddDays(2.17)}, // ignored (3,4,7 allowed within 24h)
                new MatchStats{MatchId = 9, MatchDate = firstDay.AddDays(2.25)}, // allowed (only 4,7 allowed within 24h)
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

            // Run
            var factory = new MatchSelectionModelFactory(sp, new MockedSubscriptionConfigLoader(config));
            var model = await factory.GetModel(steamId, SubscriptionType.Free);

            // Assert
            // Check that the correct matches are returned
            CollectionAssert.AreEqual(new List<long> { 1, 2, 3, 4, 7, 9 }, model.Matches.Select(x => x.MatchId).ToList());
        }
    }
}

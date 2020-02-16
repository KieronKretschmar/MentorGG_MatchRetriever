using Database;
using MatchEntities;
using MatchRetriever.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchRetrieverTestProject
{
    public static class TestHelper
    {
        public static List<MatchStats> CreateMatchStats(List<long> matchIds)
        {
            var list = matchIds.Select(x =>
                new MatchStats
                {
                    MatchId = x,
                })
                .ToList();

            return list;
        }

        public static ServiceCollection GetMoqFactoryServiceCollection(string databaseName)
        {
            var services = new ServiceCollection();
            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<MatchContext>((serviceProvider, options) =>
                {
                    options.UseInMemoryDatabase(databaseName: databaseName).UseInternalServiceProvider(serviceProvider);
                });

            services.AddLogging(services =>
            {
                services.AddConsole();
                services.AddDebug();
            });

            // Add SteamUserOperator as mock, returning mocked users
            var steamUserOperatorMock = new Mock<ISteamUserOperator>();
            steamUserOperatorMock
                .Setup(x => x.GetUsers(It.IsAny<List<long>>()))
                .Returns((List<long> steamIds) =>
                    Task.FromResult(steamIds.Distinct().Select(steamId => new SteamUser
                    {
                        SteamId = steamId,
                        ImageUrl = "ImageUrl",
                        SteamName = "SteamName",
                    })
                    .ToList())
                );
            steamUserOperatorMock.Setup(x => x.GetUser(It.IsAny<long>()))
                .Returns((long steamId) =>
                    Task.FromResult(new SteamUser
                    {
                        SteamId = steamId,
                        ImageUrl = "ImageUrl",
                        SteamName = "SteamName",
                    })
                );
            services.AddSingleton(steamUserOperatorMock.Object);

            return services;
        }
    }
}

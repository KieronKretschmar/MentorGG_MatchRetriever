using MatchRetriever;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MatchRetrieverTestProject
{
    [TestClass]
    public class BasicTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicTests()
        {
            _factory = new WebApplicationFactory<Startup>();
        }

        [DataTestMethod]
        [DataRow("filterablekills")]
        [DataRow("killsoverview")]
        [DataRow("hes")]
        [DataRow("hesoverview")]
        [DataRow("smokes")]
        [DataRow("smokesoverview")]
        [DataRow("flashes")]
        [DataRow("flashesoverview")]
        [DataRow("firenades")]
        [DataRow("firenadesoverview")]
        public async Task Get_GrenadeEndpointsReturnSuccessAndCorrectContentType(string action)
        {
            // Arrange
            var client = _factory.CreateClient();

            var url = $"/v1/public/single/{action}";
            url += "?steamId=76561198033880857";
            url += "&matchIds=1,2,3";
            if (!action.Contains("overview"))
            {
                url += "&map=de_mirage";
            }

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [DataTestMethod]
        [DataRow("v1/public/single/metamatchhistory?steamId=76561198033880857&dailyLimit=3")]
        public async Task Get_OtherEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}

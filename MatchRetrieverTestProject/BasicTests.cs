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
            //_factory = factory;
            _factory = new WebApplicationFactory<Startup>();
        }

        [DataTestMethod]
        [DataRow("/v1/public/single/firenades?steamId=76561198033880857&map=de_mirage&matchIds=1,2,3")]
        [DataRow("/v1/public/single/firenadesoverview?steamId=76561198033880857&matchIds=1,2,3")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
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

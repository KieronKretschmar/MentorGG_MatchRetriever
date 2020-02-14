using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MatchRetriever.Helpers
{
    public interface ISteamUserOperator
    {
        Task<SteamUserOperator.SteamUser> GetUser(long steamId);
        Task<List<SteamUserOperator.SteamUser>> GetUsers(List<long> steamIds);
    }

    /// <summary>
    /// Class for communicating with the SteamUserOperator service.
    /// </summary>
    public class SteamUserOperator : ISteamUserOperator
    {
        private readonly HttpClient Client;
        private readonly string endpointUri;
        private readonly ILogger _logger;

        public SteamUserOperator(ILogger logger, string getSteamUsersEndpoint)
        {
            Client = new HttpClient();
            endpointUri = getSteamUsersEndpoint;
            _logger = logger;
        }

        /// <summary>
        /// Gets users from SteamUserOperator.
        /// </summary>
        /// <exception cref="HttpRequestException"></exception>
        /// <param name="steamIds"></param>
        /// <returns></returns>
        public async Task<List<SteamUser>> GetUsers(List<long> steamIds)
        {
            steamIds = steamIds.Distinct().ToList();

            var queryString = endpointUri + "?steamIds=" + String.Join(steamIds.ToString(), ',');
            var response = await Client.GetAsync(queryString);

            // throw exception if response is not succesful
            if (!response.IsSuccessStatusCode)
            {
                var msg = $"Getting users from SteamUserOperator failed for query [ {queryString} ]. Response: {response}";
                throw new HttpRequestException(msg);
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<SteamUser>>(json);
        }


        /// <summary>
        /// Gets user from SteamUserOperator. 
        /// If possible use GetUsers instead, as querying one-by-one is inefficient.
        /// </summary>
        /// <exception cref="HttpRequestException"></exception>
        /// <param name="steamIds"></param>
        /// <returns></returns>
        public async Task<SteamUser> GetUser(long steamId)
        {
            return (await GetUsers(new List<long> { steamId })).Single();
        }

        /// <summary>
        /// SteamUser, copied from SteamUserOperator repository. Please update accordingly.
        /// </summary>
        public class SteamUser
        {
            public long SteamId { get; set; }
            public string SteamName { get; set; }
            public string ImageUrl { get; set; }
        }
    }

    public class MockSteamUserOperator : ISteamUserOperator
    {
        public async Task<SteamUserOperator.SteamUser> GetUser(long steamId)
        {
            return new SteamUserOperator.SteamUser
            {
                SteamId = steamId,
                ImageUrl = "ImageUrl",
                SteamName = "SteamName"
            };
        }

        public async Task<List<SteamUserOperator.SteamUser>> GetUsers(List<long> steamIds)
        {
            var list = steamIds.Distinct()
                .Select(async x => await GetUser(x))
                .Select(x => x.Result).ToList();
            return list;
        }
    }
}

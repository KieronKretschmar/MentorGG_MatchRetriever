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
        Task<SteamUser> GetUser(long steamId);
        Task<List<SteamUser>> GetUsers(List<long> steamIds);
    }

    /// <summary>
    /// Class for communicating with the SteamUserOperator service.
    /// </summary>
    public class SteamUserOperator : ISteamUserOperator
    {
        private readonly HttpClient Client;
        private readonly string steamUserOperatorUri;
        private readonly ILogger<SteamUserOperator> _logger;

        public SteamUserOperator(ILogger<SteamUserOperator> logger, string steamUserOperatorUri)
        {
            Client = new HttpClient();
            this.steamUserOperatorUri = steamUserOperatorUri;
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

            try
            {
                var queryString = steamUserOperatorUri + "/users?steamIds=" + String.Join(',', steamIds.Select(x=>x.ToString()));
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
            catch (Exception e)
            {
                _logger.LogError("Communication with SteamUserOperator failed. Returning dummy data instead.", e);

                // return dummy data
                return steamIds.Select(x => new SteamUser(x)).ToList();
            }
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
    }

    /// <summary>
    /// SteamUser, copied from SteamUserOperator repository. Please update accordingly.
    /// </summary>
    public class SteamUser
    {
        public long SteamId { get; set; }
        public string SteamName { get; set; }
        public string ImageUrl { get; set; }

        /// <summary>
        /// Constructor with dummy data when no data except steamId is available.
        /// </summary>
        /// <param name="steamId"></param>
        public SteamUser(long steamId)
        {
            SteamId = steamId;
            ImageUrl = null;

            if(steamId > 0)
            {
                SteamName = steamId.ToString();
            }
            else
            {
                SteamName = "Bot";
            }
        }
    }
    

    public class MockSteamUserOperator : ISteamUserOperator
    {
        public async Task<SteamUser> GetUser(long steamId)
        {
            return new SteamUser(steamId);
        }

        public async Task<List<SteamUser>> GetUsers(List<long> steamIds)
        {
            var list = steamIds.Distinct()
                .Select(async x => await GetUser(x))
                .Select(x => x.Result).ToList();
            return list;
        }
    }
}

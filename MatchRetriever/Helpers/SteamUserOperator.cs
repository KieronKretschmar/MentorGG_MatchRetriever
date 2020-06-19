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
        Task<SteamUser> GetUser(long steamId, bool forceRefresh = false);
        Task<List<SteamUser>> GetUsers(List<long> steamIds, bool forceRefresh = false);
    }

    /// <summary>
    /// Class for communicating with the SteamUserOperator service.
    /// </summary>
    public class SteamUserOperator : ISteamUserOperator
    {
        private readonly ILogger<SteamUserOperator> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public SteamUserOperator(
            ILogger<SteamUserOperator> logger,
            IHttpClientFactory clientFactory
            )
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Gets users from SteamUserOperator.
        /// </summary>
        /// <exception cref="HttpRequestException"></exception>
        /// <param name="steamIds"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public async Task<List<SteamUser>> GetUsers(List<long> steamIds, bool forceRefresh)
        {
            if (steamIds.Count == 0)
            {
                return new List<SteamUser>();
            }

            steamIds = steamIds.Distinct().ToList();

            try
            {
                var client = _clientFactory.CreateClient(ConnectedServices.SteamUserOperator);

                HttpRequestMessage message = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"users?steamIds={String.Join(',', steamIds.Select(x => x.ToString()))}&forceRefresh={forceRefresh}");

                var response = await client.SendAsync(message);

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
        public async Task<SteamUser> GetUser(long steamId, bool forceRefresh)
        {
            return (await GetUsers(new List<long> { steamId }, forceRefresh)).Single();
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

            if (steamId > 0)
            {
                SteamName = steamId.ToString();
            }
            else
            {
                SteamName = "Bot";
            }
        }
    }
}

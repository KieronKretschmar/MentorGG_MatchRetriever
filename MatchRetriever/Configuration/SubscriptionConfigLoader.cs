using System;
using System.IO;
using MatchRetriever.Enumerals;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MatchRetriever.Configuration
{
    public interface ISubscriptionConfigLoader
    {
        SubscriptionConfig Config { get; }
    }

    /// <summary>
    /// Responsible for loading the User Subscription Config.
    /// </summary>
    public class SubscriptionConfigLoader: ISubscriptionConfigLoader
    {
        /// <summary>
        /// Location of th configuration file
        /// </summary>
        private const string CONFIG_PATH = "/app/config/subscriptions.json";

        /// <summary>
        /// The SubscriptionConfig instance.
        /// </summary>
        public SubscriptionConfig Config { get; private set;}

        /// <summary>
        /// Logger for internal errors.
        /// </summary>
        public ILogger<SubscriptionConfigLoader> _logger { get; }

        /// <summary>
        /// Instantiate the SubscriptionConfig.
        /// </summary>
        public SubscriptionConfigLoader(
            ILogger<SubscriptionConfigLoader> logger)
        {
            this._logger = logger;
            Config = LoadConfig();

            _logger.LogInformation(
                $"Loaded SubscriptionConfig [ { JsonConvert.SerializeObject(Config) }]");
        }

        /// <summary>
        /// Load the configuration from disk.
        /// </summary>
        /// <returns></returns>
        private SubscriptionConfig LoadConfig()
        {
            using (StreamReader file = File.OpenText(CONFIG_PATH))
            {
                try 
                {
                    var fileContent = file.ReadToEnd();
                    return JsonConvert.DeserializeObject<SubscriptionConfig>(fileContent);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to deserialize SubscriptionConfig");
                    throw;
                }
            }
        }
    }
}
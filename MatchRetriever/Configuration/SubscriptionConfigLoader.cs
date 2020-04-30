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

    /// <summary>
    /// Mock Subscription Config Loader, For testing.
    /// </summary>
    public class MockedSubscriptionConfigLoader : ISubscriptionConfigLoader
    {
        private SubscriptionConfig defaultConfig => new SubscriptionConfig 
        {
            Free = new SubscriptionSettings {
                    DailyMatchesLimit = 3,
                    MatchAccessDurationInDays = 14,
                    PositionFramesPerSecond = 1
            },
            Premium = new SubscriptionSettings {
                    DailyMatchesLimit = 82,
                    MatchAccessDurationInDays = -1,
                    PositionFramesPerSecond = 4
            },
            Ultimate = new SubscriptionSettings {
                    DailyMatchesLimit = -1,
                    MatchAccessDurationInDays = -1,
                    PositionFramesPerSecond = 8
            }
        };
        public SubscriptionConfig Config {get;}

        /// <summary>
        /// Create a MockedSubscriptionConfigLoader with a default configuration.
        /// </summary>
        public MockedSubscriptionConfigLoader(){
            Config = defaultConfig;
        }

        /// <summary>
        /// Create a MockedSubscriptionConfigLoader with a defined configuration.
        /// </summary>
        public MockedSubscriptionConfigLoader(SubscriptionConfig subscriptionConfig){
            Config = subscriptionConfig;
        }
    }
}
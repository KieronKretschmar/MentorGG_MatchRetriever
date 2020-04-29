using System;
using System.IO;
using MatchRetriever.Enumerals;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MatchRetriever.Configuration
{
    /// <summary>
    /// Responsible for loading the User Subscription Config.
    /// </summary>
    public class SubscriptionConfigLoader
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
        }

        /// <summary>
        /// Return the corresponding SubscriptionSetting for a SubscriptionType
        /// </summary>
        public SubscriptionSettings SettingFromSubscriptionType(SubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case SubscriptionType.Free:
                    return Config.Free;
                case SubscriptionType.Premium:
                    return Config.Premium;
                case SubscriptionType.Ultimate:
                    return Config.Ultimate;
                default:
                    throw new InvalidOperationException("Unknown SubscriptionType!");
            }
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
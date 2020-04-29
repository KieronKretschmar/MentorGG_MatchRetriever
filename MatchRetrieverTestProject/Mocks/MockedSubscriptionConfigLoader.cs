using System;
using MatchRetriever.Configuration;
using MatchRetriever.Enumerals;

namespace MatchRetrieverTestProject.Mocks
{
    public class MockedSubscriptionConfigLoader : ISubscriptionConfigLoader
    {
        public SubscriptionConfig Config {get; private set;}

        /// <summary>
        /// Create the Mock SubscriptionConfigLoader
        /// Only configure the `Free` SubscriptionSettings.
        /// </summary>
        /// <param name="config"></param>
        public MockedSubscriptionConfigLoader(SubscriptionConfig config)
        {
            Config = config;
        }
        public SubscriptionSettings SettingFromSubscriptionType(SubscriptionType subscriptionType)
        {
            return Config.Free;
        }
    }
}
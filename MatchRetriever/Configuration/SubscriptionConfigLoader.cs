using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MatchRetriever.Configuration
{
    public class SubscriptionConfigLoader
    {
        public SubscriptionConfig Config { get; private set;}
        public ILogger<SubscriptionConfigLoader> _logger { get; }
        public SubscriptionConfigLoader(ILogger<SubscriptionConfigLoader> logger)
        {
            this._logger = logger;

            using (StreamReader file = File.OpenText("/app/config/subscriptions.json"))
            {
                var fileContent = file.ReadToEnd();
                Config = JsonConvert.DeserializeObject<SubscriptionConfig>(fileContent);
            }
        }
    }

    public class SubscriptionConfig
    {
        public SubscriptionType Free { get; set; }

        public SubscriptionType Premium { get; set; }

        public SubscriptionType Ultimate { get; set; }

    }

    public class SubscriptionType
    {
        public int MatchAccessDurationInDays { get; set; }

        public int DailyMatchesLimit { get; set; }

        public int PositionFramesPerSecond { get; set; }
        
        
    }
}
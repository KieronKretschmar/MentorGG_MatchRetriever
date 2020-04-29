namespace MatchRetriever.Configuration
{
    public class SubscriptionConfig
    {
        public SubscriptionSettings Free { get; set; }

        public SubscriptionSettings Premium { get; set; }

        public SubscriptionSettings Ultimate { get; set; }

    }

    public class SubscriptionSettings
    {
        public int MatchAccessDurationInDays { get; set; }

        public int DailyMatchesLimit { get; set; }

        public int PositionFramesPerSecond { get; set; }
          
    }
}
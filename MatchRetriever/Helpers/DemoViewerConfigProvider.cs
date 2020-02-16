using MatchRetriever.Enumerals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Helpers
{
    public interface IDemoViewerConfigProvider
    {
        DemoViewerConfig GetHighestAvailableConfig(DemoViewerQuality maxQuality, int maxFramesPerSecond);
    }

    public class DemoViewerConfigProvider : IDemoViewerConfigProvider
    {
        private readonly List<DemoViewerConfig> Configs = new List<DemoViewerConfig>
        {
            new DemoViewerConfig
            {
                Quality = DemoViewerQuality.Low,
                FramesPerSecond = 1,
            },
            new DemoViewerConfig
            {
                Quality = DemoViewerQuality.Medium,
                FramesPerSecond = 8,
            },
            new DemoViewerConfig
            {
                Quality = DemoViewerQuality.High,
                FramesPerSecond = 16,
            },
        };

        /// <summary>
        /// Returns the highest quality that satisfies the requirements.
        /// </summary>
        /// <param name="maxFramesPerSecond"></param>
        /// <returns></returns>
        public DemoViewerConfig GetHighestAvailableConfig(DemoViewerQuality maxQuality, int maxFramesPerSecond)
        {
            return Configs.OrderByDescending(x => x.Quality).First(x => x.Quality <= maxQuality && x.FramesPerSecond <= maxFramesPerSecond);
        }
    }

    public struct DemoViewerConfig
    {
        public DemoViewerQuality Quality { get; set; }
        public int FramesPerSecond { get; set; }
    }
}

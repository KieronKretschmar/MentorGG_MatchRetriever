using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Enumerals
{
    /// <summary>
    /// Available options for DemoViewerQuality.
    /// The values of this enum are in order from low to high.
    /// </summary>
    public enum DemoViewerQuality : byte
    {
        Low = 10,
        Medium = 20,
        High = 30,

    }

    public static class AnalyzerQualityExtensions
    {
        public static DemoViewerQuality ToDemoViewerQuality(this AnalyzerQuality analyzerQuality)
        {
            return (DemoViewerQuality)analyzerQuality;
        }
    }
}

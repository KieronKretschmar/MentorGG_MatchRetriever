using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    /// <summary>
    /// Provides means to load PlayerSamples of the given type of TSample from the database.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    public interface ISampleFactory<TSample>
        where TSample : ISample
    {
        Task<List<TSample>> LoadPlayerSamples(long steamId, string map, List<long> matchIds);
    }
}

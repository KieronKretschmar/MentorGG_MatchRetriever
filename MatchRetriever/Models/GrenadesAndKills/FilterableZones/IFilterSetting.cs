using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Used for identification of a sample with a particular context by which filtering should be possible.
    /// Example for a context aspect: The state of the bomb (planted/not planted).
    /// </summary>
    public interface IFilterSetting<TFilterSetting>
        where TFilterSetting : IFilterSetting<TFilterSetting>
    {
        /// <summary>
        /// Returns boolean whether another FilterSetting is allowed in this KillFilterSetting 
        /// </summary>
        /// <param name="sampleKillFilterSetting"></param>
        /// <returns></returns>
        public bool ContainsSetting(TFilterSetting filterSetting);
    }
}

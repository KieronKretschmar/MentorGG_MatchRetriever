using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class KillFilterSetting : IFilterSetting<KillFilterSetting>
    {
        public byte PlantStatus { get; set; }

        public KillFilterSetting(PlantFilterStatus plantStatus)
        {
            PlantStatus = (byte) plantStatus;
        }

        /// <summary>
        /// Returns boolean whether a sample's KillFilterSetting is allowed in this KillFilterSetting 
        /// </summary>
        /// <param name="filterSetting"></param>
        /// <returns></returns>
        public bool ContainsSetting(KillFilterSetting filterSetting)
        {
            return (PlantStatus == (byte)PlantFilterStatus.Irrelevant || filterSetting.PlantStatus == PlantStatus);
        }


        public enum PlantFilterStatus
        {
            Irrelevant = 0,
            NotYetPlanted = 1,
            AfterPlant = 2,
        };
    }
}

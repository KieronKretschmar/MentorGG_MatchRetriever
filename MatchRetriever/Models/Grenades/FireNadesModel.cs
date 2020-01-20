using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Database;
using MatchRetriever.Models;
using System.Threading.Tasks;
using MatchRetriever.Helpers.Trajectories;

namespace MatchRetriever.Models.Grenades
{
    public class FireNadesModel
    {
        public string Map { get; set; }
        public List<FireNadeSample> Samples { get; set; }
    }



    public class FireNadeEntityPerformance
    {
        public long CtRounds { get; set; }
        public long TerroristRounds { get; set; }

        public Dictionary<int, FireNadeZonePerformance> ZonePerformances { get; set; }
    }
}

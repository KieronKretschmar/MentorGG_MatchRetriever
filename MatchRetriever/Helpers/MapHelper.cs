using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities.Enums;

namespace MatchRetriever.Helpers
{
    public class MapHelper
    {
        private static readonly Dictionary<string, Map> Maps = new Dictionary<string, Map>()
        {
            { "de_dust2", new Map(true, 100000) },
            { "de_mirage", new Map(true, 200000) },
            { "de_nuke", new Map(true, 300000) },
            { "de_inferno", new Map(true, 400000) },
            { "de_cache", new Map(true, 500000) },
            { "de_overpass", new Map(true, 600000) },
            { "de_cbble", new Map(false, 700000) },
            { "de_train", new Map(true, 800000) },
            { "de_aztec", new Map(false, 900000) },
            { "de_vertigo", new Map(true, 1100000) },
            { "de_dust", new Map(false, 1200000) },
            { "cs_italy", new Map(false, 1300000) },
            { "cs_office", new Map(false, 1400000) },
            { "cs_agency", new Map(false, 1500000) },
            { "de_canals", new Map(false, 1600000) },
            { "de_shipped", new Map(false, 1700000) },
            { "de_thrill", new Map(false, 1800000) },
            { "de_lite", new Map(false, 1900000) },
            { "cs_insertion", new Map(false, 2000000) },
            { "de_blackgold", new Map(false, 2100000) },
            { "de_austria", new Map(false, 2200000) },
        };

        /// <summary>
        /// Returns a list of all selectable maps, i.e. maps that have a radar image and appear in Overviews 
        /// </summary>
        public static List<string> SelectableMaps => Maps.Where(x => x.Value.Selectable).Select(x => x.Key).ToList();

        public class Map
        {
            public bool Selectable;
            public int Prefix;

            public Map(bool selectable, int prefix)
            {
                this.Selectable = selectable;
                this.Prefix = prefix;
            }
        }

        public static bool IsCtZone(int zoneId)
        {
            int prefix = zoneId / 10000;
            int teamNumber = prefix % 10;
            return teamNumber == (int) StartingFaction.CtStarter;
        }
    }
}

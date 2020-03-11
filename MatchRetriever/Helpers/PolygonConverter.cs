using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MatchRetriever.Helpers
{
    /// <summary>
    /// Converts values of type Polygon to 'string' upon serialization. 
    /// </summary>
    public class PolygonConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Deserialization of Polygons is not yet implemented");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Polygon).Equals(objectType);
        }

        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// Converts a Polygon to [{"X":-2454.2336,"Y":66.837395},{"X":-2664.1128,"Y":66.837395},…]
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vectors = ((Polygon)value).Coordinates.Select(x => new Vector2((float)x.X, (float)x.Y)).ToList();
            serializer.Serialize(writer, vectors);
        }
    }
}

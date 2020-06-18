using System;
using System.Runtime.Serialization;

namespace MatchRetriever.Exceptions
{
    public class MatchNotFoundException : Exception
    {
        public MatchNotFoundException()
        {
        }

        public MatchNotFoundException(string message) : base(message)
        {
        }
    }
}
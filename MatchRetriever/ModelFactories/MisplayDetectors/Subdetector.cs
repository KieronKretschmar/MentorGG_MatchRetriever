using MatchRetriever.Models;
using System;
using System.Collections.Generic;

namespace MatchRetriever.Misplays
{
    public interface ISubdetector
    {
        public Type Type { get;}
        public ISituationCollection ComputeMisplays(long steamId, List<long> matchIds);
    }

    public abstract class Subdetector<TMisplay> : ISubdetector where TMisplay : Misplay
    {
        public Type Type => typeof(TMisplay);

        public abstract ISituationCollection ComputeMisplays(long steamId, List<long> matchIds);

    }
}
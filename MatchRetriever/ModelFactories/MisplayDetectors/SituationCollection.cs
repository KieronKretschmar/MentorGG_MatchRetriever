using MatchRetriever.ModelFactories;
using System;
using System.Collections.Generic;

namespace MatchRetriever.Misplays
{

    public interface ISituationCollection
    {
        public string Name { get; set; }
        public Type Type { get; }
        public List<Misplay> Misplays { get; set; }
    }

    public class SituationCollection<TMisplay> : ISituationCollection where TMisplay : Misplay
    {
        public string Name { get; set; }

        public List<Misplay> Misplays { get; set; }
        public Type Type => typeof(TMisplay);

        public SituationCollection()
        {
            Misplays = new List<Misplay>();
            Name = typeof(TMisplay).Name;
        }
    }
}
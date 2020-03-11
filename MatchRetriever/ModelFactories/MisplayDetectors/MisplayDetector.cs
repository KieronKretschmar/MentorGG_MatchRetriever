using MatchRetriever.Models;
using System;
using System.Collections.Generic;

namespace MatchRetriever.Misplays
{
    internal interface IMisplayDetector
    {
        List<ISituationCollection> DetectMisplays(long steamId, long matchId);
    }

    public class MisplayDetector : IMisplayDetector
    {
        private List<ISubdetector> _subdetectors;

        public MisplayDetector(List<ISubdetector> subdetectors)
        {
            _subdetectors = subdetectors;
        }

        public List<ISituationCollection> DetectMisplays(long steamId, long matchId)
        {
            var res = new List<ISituationCollection>();

            foreach (var detector in _subdetectors)
            {
                var collection = detector.ComputeMisplays(steamId, matchId);
                collection = detector.FilterOutByConfig(collection);
                res.Add(collection);
            }

            return res;
        }
    }
}
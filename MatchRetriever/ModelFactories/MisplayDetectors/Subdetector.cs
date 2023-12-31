﻿using Database;
using MatchRetriever.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MatchRetriever.Misplays
{
    public interface ISubdetector
    {
        public Type Type { get;}
        public ISituationCollection ComputeMisplays(long steamId, long matchId);
        public ISituationCollection FilterOutByConfig(ISituationCollection collection);

    }

    public abstract class Subdetector<TMisplay> : ISubdetector where TMisplay : Misplay
    {
        protected MatchContext _context;
        protected _detectorHelpers _detectorHelpers;

        public Subdetector(IServiceProvider sp)
        {
            _context = sp.GetRequiredService<MatchContext>();
            _detectorHelpers = sp.GetRequiredService<_detectorHelpers>();
        }

        public Type Type => typeof(TMisplay);

        public abstract ISituationCollection ComputeMisplays(long steamId, long matchId);

        public abstract ISituationCollection FilterOutByConfig(ISituationCollection collection);
    }
}
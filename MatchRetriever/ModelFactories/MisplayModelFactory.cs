using MatchRetriever.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Misplays;

namespace MatchRetriever.ModelFactories
{
    public class MisplayModelFactory : ModelFactoryBase
    {
        private IMisplayDetector _detector;

        public MisplayModelFactory(IServiceProvider sp) : base(sp)
        {
            _detector = sp.GetRequiredService<IMisplayDetector>();
        }


        public async Task<MisplaysModel> GetModel(long steamId, List<long> matchIds, int offset)
        {
            matchIds = matchIds.Skip(offset).ToList();
            List<ISituationCollection> SituationCollections = _detector.DetectMisplays( steamId, matchIds);
            return new MisplaysModel { SituationCollections = SituationCollections };
        }
    }
}

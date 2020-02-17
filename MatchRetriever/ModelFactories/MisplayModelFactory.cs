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


        public async Task<MisplaysModel> GetModel(long steamId, long matchId)
        {
            List<ISituationCollection> SituationCollections = _detector.DetectMisplays( steamId, matchId);
            return new MisplaysModel { SituationCollections = SituationCollections };
        }
    }
}

using MatchRetriever.Models.Grenades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public interface ISampleModelFactory<TSample>
        where TSample : IGrenadeSample
    {
        //Task<ISampleModel<TSample>> GetModel(long steamId, string map, List<long> matchIds);
    }

    public abstract class SampleModelFactory<TSample> : ModelFactoryBase, ISampleModelFactory<TSample> 
        where TSample : IGrenadeSample
    {
        public SampleModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected abstract Task<List<TSample>> PlayerSamples(long steamId, string map, List<long> matchIds);
    }
}

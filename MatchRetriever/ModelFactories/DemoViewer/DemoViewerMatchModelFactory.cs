using MatchRetriever.Models;
using MatchRetriever.Models.DemoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.DemoViewer
{
    public interface IDemoViewerMatchModelFactory
    {
        Task<DemoViewerMatchModel> GetModel(long matchId);
    }

    public class DemoViewerMatchModelFactory : ModelFactoryBase, IDemoViewerMatchModelFactory
    {
        public DemoViewerMatchModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<DemoViewerMatchModel> GetModel(long matchId)
        {
            return new DemoViewerMatchModel
            {
            };
        }
    }
}

using Database;
using MatchRetriever.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories
{
    public class ModelFactoryBase
    {
        protected readonly ILogger<ModelFactoryBase> _logger;
        protected readonly ISteamUserOperator _steamUserOperator;
        protected readonly MatchContext _context;
        protected readonly IServiceProvider _sp;

        public ModelFactoryBase(IServiceProvider sp)
        {
            _logger = sp.GetRequiredService<ILogger<ModelFactoryBase>>();
            _steamUserOperator = sp.GetRequiredService<ISteamUserOperator>();
            _context = sp.GetRequiredService<MatchContext>();
            _sp = sp;
        }
    }
}

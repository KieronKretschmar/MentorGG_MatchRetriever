using Database;
using MatchRetriever.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models
{
    /// <summary>
    /// 
    /// This approach of a model with dependency injection is not good, but the quickest to implement right now. Might refactor later.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseModel<T>
    {
        protected readonly ILogger<T> _logger;
        protected readonly ISteamUserOperator _steamUserOperator;
        protected readonly MatchContext _context;
        protected readonly IServiceProvider _sp;

        public BaseModel(IServiceProvider sp)
        {
            _logger = sp.GetRequiredService<ILogger<T>>();
            _steamUserOperator = sp.GetRequiredService<ISteamUserOperator>();
            _context = sp.GetRequiredService<MatchContext>();
            _sp = sp;
        }
    }
}

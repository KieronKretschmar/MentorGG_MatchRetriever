using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace MatchRetriever.Controllers
{
    /// <summary>
    /// Baseclass for all Web ApiController in this project
    /// </summary>
    public class BaseApiController : ApiController
    {
        public BaseApiController()
        {
            CultureInfo modCulture = (CultureInfo)CultureInfo.GetCultureInfo("en-US").Clone();
            modCulture.NumberFormat.NaNSymbol = "n/a";
            Thread.CurrentThread.CurrentCulture = modCulture;
        }
    }
}

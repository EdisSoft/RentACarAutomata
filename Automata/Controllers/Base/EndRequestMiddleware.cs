using FunctionsCore;
using FunctionsCore.Attributes;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Automata.Controllers.Base
{
    public class EndRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public EndRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //Log.Info("Request start");
                // Let the middleware pipeline run
                await _next(context);
            }
            finally
            {
              
                TrackTimeFilter.Delete(context?.TraceIdentifier);
                BaseDbContext.DeleteContext<BaseDbContext>();
              
                //Log.Info("Request end");
            }
        }
    }
}

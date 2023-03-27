using Microsoft.AspNetCore.Http;

namespace FunctionsCore.Contexts
{
    public static class Fonix3HttpContextAccessor
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext CurrentHttpContext => _httpContextAccessor?.HttpContext;
    }
}

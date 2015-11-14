using Microsoft.AspNet.Http;

namespace NHibernate.vNext
{
    public class HttpContext
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static Microsoft.AspNet.Http.HttpContext Current => _httpContextAccessor.HttpContext;
    }
}

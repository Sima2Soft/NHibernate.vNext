using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace NHibernate.vNext
{
    public class DatabaseFactoryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDatabaseFactory _databaseFactory;

        public DatabaseFactoryMiddleware(RequestDelegate next, IDatabaseFactory databaseFactory)
        {
            _next = next;
            _databaseFactory = databaseFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = _databaseFactory.BeginRequest();
            await _next.Invoke(context);
            request.Finish();
        }

    }
}
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.DependencyInjection;

namespace NHibernate.vNext
{
    public class SessionRequiredAttribute : ActionFilterAttribute
    {
        private readonly bool _openTransaction;
        private readonly bool _verifyModelStateError;

        public SessionRequiredAttribute(bool openTransaction = true, bool verifyModelStateError = true)
        {
            _openTransaction = openTransaction;
            _verifyModelStateError = verifyModelStateError;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.ApplicationServices.GetService<IDatabaseFactory>().BeginRequest(_openTransaction);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var factory = filterContext.HttpContext.ApplicationServices.GetService<IDatabaseFactory>();
            
            if (filterContext.Exception != null || (_verifyModelStateError && filterContext.ModelState.ErrorCount > 0))
            {
                factory.EndRequest(true); /*force rollback.*/
                return;
            }

            factory.EndRequest();
        }


    }
}

using System;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.DependencyInjection;

namespace NHibernate.vNext
{
    public class SessionRequiredAttribute : ActionFilterAttribute
    {
        private readonly bool _openTransaction;
        private readonly bool _verifyModelStateError;
        private IDatabaseRequest _request;

        public SessionRequiredAttribute(bool openTransaction = true, bool verifyModelStateError = true)
        {
            _openTransaction = openTransaction;
            _verifyModelStateError = verifyModelStateError;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var factory = filterContext.HttpContext.ApplicationServices.GetService<IDatabaseFactory>();
            _request = factory.BeginRequest(_openTransaction);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null || (_verifyModelStateError && filterContext.ModelState.ErrorCount > 0))
            {
                _request.Finish(true); /*force rollback.*/
                return;
            }

            _request.Finish();
        }
    }
}

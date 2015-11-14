using System.Collections;
using NHibernate.Context;
using NHibernate.Engine;

namespace NHibernate.vNext
{
    public class AspNetvNextWebSessionContext : MapBasedSessionContext
    {
        private const string SessionFactoryMapKey = "NHibernate.Context.WebSessionContext.SessionFactoryMapKey";

        public AspNetvNextWebSessionContext(ISessionFactoryImplementor factory) : base(factory) { }

        protected override IDictionary GetMap()
        {
            return HttpContext.Current.Items[SessionFactoryMapKey] as IDictionary;
        }

        protected override void SetMap(IDictionary value)
        {
            HttpContext.Current.Items.Add(SessionFactoryMapKey, value);
        }

    }
}

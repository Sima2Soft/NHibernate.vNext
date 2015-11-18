using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Context;

namespace NHibernate.vNext
{
    public class DatabaseRequest : IDatabaseRequest
    {
        private readonly ISessionFactory _sessionFactory;

        public DatabaseRequest(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        internal virtual IDatabaseRequest Open(bool beginTransaction = true)
        {
            ISession session = _sessionFactory.OpenSession();

            if (beginTransaction) session.BeginTransaction();

            CurrentSessionContext.Bind(session);

            return this;

        }

        public void BeginTransaction()
        {
            _sessionFactory.GetCurrentSession().Transaction.Begin();
        }

        public virtual void CommitTransaction()
        {
            _sessionFactory.GetCurrentSession().Transaction.Commit();
        }

        public virtual void RollbackTransaction()
        {
            _sessionFactory.GetCurrentSession().Transaction.Rollback();
        }

        public virtual void Finish(bool forceRollback = false)
        {

            if (!CurrentSessionContext.HasBind(_sessionFactory))
                return;

            var session = CurrentSessionContext.Unbind(_sessionFactory);

            if (session == null) return;

            try
            {
                if (!session.Transaction.IsActive) return;

                if (forceRollback)
                {
                    session.Transaction.Rollback();
                    return;
                }

                session.Transaction.Commit();
            }
            catch (Exception)
            {
                if (session.IsOpen && session.Transaction.IsActive)
                    session.Transaction.Rollback();

                throw;
            }
            finally
            {
                if (session.IsOpen)
                    session.Close();

                session.Dispose();
            }            
        }


        public void Dispose()
        {
            Finish();
        }

    }
}

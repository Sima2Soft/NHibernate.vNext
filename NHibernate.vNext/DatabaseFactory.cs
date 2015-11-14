using System;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate.Cache;
using NHibernate.Context;

namespace NHibernate.vNext
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private static ISessionFactory _sessionFactory;

        public DatabaseFactory(DataConfiguration dataConfiguration)
        {
            _sessionFactory = Fluently.Configure()
                .Database(NHibernateConfiguration.GetDatabaseConfiguration(dataConfiguration))
                .Cache(c => c.UseQueryCache().ProviderClass<HashtableCacheProvider>())
                .CurrentSessionContext<AspNetvNextWebSessionContext>()
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.Load(dataConfiguration.MapAssembly))
                    .Conventions.Add(DefaultLazy.Always()))
                .Mappings(m => m.HbmMappings.AddFromAssembly(Assembly.Load(dataConfiguration.CoreAssembly)))
                .BuildSessionFactory();
        }

        public ISession Session => !CurrentSessionContext.HasBind(_sessionFactory) ? null : _sessionFactory.GetCurrentSession();
        
        public virtual void BeginRequest(bool beginTransaction = true)
        {
            ISession session = _sessionFactory.OpenSession();

            if (beginTransaction) session.BeginTransaction();

            CurrentSessionContext.Bind(session);
        }

        public void BeginTransaction()
        {
            Session.Transaction.Begin();
        }

        public virtual void CommitTransaction()
        {
            Session.Transaction.Commit();
        }

        public virtual void RollbackTransaction()
        {
            Session.Transaction.Rollback();
        }
        
        public virtual void EndRequest(bool errors = false, bool reopen = false)
        {
            var session = CurrentSessionContext.Unbind(_sessionFactory);

            if (session == null) return;

            try
            {
                if (session.Transaction.IsActive)
                {
                    if (errors) //errors in context. Not unbind session.
                        session.Transaction.Rollback();
                    else
                        session.Transaction.Commit();
                }
            }
            catch (Exception exception)
            {
                if (session.IsOpen && session.Transaction.IsActive)
                    session.Transaction.Rollback();

                throw new Exception(exception.Message);
            }
            finally
            {
                if (session.IsOpen)
                    session.Close();

                session.Dispose();
            }

            if (reopen)
                BeginRequest();

        }

    }
}

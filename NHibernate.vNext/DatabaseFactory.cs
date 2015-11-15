using System;
using System.IO;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate.Cache;
using NHibernate.Context;

namespace NHibernate.vNext
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly DataConfiguration _dataConfiguration;
        private static ISessionFactory _sessionFactory;

        public DatabaseFactory(DataConfiguration dataConfiguration)
        {
            _dataConfiguration = dataConfiguration;
        }

        public ISession Session => !CurrentSessionContext.HasBind(_sessionFactory) ? null : _sessionFactory.GetCurrentSession();
        
        public virtual IDatabaseRequest BeginRequest(bool beginTransaction = true)
        {
            if (_sessionFactory == null)
                _sessionFactory = GetConfiguration();

            return new DatabaseRequest(_sessionFactory)
                .Open(beginTransaction);
        }

       
        private Assembly GetAssembly(string name)
        {
            try
            {
                return Assembly.Load(GetAssemblyName(name));
            }
            catch (FileLoadException)
            {
                throw new Exception($"Cannot find assembly name {name}. Please, configure correctly CoreAssembly and MapAssembly onto your config file.");
            }
            
        }

        private string GetAssemblyName(string path)
        {
            try
            {
                if (!path.Contains(",")) return path;

                var position = path.IndexOf(",", StringComparison.Ordinal);
                path = path.Substring(position + 1, path.Length - position - 1).Trim();

            }
            catch
            {
                // ignored
            }

            return path;

        }


        private ISessionFactory GetConfiguration()
        {
            return Fluently.Configure()
                    .Database(NHibernateConfiguration.GetDatabaseConfiguration(_dataConfiguration))
                    .Cache(c => c.UseQueryCache().ProviderClass<HashtableCacheProvider>())
                    .CurrentSessionContext<AspNetvNextWebSessionContext>()
                    .Mappings(m => m.FluentMappings.AddFromAssembly(GetAssembly(_dataConfiguration.MapAssembly))
                        .Conventions.Add(DefaultLazy.Always()))
                    .Mappings(m => m.HbmMappings.AddFromAssembly(GetAssembly(_dataConfiguration.CoreAssembly)))
                    .BuildSessionFactory();


        }


    }
}

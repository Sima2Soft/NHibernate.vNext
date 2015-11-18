using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate.Cache;
using NHibernate.Context;

namespace NHibernate.vNext
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly IDictionary<string, string> _configuration;
        private static ISessionFactory _sessionFactory;

        public DatabaseFactory(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public DatabaseFactory(IDictionary<string, string> configuration)
        {
            _configuration = configuration;
        }


        public ISession Session => !CurrentSessionContext.HasBind(_sessionFactory) ? null : _sessionFactory.GetCurrentSession();
        
        public virtual IDatabaseRequest BeginRequest(bool beginTransaction = true)
        {
            if (_sessionFactory == null)
                _sessionFactory = GetConfiguration();

            return new DatabaseRequest(_sessionFactory)
                .Open(beginTransaction);
        }


        protected Assembly GetAssembly(string name)
        {
            var type = System.Type.GetType(name);

            if (type == null)
                throw new Exception(
                    $"Cannot find assembly name {name}. Please, configure correctly CoreAssembly and MapAssembly onto your config file.");

            return type.Assembly;

        }

        protected Assembly MapAssembly => GetAssembly(_configuration["mappingfluent"]);

        protected virtual ISessionFactory GetConfiguration()
        {
            return Fluently.Configure(new Cfg.Configuration().AddProperties(_configuration))
                .Mappings(m => m.FluentMappings.AddFromAssembly(MapAssembly)
                    .Conventions.Add(DefaultLazy.Always()))
                .BuildSessionFactory();
        }


    }
}

using System;
using FluentNHibernate.Cfg.Db;
using NHibernate.Driver;

namespace NHibernate.vNext
{
    public static class NHibernateConfiguration
    {
        public static IPersistenceConfigurer GetDatabaseConfiguration(DataConfiguration config)
        {
            if (string.IsNullOrEmpty(config.Driver))
                throw new ArgumentException(nameof(config.Driver));

            if (string.IsNullOrEmpty(config.ConnectionString))
                throw new ArgumentException(nameof(config.ConnectionString));

            if (string.IsNullOrEmpty(config.Dialect))
                throw new ArgumentException(nameof(config.Dialect));

            switch (config.Driver.ToLower())
            {
                case "oracle":
                    return
                        GetOracleDialect(config.Dialect, config.UseDriverOracleODAC)
                            .ConnectionString(config.ConnectionString);
                case "mssqlserver":
                    return
                        GetSqlServerDialect(config.Dialect).ConnectionString(config.ConnectionString);
                case "mysql":
                    return
                        GetMySqlDialect(config.Dialect).ConnectionString(config.ConnectionString);
                default:
                    throw new Exception($"Driver '{config.Driver}' not has been found. Options for driver are 'Oracle', 'MsSqlServer' or 'MySql'");

            }
        }

        private static MsSqlConfiguration GetSqlServerDialect(string dialect)
        {
            if (string.IsNullOrEmpty(dialect))
                throw new ArgumentException(nameof(dialect));

            switch (dialect.ToLower())
            {
                case "mssql7":
                    return MsSqlConfiguration.MsSql7;
                case "mssql2000":
                    return MsSqlConfiguration.MsSql2000;
                case "mssql2005":
                    return MsSqlConfiguration.MsSql2005;
                case "mssql2008":
                    return MsSqlConfiguration.MsSql2008;
                default:
                    throw new Exception($"Dialect '{dialect}' not has been found. Options for dialect are 'MsSql7', 'MsSql2000', 'MsSql2005', 'MsSql2008'");
            }
        }

        private static MySQLConfiguration GetMySqlDialect(string dialect)
        {
            if (string.IsNullOrEmpty(dialect))
                throw new ArgumentException(nameof(dialect));

            switch (dialect.ToLower())
            {
                case "mysql":
                    return MySQLConfiguration.Standard;
                default:
                    throw new Exception($"Dialect '{dialect}' not has been found. Options for dialect are 'MySql'");
            }
        }

        private static OracleClientConfiguration GetOracleDialect(string dialect, bool useOracleOdacDriver)
        {

            if (string.IsNullOrEmpty(dialect))
                throw new ArgumentException(nameof(dialect));

            OracleClientConfiguration diac;

            switch (dialect.ToLower())
            {
                case "oracle9":
                    diac = OracleClientConfiguration.Oracle9;
                    break;
                case "oracle10":
                    diac = OracleClientConfiguration.Oracle10;
                    break;
                default:
                    throw new Exception($"Dialect '{dialect}' not has been found. Options for dialect are 'Oracle9' or 'Oracle10'");
            }
            
            if (useOracleOdacDriver)
                diac.Driver<OracleManagedDataClientDriver>();

            return diac;
        }

    }

}

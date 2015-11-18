using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;

namespace NHibernate.vNext
{
    public static  class ConfigurationSectionHelper
    {
        public static IDictionary<string, string> ToDictionary(this IConfigurationSection configurationSection)
        {
            var nhibernateConfiguration = new Dictionary<string, string>();
            configurationSection.Bind(nhibernateConfiguration);
            return nhibernateConfiguration;
        }

    }
}

# NHibernate.vNext
Use NHibernate with Asp.net vNext

# Description

This project contains an implementation of WebSessionContext to Asp.Net vNext beta-8 and utils to create NHibernate Session using FluentNHibernate and vNext configuration file.

# Nuget Package

https://www.nuget.org/packages/NHibernate.vNext/

```nuget 
Install-Package NHibernate.vNext
```

# Usage

config.json

```json
  "dependencies": {
    "Microsoft.AspNet.IISPlatformHandler": "1.0.0-beta8",
    "Microsoft.AspNet.Mvc": "6.0.0-beta8",
    "Microsoft.AspNet.Server.Kestrel": "1.0.0-beta8",
    "Microsoft.AspNet.StaticFiles": "1.0.0-beta8",
    "Microsoft.AspNet.Session": "1.0.0-beta8",
    "Microsoft.Framework.Logging": "1.0.0-beta8",
    "Microsoft.Framework.Logging.Console": "1.0.0-beta8",
    "Microsoft.Framework.Logging.Debug": "1.0.0-beta8",
    "Microsoft.Framework.DependencyInjection": "1.0.0-beta8",
    "Microsoft.Framework.Configuration": "1.0.0-beta8",
    "Microsoft.Framework.Configuration.Json": "1.0.0-beta8",
    "NHibernate": "4.0.4.4000",
    "FluentNHibernate": "2.0.3",
    "NHibernate.vNext": "1.0.2"
  }
```

NHibernate require System.Data, then add to DNX451

```json
"frameworks": {
    "dnx451": {
      "frameworkAssemblies": {
        "System.Data": "4.0.0.0"
      }
    }
  }
```



Create your configuration file on **wwwroot\config.Development.json**

```json
{
  "Data": {
    "ConnectionString": "Data Source=(LocalDB)\\v11.0;AttachDbFilename=|DataDirectory|\\Movies.mdf;Integrated Security=True",
    "Dialect": "mssql2008",
    "Driver": "mssqlserver",
    "UseDriverOracleODAC": false,
    "CoreAssembly":  "Application.Core, Application",
    "MapAssembly":  "Application.Mapping, Application"
  }
}
```

Startup.cs configuration.

```C#
public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IHostingEnvironment env)
        {
            //getting json configuration
            //required Microsoft.Framework.Configuration and Microsoft.Framework.Configuration.Json
            _configuration = new ConfigurationBuilder()
                .SetBasePath(env.WebRootPath)
                .AddJsonFile($"config.{env.EnvironmentName}.json")
                .AddEnvironmentVariables()
                .Build();
        }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ...

            //required Microsoft.AspNet.Session.
            services.AddSession();

            //Registry di container. You can create your own implementation of IDatabaseFactory
            services.AddSingleton<IDatabaseFactory, DatabaseFactory>()                
            .AddSingleton(x => _configuration.Get<DataConfiguration>("Data"));

            return services.BuildServiceProvider();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
           ...
            //configure helper for httpcontext.
            HttpContextHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }
    }
```

```c#
[Route("api/[controller]")]
    public class ExampleController : Controller
    {
        private readonly IDatabaseFactory _databaseFactory;

        public ValuesController(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<User> Get()
        {
            using (_databaseFactory.BeginRequest())
            {
                var users = (from user in _databaseFactory.Session.Query<User>() select user);
                return users.ToList();
            }
        }
    }
```

Or Just use Attribute SessionRequired on your controller to manage (begin/end/commit) NHibernate Session.

```c#
        [HttpGet, SessionRequired]
        public IEnumerable<User> Get()
        {
            return _userRepository.GetUsers();
        }
```

Done.

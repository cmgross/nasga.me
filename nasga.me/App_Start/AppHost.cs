using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using nasga.me.Interfaces;
using nasga.me.Models;
using nasga.me.Services;
using ServiceStack.Configuration;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.WebHost.Endpoints;

[assembly: WebActivator.PreApplicationStartMethod(typeof(nasga.me.App_Start.AppHost), "Start")]

//IMPORTANT: Add the line below to MvcApplication.RegisterRoutes(RouteCollection) in the Global.asax:
//routes.IgnoreRoute("api/{*pathInfo}"); 

/**
 * Entire ServiceStack Starter Template configured with a 'Hello' Web Service and a 'Todo' Rest Service.
 *
 * Auto-Generated Metadata API page at: /metadata
 * See other complete web service examples at: https://github.com/ServiceStack/ServiceStack.Examples
 */

namespace nasga.me.App_Start
{
    //A customizeable typed UserSession that can be extended with your own properties
    //To access ServiceStack's Session, Cache, etc from MVC Controllers inherit from ControllerBase<CustomUserSession>
    public class CustomUserSession : AuthUserSession
    {
        public string CustomProperty { get; set; }
    }

    public class AppHost
        : AppHostBase
    {
        public AppHost() //Tell ServiceStack the name and where to find your web services
            : base("Nasga Athlete Data", typeof(AthleteService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            //Set JSON web services to return idiomatic JSON camelCase properties
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

            //Configure User Defined REST Paths
            Routes
                //.Add<Hello>("/hello")
                //.Add<Hello>("/hello/{Name*}");
            .Add<Athlete>("/athlete/{FirstName}/{LastName}/{Class}");
            //Uncomment to change the default ServiceStack configuration
            //SetConfig(new EndpointHostConfig {
            //});

            //Enable Authentication
            //ConfigureAuth(container);

            IResourceManager appSettings = new AppSettings();
            string athleteKey = appSettings.GetString("AthleteKeyName");
            string athleteFirstNameKey = appSettings.GetString("AthleteFirstNameKey");
            string athleteLastNameKey = appSettings.GetString("AthleteLastNameKey");
            string athleteClassKey = appSettings.GetString("AthleteClassKey");
            int expirationDays = Convert.ToInt16(appSettings.GetString("ConfigurationExpirationDays"));
            List<string> athleteClasses = appSettings.GetList("AthleteClasses").ToList();
            string athleteComboClass = appSettings.GetString("AthleteComboClass");

            List<string> years = new List<string>();
            for (int i = 2009; i <= DateTime.Now.Year; i++)
                years.Add(i.ToString());

            var appConfigManager = new AppConfigManager
            {
                AthleteKey = athleteKey,
                AthleteFirstNameKey = athleteFirstNameKey,
                AthleteLastNameKey = athleteLastNameKey,
                AthleteClassKey = athleteClassKey,
                AthleteClasses = athleteClasses,
                ConfigurationExpirationDays = expirationDays,
                AthleteComboClass = athleteComboClass,
                Years = years.OrderByDescending(y => y).ToList()
            };

            //Register all your dependencies
            //container.Register(new TodoRepository());
            container.Register<IHttpContextBaseWrapper>(c => new HttpContextBaseWrapper());
            container.Register<IProfileManager>(c => new CookieProfileManager(new HttpContextBaseWrapper(), appConfigManager));
            container.Register<IConfigManager>(c => appConfigManager);
            //container.Register<AthleteService>(c => ResolveService<AthleteService>(HttpContext.Current));
            //Set MVC to use the same Funq IOC as ServiceStack
            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
        }

        /* Uncomment to enable ServiceStack Authentication and CustomUserSession
        private void ConfigureAuth(Funq.Container container)
        {
            var appSettings = new AppSettings();

            //Default route: /auth/{provider}
            Plugins.Add(new AuthFeature(() => new CustomUserSession(),
                new IAuthProvider[] {
                    new CredentialsAuthProvider(appSettings), 
                    new FacebookAuthProvider(appSettings), 
                    new TwitterAuthProvider(appSettings), 
                    new BasicAuthProvider(appSettings), 
                })); 

            //Default route: /register
            Plugins.Add(new RegistrationFeature()); 

            //Requires ConnectionString configured in Web.Config
            var connectionString = ConfigurationManager.ConnectionStrings["AppDb"].ConnectionString;
            container.Register<IDbConnectionFactory>(c =>
                new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider));

            container.Register<IUserAuthRepository>(c =>
                new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

            var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
            authRepo.CreateMissingTables();
        }
        */

        public static void Start()
        {
            new AppHost().Init();
        }
    }
}
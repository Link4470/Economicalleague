using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Economicalleague.EntityFramework;

namespace Economicalleague.Api
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // 配置log4net
            log4net.Config.XmlConfigurator.Configure();

            //AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            //EF数据库启动时预热
            using (var dbContext = new EconomicalleagueContainer())
            {
                var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                mappingCollection.GenerateViews(new List<EdmSchemaError>());
            }
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include
            };
        }
    }
}

using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Reflectensions.ExtensionMethods;

namespace middlerApp.API.ExtensionMethods
{
    public static class IEndpointRouteBuilderExtensions
    {

        public static void MapControllersWithAttribute<T>(this IEndpointRouteBuilder endpoints) where T: Attribute
        {

            var ass = typeof(Microsoft.AspNetCore.Mvc.Routing.DynamicRouteValueTransformer).Assembly;
            var type = ass.GetTypes().FirstOrDefault(t => t.Name.Equals("ControllerActionEndpointDataSource"));
            
            var dataSource = endpoints.DataSources.FirstOrDefault(ds => ds.GetType() == type);
            if (dataSource == null)
            {
                dataSource = (EndpointDataSource)endpoints.ServiceProvider.GetRequiredService(type);
                var filteredEndpoints = dataSource.Endpoints.Where(e => e.Metadata.Any(m => m.GetType().Equals<T>()));
                var d = new DefaultEndpointDataSource(filteredEndpoints);
                

                endpoints.DataSources.Add(d);
            }
 
        }

    }
}

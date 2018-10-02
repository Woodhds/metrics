using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAL;
using metrics.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace metrics
{
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var type in Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(Assembly.Load)
                .SelectMany(x => x.DefinedTypes).Where(c => typeof(BaseEntity).IsAssignableFrom(c) && !c.IsAbstract))
            {
                var typeName = type.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {
                    var controllerType = typeof(EntitiesController<>)
                        .MakeGenericType(type).GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                }
            }
        }
    }
}
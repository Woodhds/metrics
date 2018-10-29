using System;
using metrics.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace metrics
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenericControllerNameConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() != 
                typeof(EntitiesController<>))
            {
                return;
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using DAL;
using DAL.Attributes;
using DAL.Models;
using DAL.Services.Abstract;

namespace Core.Services.Concrete
{
    public class ViewConfigService : IViewConfigService
    {
        private ConcurrentDictionary<string, ViewConfig> _configs = new ConcurrentDictionary<string, ViewConfig>();


        public ViewConfig GetConfig(string type)
        {
            if (_configs.TryGetValue(type, out var config))
            {
                return config;
            }

            throw new ArgumentException();
        }

        public ViewConfig GetConfig(Type type)
        {
            return GetConfig(type.Name);
        }

        public ViewConfig GetConfig<T>()
        {
            return GetConfig(typeof(T));
        }

        public void GetConfigs()
        {
            foreach (var type in Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(Assembly.Load)
               .SelectMany(x => x.DefinedTypes).Where(c => typeof(BaseEntity).IsAssignableFrom(c) && !c.IsAbstract &&
                c.GetCustomAttribute<ViewConfigAttribute>() != null))
            {
                var configAttr = type.GetCustomAttribute<ViewConfigAttribute>();
                var name = type.Name;
                var config = new ViewConfig
                {
                    Name = name,
                    LookupProperty = configAttr?.LookupProperty,
                    Columns = type.GetProperties().Select(c => new { p = c, attr = c.GetCustomAttribute<ListViewAttribute>() })
                    .Where(z => z.attr != null)
                    .Select(c => new ColumnView()
                    {
                        Type = GetPropertyDataType(c.p.PropertyType),
                        Name = c.p.Name,
                        Title = c.attr.Name
                    }).ToList()
                };
                _configs.TryAdd(type.Name, config);
            }
        }

        public PropertyDataType GetPropertyDataType(Type type)
        {
            if (typeof(int).IsAssignableFrom(type))
                return PropertyDataType.Integer;

            return PropertyDataType.String;
        }
    }
}
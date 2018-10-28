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
            if(_configs.TryGetValue(type, out var config))
            {
                return config;
            }

            throw new ArgumentException();
        }

        public ViewConfig GetConfig(Type type)
        {
            return GetConfig(type.FullName);
        }

        public void GetConfigs()
        {
            foreach (var type in Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(Assembly.Load)
               .SelectMany(x => x.DefinedTypes).Where(c => typeof(BaseEntity).IsAssignableFrom(c) && !c.IsAbstract))
            {
                var config = new ViewConfig
                {
                    Columns = type.GetProperties().Select(c => new { p = c, attr = c.GetCustomAttribute<ListViewAttribute>() })
                    .Where(z => z.attr != null)
                    .Select(c => new ColumnView()
                    {
                        Name = c.p.Name
                    }).ToList()
                };
                _configs.TryAdd(type.FullName, config);
            }
        }
    }
}
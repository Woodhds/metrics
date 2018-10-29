using System;
using System.Collections.Generic;
using System.Text;
using DAL.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class ViewConfigInitializer
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            var configService = serviceProvider.GetRequiredService<IViewConfigService>();
            configService.GetConfigs();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public static class DataBaseInitializer
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DbContext>();
            context.Database.Migrate();
        }
    }
}

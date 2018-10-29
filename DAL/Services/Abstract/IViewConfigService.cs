using DAL.Models;
using System;

namespace DAL.Services.Abstract
{
    public interface IViewConfigService
    {
        ViewConfig GetConfig(string type);
        ViewConfig GetConfig(Type type);

        ViewConfig GetConfig<T>();
        void GetConfigs();
    }
}
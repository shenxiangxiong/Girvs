﻿using Girvs.Cache.Configuration;
using Girvs.Infrastructure;

namespace Girvs.Cache.Extensions
{
    public static class EngineExtension
    {
        public static CacheConfig GetCacheConfig(this IEngine engine)
        {
            return engine.GetAppModuleConfig<CacheConfig>();
        }
    }
}
﻿using System;
using System.Linq;
using System.Net;
using Girvs.Configuration;
using Girvs.FileProvider;
using Girvs.TypeFinder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Girvs.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static (IEngine, AppSettings) ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            CommonHelper.DefaultFileProvider = new GirvsFileProvider(webHostEnvironment);
            services.AddHttpContextAccessor();

            var appSettings = new AppSettings();
            services.AddBindAppModelConfiguation(configuration, appSettings);

            var engine = EngineContext.Create();

            engine.ConfigureServices(services, configuration);

            return (engine, appSettings);
        }

        public static TConfig AddConfig<TConfig>(this IServiceCollection services, IConfiguration configuration)
            where TConfig : class, IConfig, new()
        {
            var config = new TConfig();
            configuration.Bind(config.Name, config);
            services.AddSingleton(config);
            return config;
        }


        public static void AddBindAppModelConfiguation(this IServiceCollection services, IConfiguration configuration,
            AppSettings appSettings)
        {
            configuration.Bind(appSettings);
            appSettings.PreLoadModelConfig();

            var typeFinder = new WebAppTypeFinder();
            var modelSettings = typeFinder.FindClassesOfType<IAppModuleConfig>(true);


            var instances = modelSettings
                .Select(startup => (IAppModuleConfig)Activator.CreateInstance(startup));

            foreach (var appModelConfig in instances)
            {
                var nodeName = appModelConfig?.GetType().Name ?? "TempModel";
                configuration.GetSection($"ModuleConfigurations:{nodeName}").Bind(appModelConfig);
                appSettings.ModuleConfigurations.Add(nodeName, appModelConfig);
            }

            services.AddSingleton(appSettings);
            AppSettingsHelper.SaveAppSettings(appSettings);
        }

        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
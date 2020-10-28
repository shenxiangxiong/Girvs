﻿using Girvs.Domain.Infrastructure;
using Girvs.WebFrameWork.Infrastructure.SpSignalRExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Girvs.WebFrameWork.Infrastructure
{
    public class GirvsStartup : IGirvsStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSpSignalRServer();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseAutoSignalRServices();
        }

        public void EndpointRouteBuilder(IEndpointRouteBuilder builder)
        {
            
        }

        public int Order { get; } = int.MaxValue;
    }
}
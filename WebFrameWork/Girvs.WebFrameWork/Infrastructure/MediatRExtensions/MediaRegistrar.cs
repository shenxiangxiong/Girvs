﻿using System;
using System.Linq;
using Girvs.Domain.Caching.CommandHandlers;
using Girvs.Domain.Caching.Commands;
using Girvs.Domain.Configuration;
using Girvs.Domain.Driven.Commands;
using Girvs.Domain.Infrastructure.DependencyManagement;
using Girvs.Domain.TypeFinder;
using Girvs.WebFrameWork.Infrastructure.ServicesExtensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Girvs.WebFrameWork.Infrastructure.MediatRExtensions
{
    public class MediaRegistrar : IDependencyRegistrar
    {
        public void Register(IServiceCollection services, ITypeFinder typeFinder, GirvsConfig config)
        {
            services.AddMediatR(typeof(MediaRegistrar));

            //services.AddScoped<IRequestHandler<RemoveByKeyCommand, bool>, RemoveByKeyCommandHandler>();
            RegisterType(services, typeof(INotificationHandler<>), typeFinder);
            RegisterType(services, typeof(CommandHandler), typeFinder);
        }


        public void RegisterType(IServiceCollection services, Type type, ITypeFinder typeFinder)
        {
            var types = typeFinder.FindClassesOfType(type, false, true);
            var interFaceTypes = types.Where(x => x.Name != type.Name).ToList();
            foreach (var repositoryType in interFaceTypes)
            {
                var implementedInterfaces = ((System.Reflection.TypeInfo)repositoryType).ImplementedInterfaces;
                if (implementedInterfaces != null && implementedInterfaces.Count() > 0)
                {
                    foreach (var bcType in implementedInterfaces)
                    {
                        services.AddScoped(bcType, repositoryType);
                    }
                }
            }
        }

        public int Order { get; } = 108;
    }
}
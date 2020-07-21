﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Girvs.Domain.Caching.Commands;
using Girvs.Domain.Caching.Interface;
using MediatR;

namespace Girvs.Domain.Caching.CommandHandlers
{
    public class RemoveByPrefixCommandHandler: IRequestHandler<RemoveByPrefixCommand,bool>
    {
        private readonly IStaticCacheManager _staticCacheManager;

        public RemoveByPrefixCommandHandler(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager ?? throw new ArgumentNullException(nameof(staticCacheManager));
        }

        public Task<bool> Handle(RemoveByPrefixCommand request, CancellationToken cancellationToken)
        {
            _staticCacheManager.Remove(request.Prefix);
            return Task.FromResult(true);
        }
    }
}
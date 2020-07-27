﻿using System;
using System.Threading.Tasks;
using Girvs.Application;
using Girvs.Domain.Caching.Interface;
using Girvs.Domain.Driven.Bus;
using Girvs.Domain.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Test.Domain.Commands.User;
using Test.Domain.Extensions;
using Test.Domain.Models;
using Test.Domain.Repositories;
using Test.GrpcService.BaseServices.Public;
using Test.GrpcService.BaseServices.UserGrpcService;
using UserType = Test.Domain.Enumerations.UserType;

namespace Test.Application.Services
{
    [Authorize]
    public class UserService : UserGrpcService.UserGrpcServiceBase, IGrpcService
    {
        private readonly IMediatorHandler _bus;
        private readonly IUserRepository _userRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyManager<User> _cacheKeyManager;

        public UserService(IMediatorHandler bus, IUserRepository userRepository, IStaticCacheManager staticCacheManager,
            ICacheKeyManager<User> cacheKeyManager)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _staticCacheManager = staticCacheManager ?? throw new ArgumentNullException(nameof(staticCacheManager));
            _cacheKeyManager = cacheKeyManager ?? throw new ArgumentNullException(nameof(cacheKeyManager));
        }

        public override async Task<GetByIdResponse> GetById(MainKeyMessage request, ServerCallContext context)
        {
            User user = await _staticCacheManager.GetAsync(
                _cacheKeyManager.BuildCacheEntityKey(request.Id.ToGuid()),
                async () => await _userRepository.GetByIdAsync(request.Id.ToGuid()), 60);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "未找到对应的用户"));

            return new GetByIdResponse
            {
                UserMessage = new UserMessage()
                {
                    UserAccount = user.UserAccount,
                    ContactNumber = user.ContactNumber,
                    Id = user.Id.ToString(),
                    State = (GrpcService.BaseServices.Public.DataState)user.State,
                    UserName = user.UserName,
                    UserType = (GrpcService.BaseServices.Public.UserType)user.UserType
                }
            };
        }

        public override async Task GetAll(Empty request, IServerStreamWriter<GetAllResponse> responseStream,
            ServerCallContext context)
        {
            var list = await _staticCacheManager.GetAsync(_cacheKeyManager.CacheKeyListAllPrefix, async () =>
                await _userRepository.GetAllAsync(), 60);
            foreach (var user in list)
            {
                await responseStream.WriteAsync(new GetAllResponse
                {
                    UserMessage = new UserMessage()
                    {
                        UserAccount = user.UserAccount,
                        ContactNumber = user.ContactNumber,
                        Id = user.Id.ToString(),
                        State = (GrpcService.BaseServices.Public.DataState)user.State,
                        UserName = user.UserName,
                        UserType = (GrpcService.BaseServices.Public.UserType)user.UserType
                    }
                });
            }
        }

        public override async Task<EditResponse> Add(EditRequest request, ServerCallContext context)
        {
            var command = new CreateUserCommand(request.UserMessage.UserAccount,
                request.UserMessage.UserPassword.ToMd5(), request.UserMessage.UserName,
                request.UserMessage.ContactNumber, (Test.Domain.Enumerations.DataState)((int)request.UserMessage.State));

            await _bus.SendCommand(command);

            request.UserMessage.Id = command.Id.ToString();

            return new EditResponse
            {
                UserMessage = request.UserMessage
            };
        }

        public override async Task<EditResponse> Update(EditRequest request, ServerCallContext context)
        {
            var command = new UpdateUserCommand(request.UserMessage.UserAccount,
                request.UserMessage.UserPassword.ToMd5(), request.UserMessage.UserName,
                request.UserMessage.ContactNumber, (Test.Domain.Enumerations.DataState)((int)request.UserMessage.State));

            await _bus.SendCommand(command);

            return new EditResponse
            {
                UserMessage = request.UserMessage
            };
        }

        public override async Task<Empty> Delete(MainKeyMessage request, ServerCallContext context)
        {
            var command = new DeleteUserCommand(request.Id.ToGuid());
            await _bus.SendCommand(command);
            return new Empty();
        }

        public override async Task<Empty> Disable(DisableRequest request, ServerCallContext context)
        {
            var command = new ChangeUserStateCommand(request.Id.ToGuid(), (Test.Domain.Enumerations.DataState)request.State);
            await _bus.SendCommand(command);
            return new Empty();
        }

        public override async Task<Empty> Enable(EnableRequest request, ServerCallContext context)
        {
            var command = new ChangeUserStateCommand(request.Id.ToGuid(), (Test.Domain.Enumerations.DataState)request.State);
            await _bus.SendCommand(command);
            return new Empty();
        }
    }
}
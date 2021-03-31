﻿using System;
using Girvs.Application;
using Power.BasicManagement.Domain.Enumerations;

namespace Power.BasicManagement.Application.ViewModels.User
{
    public class UserEditViewModel : IDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 用户登陆名称
        /// </summary>
        public string UserAccount { get;  set; }

        /// <summary>
        /// 用户登陆密码
        /// </summary>
        public string UserPassword { get;  set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get;  set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactNumber { get;  set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public DataState State { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get;  set; }
    }
}
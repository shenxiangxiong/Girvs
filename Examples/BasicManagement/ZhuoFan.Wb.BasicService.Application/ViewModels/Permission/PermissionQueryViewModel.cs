﻿using System;
using Girvs.AuthorizePermission.Enumerations;

namespace ZhuoFan.Wb.BasicService.Application.ViewModels.Permission
{
    public class PermissionQueryViewModel
    {
        /// <summary>
        /// 指定对角色授权,还是用户授权(0为角色 1为用户)
        /// </summary>
        public PermissionAppliedObjectType PermissionAppliedObjectType { get; set; }

        /// <summary>
        /// 对应功能模块Id
        /// </summary>
        public Guid AppliedID { get; set; }

        public PermissionValidateObjectType PermissionValidateObjectType { get; set; } =
            PermissionValidateObjectType.FunctionMenu;
    }
}
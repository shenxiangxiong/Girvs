﻿using System;
using Girvs.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZhuoFan.Wb.BasicService.Domain.Models;

namespace ZhuoFan.Wb.BasicService.Infrastructure.EntityConfigurations
{
    public class BasalPermissionEntityTypeConfiguration : IEntityTypeConfiguration<BasalPermission>
    {
        public void Configure(EntityTypeBuilder<BasalPermission> builder)
        {
            GirvsDbContext.OnModelCreatingBaseEntityAndTableKey<BasalPermission, Guid>(builder);
            builder.Property(x => x.AppliedObjectType).HasColumnType("int");
            builder.Property(x => x.ValidateObjectType).HasColumnType("int");
            builder.Property(x => x.DenyMask).HasColumnType("bigint");
            builder.Property(x => x.AllowMask).HasColumnType("bigint");
        }
    }
}
﻿using System;
using Girvs.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Girvs.Infrastructure
{
    public abstract class ScsDbContext : DbContext
    {
        protected ScsDbContext(DbContextOptions options)
            : base(options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
        }

        public static void OnModelCreatingBaseEntityAndTableKey<T>(EntityTypeBuilder<T> entity)
            where T : BaseEntity, new()
        {
            string tableName = typeof(T).Name.Replace("Entity", "").Replace("Model","");
            entity.ToTable(tableName).HasKey(x => x.Id);
            entity.Property(x => x.CreateTime).HasColumnType("datetime");
            entity.Property(x => x.UpdateTime).HasColumnType("datetime");
        }
    }
}
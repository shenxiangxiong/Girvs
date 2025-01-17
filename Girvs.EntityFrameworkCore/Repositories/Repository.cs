﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Girvs.BusinessBasis.Entities;
using Girvs.BusinessBasis.Queries;
using Girvs.BusinessBasis.Repositories;
using Girvs.EntityFrameworkCore.Context;
using Girvs.EntityFrameworkCore.DbContextExtensions;
using Girvs.Extensions;
using Girvs.Infrastructure;
using Girvs.TypeFinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Girvs.EntityFrameworkCore.Repositories
{
    public class Repository<TEntity> : Repository<TEntity, Guid>, IRepository<TEntity> where TEntity : BaseEntity<Guid>
    {
    }

    public class Repository<TEntity, Tkey> : IRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        private readonly ILogger<Repository<TEntity, Tkey>> _logger;
        protected DbContext DbContext { get; }
        protected DbSet<TEntity> DbSet { get; }
        protected readonly IRepositoryOtherQueryCondition _repositoryQueryCondition;

        protected Repository()
        {
            _repositoryQueryCondition = EngineContext.Current.Resolve<IRepositoryOtherQueryCondition>();
            _logger = EngineContext.Current.Resolve<ILogger<Repository<TEntity, Tkey>>>() ??
                      throw new ArgumentNullException(nameof(Microsoft.EntityFrameworkCore.DbContext));
            DbContext = GetRelatedDbContext() ??
                        throw new ArgumentNullException(nameof(Microsoft.EntityFrameworkCore.DbContext));
            DbSet = DbContext.Set<TEntity>();
        }

        private DbContext GetRelatedDbContext()
        {
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var ts = typeFinder.FindOfType(typeof(IDbContext));
            return ts.Where(x =>
                    x.GetProperties().Any(propertyInfo => propertyInfo.PropertyType == typeof(DbSet<TEntity>)))
                .Select(x => EngineContext.Current.Resolve(x) as GirvsDbContext).FirstOrDefault();
        }

        private object ConverToTkeyValue(string value)
        {
            if (typeof(Tkey) == typeof(Guid))
            {
                return value.ToHasGuid();
            }

            if (typeof(Tkey) == typeof(Int32))
            {
                return int.Parse(value);
            }
            return value;
        }
        
        public Expression<Func<TEntity, bool>> OtherQueryCondition
        {
            get
            {
                Expression<Func<TEntity, bool>> expression = x => true;

                if (_repositoryQueryCondition != null)
                {
                    expression = expression.And(_repositoryQueryCondition.GetOtherQueryCondition<TEntity>());
                }

                if (!typeof(IIncludeMultiTenant<Tkey>).IsAssignableFrom(typeof(TEntity))) return expression;

                var tenantId = ConverToTkeyValue(EngineContext.Current.ClaimManager.GetTenantId());

                var param = Expression.Parameter(typeof(TEntity), "entity");
                var left = Expression.Property(param, nameof(IIncludeMultiTenant<Tkey>.TenantId));
                var right = Expression.Constant(tenantId);
                
                var be = Expression.Equal(left, right);

                expression = expression.And(Expression.Lambda<Func<TEntity, bool>>(be, param));
                return expression;
            }
        }

        public bool CompareTenantId(TEntity entity)
        {
            if (entity is not IIncludeMultiTenant<Tkey>) return true;
            var tenantId = EngineContext.Current.ClaimManager.GetTenantId();
            var propertyValue = CommonHelper.GetProperty(entity, nameof(IIncludeMultiTenant<Tkey>.TenantId));
            return propertyValue != null && propertyValue.ToString() == tenantId;

        }

        public virtual async Task<TEntity> AddAsync(TEntity t)
        {
            if (!CompareTenantId(t))
            {
                throw new GirvsException("当前租户与数据不一致，无法操作", 568);
            }

            return (await DbSet.AddAsync(t)).Entity;
        }

        public virtual async Task<List<TEntity>> AddRangeAsync(List<TEntity> ts)
        {
            if (ts.Any(entity => !CompareTenantId(entity)))
            {
                throw new GirvsException("当前租户与数据不一致，无法操作", 568);
            }
            
            await DbSet.AddRangeAsync(ts);
            return ts;
        }

        public virtual Task UpdateAsync(TEntity t, params string[] fields)
        {
            if (!CompareTenantId(t))
            {
                throw new GirvsException("当前租户与数据不一致，无法操作", 568);
            }

            return UpdateEntity(t, fields);
        }

        public virtual async Task UpdateRangeAsync(List<TEntity> ts, params string[] fields)
        {
            if (ts.Any(entity => !CompareTenantId(entity)))
            {
                throw new GirvsException("当前租户与数据不一致，无法操作", 568);
            }
            
            foreach (var entity in ts)
            {
                await UpdateEntity(entity, fields);
            }
        }

        public virtual Task DeleteAsync(TEntity t)
        {
            if (!CompareTenantId(t))
            {
                throw new GirvsException("当前租户与数据不一致，无法操作", 568);
            }
            
            DbSet.Remove(t);
            return Task.CompletedTask;
        }

        public virtual Task DeleteRangeAsync(List<TEntity> ts)
        {
            if (ts.Any(entity => !CompareTenantId(entity)))
            {
                throw new GirvsException("当前租户与数据不一致，无法操作", 568);
            }
            
            DbSet.RemoveRange(ts);
            return Task.CompletedTask;
        }

        public virtual async Task<TEntity> GetByIdAsync(Tkey id)
        {
            var entity = await DbSet.FindAsync(id);
            if (!CompareTenantId(entity))
            {
                throw new GirvsException("当前租户与数据不一致，无法操作", 568);
            }

            return entity;
        }

        public virtual Task<List<TEntity>> GetAllAsync(params string[] fields)
        {
            if (fields != null && fields.Any())
            {
                //临时方法，待改进,不科学的方法
                return Task.Run(() =>
                    DbSet.Where(OtherQueryCondition).SelectProperties(fields).ToList());
            }
            else
            {
                return DbSet.Where(OtherQueryCondition).ToListAsync();
            }
        }

        public virtual Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = predicate.And(OtherQueryCondition);
            return DbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetByQueryAsync(QueryBase<TEntity> query)
        {
            var queryCondition = OtherQueryCondition.And(query.GetQueryWhere());

            query.RecordCount = await DbSet.Where(queryCondition).CountAsync();
            if (query.RecordCount < 1)
            {
                query.Result = new List<TEntity>();
            }
            else
            {
                if (query.QueryFields != null && query.QueryFields.Any())
                {
                    //临时方法，待改进,不科学的方法
                    query.Result =
                        await Task.Run(() =>
                            DbSet
                                .Where(queryCondition)
                                .SelectProperties(query.QueryFields)
                                .OrderByDescending(query.OrderBy) //暂时取消排序
                                .Skip(query.PageStart)
                                .Take(query.PageSize)
                                .ToList());
                }
                else
                {
                    query.Result = await DbSet
                        .Where(queryCondition)
                        .OrderByDescending(query.OrderBy) //暂时取消排序
                        .Skip(query.PageStart)
                        .Take(query.PageSize)
                        .ToListAsync();
                }
            }

            return query.Result;
        }

        /// <summary>
        /// 此方法暂时方法，不科学
        /// </summary>
        /// <param name="t">泛型T实例</param>
        /// <param name="fields">指定更新的字段</param>
        private Task UpdateEntity(TEntity t, string[] fields)
        {
            if (t is IIncludeUpdateTime updateTimeEntity)
            {
                updateTimeEntity.UpdateTime = DateTime.Now;
            }

            DbSet.Update(t);
            return Task.CompletedTask;
        }

        public virtual Task<bool> ExistEntityAsync(Tkey id)
        {
            Expression<Func<TEntity, bool>> predicate = t => t.Id.Equals(id);
            predicate = predicate.And(OtherQueryCondition);

            return ExistEntityAsync(predicate);
        }

        public virtual Task<bool> ExistEntityAsync(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = predicate.And(OtherQueryCondition);
            return DbSet.AnyAsync(predicate);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = predicate.And(OtherQueryCondition);
            return DbSet.FirstOrDefaultAsync(predicate);
        }
    }
}
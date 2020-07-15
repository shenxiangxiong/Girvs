﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Girvs.Domain.Managers;
using Girvs.Domain.Models;

namespace Girvs.Domain.IRepositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// 新增或更新实体，当Id为空时则为新增，不为空代表更新
        /// </summary>
        /// <param name="t">实体</param>
        /// <param name="useCache"></param>
        /// <param name="cacheTime"></param>
        /// <returns>是否成功</returns>
        Task<bool> AddAsync(TEntity t);

        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="ts">实体集合</param>
        /// <param name="useCache"></param>
        /// <param name="cacheTime"></param>
        /// <returns>影响的行数</returns>
        Task<int> AddRangeAsync(List<TEntity> ts);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t">实体</param>
        /// <param name="useCache">是否启用缓存</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <param name="fields">需要更新的字段列表</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(TEntity t, params string[] fields);

        /// <summary>
        /// 更新实体集合
        /// </summary>
        /// <param name="ts">实体集合</param>
        /// <param name="useCache">是否启用缓存</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <param name="fields">需要更新的字段列表</param>
        /// <returns>多少条记录被响应</returns>
        Task<int> UpdateRangeAsync(List<TEntity> ts, params string[] fields);

        /// <summary>
        /// 删除指定的主键值的实体
        /// </summary>
        /// <param name="t">实体</param>
        /// <param name="useCache"></param>
        /// <param name="cacheTime"></param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(TEntity t);

        /// <summary>
        /// 根据主值集合删除对实体集
        /// </summary>
        /// <param name="ts">集合</param>
        /// <param name="useCache"></param>
        /// <param name="cacheTime"></param>
        /// <returns>主键集合</returns>
        Task<int> DeleteRangeAsync(List<TEntity> ts);

        /// <summary>
        /// 根据主键值获取相关的实体
        /// </summary>
        /// <param name="id">主健值</param>
        /// <param name="useCache">是否启用缓存</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <returns>对应的实体</returns>
        Task<TEntity> GetByIdAsync(Guid id);

        /// <summary>
        /// 获取所有实体列表集合
        /// </summary>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="cacheTime">缓存时间，默认60分钟</param>
        /// <param name="fields">需要查询的字段列表</param>
        /// <returns>实体列表集合</returns>
        Task<List<TEntity>> GetAllAsync(params string[] fields);

        /// <summary>
        /// 根据查询条件获取集合
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="cacheTime">缓存时间，默认60分钟</param>
        /// <returns>实体集合</returns>
        Task<List<TEntity>> GetByQueryAsync(QueryBase<TEntity> query);

        /// <summary>
        /// 是否存在指定键的实体
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistEntityAsync(Guid id);
    }
}
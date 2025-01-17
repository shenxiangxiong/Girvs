﻿using System;
using System.Linq.Expressions;
using Girvs.BusinessBasis.Entities;

namespace Girvs.BusinessBasis.Repositories
{
    public interface IRepositoryOtherQueryCondition : IManager
    {
        Expression<Func<TEntity, bool>> GetOtherQueryCondition<TEntity>() where TEntity : Entity;
    }
}
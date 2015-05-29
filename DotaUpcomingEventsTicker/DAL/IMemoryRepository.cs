using DotaUpcomingEventsTicker.Api.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.DAL
{
    public interface IMemoryRepository<TEntity> 
        where TEntity : BaseEntity
    {
        bool Add(TEntity entity);
        bool Remove(TEntity entity);
        bool Contains(TEntity entity);
        int Count();
    }
}

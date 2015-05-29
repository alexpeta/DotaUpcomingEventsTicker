using DotaUpcomingEventsTicker.Api.Models;
using DotaUpcomingEventsTicker.Api.Scraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.DAL
{
    public interface IRepository<TEntity> 
        where TEntity :BaseEntity
    {
        List<TEntity> GetList();
        Task<List<TEntity>> GetListAsync();
    }
}

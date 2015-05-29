using DotaUpcomingEventsTicker.Api.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaUpcomingEventsTicker.DAL
{
    public class SimpleMemoryRepository : IMemoryRepository<Match>
    {
        private static readonly ConcurrentDictionary<string, Match> _inMemoryStorage = new ConcurrentDictionary<string, Match>();

        public bool Add(Match entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return _inMemoryStorage.TryAdd(entity.Id, entity);
        }
        public bool Remove(Match entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Match outed;
            return _inMemoryStorage.TryRemove(entity.Id, out outed);
        }
        public int Count()
        {
            return _inMemoryStorage.Count;
        }
        public bool Contains(Match entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return _inMemoryStorage.ContainsKey(entity.Id);
        }
    }
}

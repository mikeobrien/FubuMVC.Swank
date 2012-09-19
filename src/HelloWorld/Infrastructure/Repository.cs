using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloWorld.Infrastructure
{
    public interface IRepository<T> : IEnumerable<T> where T : class, new()
    {
        T Get(Guid id);
        T Add(T entity);
        void Modify(T entity);
        void Delete(Guid id);
    }

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly List<T> _entities;
        private readonly Func<T, Guid> _getId;
        private readonly Action<T, Guid> _setId;

        public Repository(params T[] entities)
        {
            _entities = entities.ToList();
            var property = typeof (T).GetProperty("Id");
            _getId = x => (Guid)property.GetValue(x, null);
            _setId = (x, id) => property.SetValue(x, id, null);
            _entities.Where(x => _getId(x) == Guid.Empty).ToList().ForEach(x => _setId(x, Guid.NewGuid()));
        }

        public static Repository<T> With(params T[] entities)
        {
            return new Repository<T>(entities);
        } 

        public T Get(Guid id)
        {
            return _entities.FirstOrDefault(x => _getId(x) == id);
        }

        public T Add(T entity)
        {
            _setId(entity, Guid.NewGuid());
            _entities.Add(entity);
            return entity;
        }

        public void Modify(T entity)
        {
            Delete(_getId(entity));
            _entities.Add(entity);
        }

        public void Delete(Guid id)
        {
            var entity = Get(id);
            if (entity != null) _entities.Remove(entity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }
    }
}
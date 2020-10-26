using System;
using System.Threading.Tasks;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure
{
    public class RavenDbRepository<T, TId>
        where T : AggregateRoot<TId>
        where TId : Value<TId>
    {
        private readonly Func<TId, string> _entityId;
        private readonly IAsyncDocumentSession _session;

        public RavenDbRepository(
            IAsyncDocumentSession session,
            Func<TId, string> entityId)
        {
            _session = session;
            _entityId = entityId;
        }

        public Task Add(T entity)
        {
            return _session.StoreAsync(entity, _entityId(entity.Id));
        }

        public Task<bool> Exists(TId id)
        {
            return _session.Advanced.ExistsAsync(_entityId(id));
        }

        public Task<T> Load(TId id)
        {
            return _session.LoadAsync<T>(_entityId(id));
        }
    }
}
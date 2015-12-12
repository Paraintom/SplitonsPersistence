using System.Collections.Generic;
using SplitonsPersistence;
using SplitonsPersistence.Persistence;

namespace Tests
{
    class InMemPersister : IPersister
    {
        Dictionary<string, List<UpdatableElement>> cached = new Dictionary<string, List<UpdatableElement>>();
        public void Persist(string projectId, List<UpdatableElement> transactions)
        {
            cached[projectId] = transactions;
        }

        public List<UpdatableElement> Read(string projectId)
        {
            return cached.ContainsKey(projectId) ? cached[projectId] : new List<UpdatableElement>();
        }
    }
}

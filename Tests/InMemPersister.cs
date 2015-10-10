using System.Collections.Generic;
using SplitonsPersistence;
using SplitonsPersistence.Persistence;

namespace Tests
{
    class InMemPersister : IPersister
    {
        Dictionary<string,List<Transaction>> cached = new Dictionary<string, List<Transaction>>();
        public void Persist(string projectId, List<Transaction> transactions)
        {
            cached[projectId] = transactions;
        }

        public List<Transaction> Read(string projectId)
        {
            return cached.ContainsKey(projectId) ? cached[projectId] : new List<Transaction>();
        }
    }
}

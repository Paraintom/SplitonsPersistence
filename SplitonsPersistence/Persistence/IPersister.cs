using System.Collections.Generic;

namespace SplitonsPersistence.Persistence
{
    public interface IPersister
    {
        void Persist(string projectId, List<Transaction> transactions);
        List<Transaction> Read(string projectId);
    }
}

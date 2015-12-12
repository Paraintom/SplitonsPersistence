using System.Collections.Generic;

namespace SplitonsPersistence.Persistence
{
    public interface IPersister
    {
        void Persist(string projectId, List<UpdatableElement> transactions);
        List<UpdatableElement> Read(string projectId);
    }
}

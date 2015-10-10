using System;
using System.Collections.Generic;
using System.Linq;
using SplitonsPersistence.Persistence;

namespace SplitonsPersistence
{
    public class ProjectManager : IProjectManager
    {
        private readonly IPersister persister;
        
        public ProjectManager(IPersister persister)
        {
            this.persister = persister;
        }

        public List<Transaction> Update(string projectId, long from, List<Transaction> toSynchronize)
        {
            lock (string.Intern(projectId))
            {
                var currentState = persister.Read(projectId);
                //first we update the current state
                foreach (var updatedTransaction in toSynchronize)
                {
                    var id = updatedTransaction.id;
                    //It is a new transaction, we just add it, else we might have to remove a previous version.
                    if (currentState.Count(o => o.id == id) != 0)
                    {
                        var existingTransaction = currentState.First(o => o.id == id);
                        if (existingTransaction.lastUpdated < updatedTransaction.lastUpdated)
                        {
                            //Then we update
                            currentState.Remove(existingTransaction);
                            
                        }
                        else
                        {
                            //else the existing transaction is newer, so we keep the newest state.
                            //We ignore the out of date update
                            continue;
                        }
                    }
                    var newStateTransaction = updatedTransaction;
                    newStateTransaction.lastUpdated = DateTime.Now.JavascriptTicks();
                    currentState.Add(newStateTransaction);
                }
                persister.Persist(projectId, currentState);
                return currentState.Where(o=>o.lastUpdated >= from).ToList();
            }
        }
    }
}

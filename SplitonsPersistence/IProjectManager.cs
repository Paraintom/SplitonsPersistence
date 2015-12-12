using System;
using System.Collections.Generic;

namespace SplitonsPersistence
{
    public interface IProjectManager
    {
        /// <summary>
        /// Receive the list of updated transaction and a last updated time and return all the new transactions
        /// </summary>
        /// <param name="projectId">The project Id</param>
        /// <param name="from">Date of last synch</param>
        /// <param name="toSynchronize">List transaction updated client side</param>
        /// <returns>The List of Transaction synch (include the one sent to synch more the potential new ones)</returns>
        List<UpdatableElement> Update(string projectId, long from, List<UpdatableElement> toSynchronize);
    }
}

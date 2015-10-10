using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace SplitonsPersistence.Persistence
{
    public class FilePersister : IPersister
    {
        private Logger log = LogManager.GetCurrentClassLogger();


        public void Persist(string projectId, List<Transaction> transactions)
        {
            try
            {
                //Todo check if it is smart to save it... if the same just return
                log.Debug("Persisting project {0}", projectId);
                //We do this in 3 step :
                //0 we write a temp state file
                string tempFileName = projectId + ".temp";
                var stateFileName = GetStateFileName(projectId);
                if (File.Exists(tempFileName))
                {
                    log.Warn("We still have the temp file, something did not ended well last time we tried to save this project {0}", projectId);
                    File.Delete(tempFileName);
                    //We could try to see, if there is no state file, if this one is correct and continue the process.
                }
                File.AppendAllText(tempFileName, JsonConvert.SerializeObject(transactions));
                //1 we remove the current state if exist
                if (File.Exists(stateFileName))
                {
                    File.Delete(stateFileName);
                }
                //2 we rename the first state file
                File.Move(tempFileName, stateFileName);

            }
            catch (Exception e)
            {
                log.Error(string.Format("Error while persisting state for project [{0}] :", projectId));
                log.Error(e);
                throw;
            }
        }

        public List<Transaction> Read(string projectId)
        {
            List<Transaction> result= new List<Transaction>();;
            try
            {
                log.Debug("Reading project {0}", projectId);
                var stateFileName = GetStateFileName(projectId);
                if (File.Exists(stateFileName))
                {
                    var stringState = File.ReadAllText(stateFileName);
                    var temp = JsonConvert.DeserializeObject<List<Transaction>>(stringState);
                    temp.ForEach(o=>result.Add(o));
                }
                else
                {
                    log.Debug("No file found");
                }
                log.Debug("Found {0} transactions", result.Count);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error while persisting state for project [{0}] :", projectId));
                log.Error(e);
                throw;
            }
            return result;
        }
        private static string GetStateFileName(string projectId)
        {
            return projectId + ".state";
        }
    }
}

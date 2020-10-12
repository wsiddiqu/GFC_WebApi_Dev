using GFC.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL.Interfaces
{
    public interface IPrePopJobsBAL
    {
        List<PrePopJobModel> GetPrePopJobs(string type);
        PrePopJobModel GetPrePopJobDetails(string jobId,string type);
        void CreatePrePopJob(PrePopulationJob jobParams,string type);
        void UpdatePrePopJob(PrePopulationJob jobParams,string type);
        void DeleteAllPrePopJobs(string type);
        void DeletePrePopJobDetails(string jobId, string type);
    }
}

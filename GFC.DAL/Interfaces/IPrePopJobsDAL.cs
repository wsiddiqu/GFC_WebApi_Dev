using GFC.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.DAL.Interfaces
{
    public interface IPrePopJobsDAL
    {
        List<PrePopJobModel> GetPrePopJobs();
        PrePopJobModel GetPrePopJobDetails(string jobId);
        void CreatePrePopJob(PrePopulationJob jobParams);
        void UpdatePrePopJob(PrePopulationJob jobParams);
        void DeleteAllPrePopJobs();
        void DeletePrePopJobDetails(string jobId);
    }
}

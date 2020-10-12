using GFC.BAL.Interfaces;
using GFC.Models;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFC.UnitTest
{
    public class PrePopJobsControllerMock : IPrePopJobsBAL
    {

        private readonly List<PrePopJobModel> _prePopJobModel;
        string type = "XML";

        public PrePopJobsControllerMock()
        {
            _prePopJobModel = new List<PrePopJobModel>();
           PrePopJobModel prePopJob = new PrePopJobModel();
            prePopJob.JobID = "1";
            prePopJob.CacheName = new string[] { "All"};
            _prePopJobModel.Add(prePopJob);
        }
        public void CreatePrePopJob(PrePopulationJob jobParams,string type)
        {
            PrePopJobModel prePopJob = new PrePopJobModel();
            prePopJob.JobID =string.IsNullOrEmpty(jobParams.JobID)? Guid.NewGuid().ToString() : jobParams.JobID;
            prePopJob.CacheName = jobParams.CacheName;
            _prePopJobModel.Add(prePopJob);
        }

        public void DeleteAllPrePopJobs(string type)
        {
            throw new NotImplementedException();
        }

        public void DeletePrePopJobDetails(string jobId,String Type)
        {
            throw new NotImplementedException();
        }

        public PrePopJobModel GetPrePopJobDetails(string jobId,string type)
        {
            PrePopJobModel prePopJob = new PrePopJobModel();
            prePopJob.JobID = "1";
            prePopJob.CacheName = new string[] { "All" };
            return prePopJob;
        }

        public List<PrePopJobModel> GetPrePopJobs(string type)
        {
            return _prePopJobModel;
        }

        public void UpdatePrePopJob(PrePopulationJob jobParams,string type)
        {
            throw new NotImplementedException();
        }
    }
}

using GFC.BAL.Interfaces;
using GFC.DAL.Interfaces;
using GFC.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static GFC.Utility.Common.Helper;

namespace GFC.BAL
{
    public class PrePopJobsBAL : IPrePopJobsBAL
    {

        private IPrePopJobsDAL _prePopJobsDAL;
        private readonly IPrePopJobFactory _prePopJobFactory;

        public PrePopJobsBAL(IPrePopJobFactory prePopJobFactory)
        {
            _prePopJobFactory = prePopJobFactory;
        }

        /// <summary>
        /// Call the CreatePrePopJob of DAL based on type seleted 
        /// </summary>
        /// <param name="jobParams"></param>
        /// <param name="type"></param>
        public void CreatePrePopJob(PrePopulationJob jobParams, string type)
        {
            _prePopJobsDAL = _prePopJobFactory.CreatePrePopJobOperation(type);
            jobParams = GetJobParamsMapping(jobParams);
            _prePopJobsDAL.CreatePrePopJob(jobParams);
        }

        /// <summary>
        /// Call the UpdatePrePopJob of DAL Layer based on type seleted 
        /// </summary>
        /// <param name="jobParams"></param>
        /// <param name="type"></param>
        public void UpdatePrePopJob(PrePopulationJob jobParams, string type)
        {
            _prePopJobsDAL = _prePopJobFactory.CreatePrePopJobOperation(type);
            jobParams = GetJobParamsMapping(jobParams);
            _prePopJobsDAL.UpdatePrePopJob(jobParams);
        }

        /// <summary>
        ///  Call the GetPrePopJobs of DAL Layer based on type seleted 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<PrePopJobModel> GetPrePopJobs(string type)
        {
            _prePopJobsDAL = _prePopJobFactory.CreatePrePopJobOperation(type);
            return _prePopJobsDAL.GetPrePopJobs();
        }

        /// <summary>
        /// Call the GetPrePopJobDetails of DAL Layer based on type seleted 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public PrePopJobModel GetPrePopJobDetails(string jobId, string type)
        {
            _prePopJobsDAL = _prePopJobFactory.CreatePrePopJobOperation(type);
            return _prePopJobsDAL.GetPrePopJobDetails(jobId);
        }

        /// <summary>
        /// Call the DeleteAllPrePopJobs of DAL Layer based on type seleted 
        /// </summary>
        /// <param name="type"></param>
        public void DeleteAllPrePopJobs(string type)
        {
            _prePopJobsDAL = _prePopJobFactory.CreatePrePopJobOperation(type);
            _prePopJobsDAL.DeleteAllPrePopJobs();
        }

        /// <summary>
        /// Call the DeletePrePopJobDetails of DAL Layer based on type seleted 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="type"></param>
        public void DeletePrePopJobDetails(string jobId, string type)
        {
            _prePopJobsDAL = _prePopJobFactory.CreatePrePopJobOperation(type);
            _prePopJobsDAL.DeletePrePopJobDetails(jobId);
        }

        /// <summary>
        /// Method is reponsible to map the parameters as per the input format
        /// </summary>
        /// <param name="jobParams"></param>
        /// <returns></returns>
        private PrePopulationJob GetJobParamsMapping(PrePopulationJob jobParams)
        {
            if (string.IsNullOrEmpty(jobParams.JobID))
            {
                jobParams.JobID = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(jobParams.CacheName[0]))
            {
                jobParams.CacheName[0] = "*All*";
            }
            if (jobParams.Filters.MetadataOnly != 0)
            {
                jobParams.isFilterByModifiedTime = false;
                jobParams.Filters.ExtensionFlag = (int)ExtensionType.NoVal;
                jobParams.Filters.Extensions = string.Empty;
            }
            if (jobParams.isFilterByModifiedTime && (jobParams.ModifiedBy > 0))
            {
                jobParams.Filters.LastModifiedTime = GetModifiedIn(jobParams.ModifiedBy, jobParams.ModifiedIn);
            }
            if (jobParams.Filters.ExtensionFlag.ToString() != "Include" && jobParams.Filters.ExtensionFlag.ToString() != "Exclude")
            {
                jobParams.Filters.Extensions = string.Empty;
                jobParams.Filters.ExtensionFlag = 0;
            }
            if (jobParams.Frequency != null)
            {
                JobFrequency jobFrequency = GetJobFrequency(jobParams.Frequency);
            }
            if (jobParams.StartTime != null)
            {
                jobParams.StartTime = new DateTime(jobParams.StartTime.Year, jobParams.StartTime.Month, jobParams.StartTime.Day, jobParams.StartTime.Hour, jobParams.StartTime.Minute, jobParams.StartTime.Second);
            }
            else
            {
                jobParams.StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }
            if (jobParams.StopTime != null)
            {
                jobParams.StopTime = new DateTime(jobParams.StopTime.Year, jobParams.StopTime.Month, jobParams.StopTime.Day, jobParams.StopTime.Hour, jobParams.StopTime.Minute, jobParams.StopTime.Second);
            }
            else
            {
                jobParams.StopTime = new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            }
            return jobParams;
        }

        /// <summary>
        /// Map the Frequency parameters
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        private JobFrequency GetJobFrequency(JobFrequency frequency)
        {
            DateTime dt = !string.IsNullOrEmpty(frequency.ExecuteAt) ? Convert.ToDateTime(frequency.ExecuteAt) : DateTime.Now;

            if (frequency.JobType == 1)
            {
                frequency.JobType = (uint)FrequencyType.RepeatEvery;
                frequency.Value = (string.IsNullOrEmpty(frequency.Value) ? "days" : frequency.Value);
                frequency.ExecuteAt = dt.Hour.ToString("D2") + ":" + dt.Minute.ToString("D2") + ":" + dt.Second.ToString("D2");
            }
            else if (frequency.JobType == 2)
            {
                frequency.JobType = (uint)FrequencyType.RepeatEveryDayOfWeek;
                frequency.Value = (string.IsNullOrEmpty(frequency.Value) ? "Sunday" : frequency.Value);
                frequency.ExecuteAt = dt.Hour.ToString("D2") + ":" + dt.Minute.ToString("D2") + ":" + dt.Second.ToString("D2");
            }
            else if (frequency.JobType == 3)
            {
                frequency.JobType = (uint)FrequencyType.RepeatEveryDayOfMonth;
                frequency.Value = (string.IsNullOrEmpty(frequency.Value) ? "1st" : frequency.Value);
                frequency.ExecuteAt = dt.Hour.ToString("D2") + ":" + dt.Minute.ToString("D2") + ":" + dt.Second.ToString("D2");
            }
            else
            {
                frequency.JobType = (uint)FrequencyType.OneTime;
                frequency.Value = string.Empty;
                frequency.ExecuteAt = string.Empty;
            }
            return frequency;

        }

        /// <summary>
        /// format the date in expected input format
        /// </summary>
        /// <param name="modifiedBy"></param>
        /// <param name="modifiedIn"></param>
        /// <returns></returns>
        private ulong GetModifiedIn(int modifiedBy, ModifiedIn modifiedIn)
        {
            int r = 0;
            switch (modifiedIn.ToString().ToLower())
            {
                case "minutes":
                    {
                        r = 60 * 1000;
                        break;
                    }
                case "hours":
                    {
                        r = 60 * 60 * 1000;
                        break;
                    }
                case "days":
                    {
                        r = 24 * 60 * 60 * 1000;
                        break;
                    }
            }
            return (ulong)modifiedBy * (ulong)r;
        }
    }
}

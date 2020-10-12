using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GFC.BAL.Interfaces;
using GFC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GFC.Api.Controllers.Configurations.CoreConfiguration
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrePopJobsController : Controller
    {
        protected IConfiguration Configuration;
        protected AppSettings AppSettings { get; set; }
        private readonly IPrePopJobsBAL _prePopJobsBAL;
        string prePopJobFormat = string.Empty;
        public PrePopJobsController(IPrePopJobsBAL prePopJobsBAL, IOptionsSnapshot<AppSettings> settings = null, IConfiguration configuration = null)
        {
            _prePopJobsBAL = prePopJobsBAL;
            if (settings != null)
            {
                AppSettings = settings.Value;
            }
            Configuration = configuration;
            prePopJobFormat = string.IsNullOrEmpty(Configuration.GetSection("AppSettings")["PrePopJobFormat"]) ? "XML" : Configuration.GetSection("AppSettings")["PrePopJobFormat"];
        }

        #region Get Api Endpoints
        /// <summary>
        /// Get All thePre Population Job List
        /// </summary>
        /// <returns> List<PrePopJobModel></returns>
        [HttpGet]
        [Route("GetPrePopList")]
        public ActionResult<List<PrePopJobModel>> GetPrePopulationJobs()
        {
            List<PrePopJobModel> ppJobList;
            ppJobList = _prePopJobsBAL.GetPrePopJobs(prePopJobFormat);
            if (ppJobList == null)
            {
                return BadRequest("No Result Found");
            }
            return Ok(ppJobList);
        }

        /// <summary>
        /// Get the Job Details
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns>PrePopJobModel</returns>
        [HttpGet]
        [Route("GetPrePopJobDetails")]
        public ActionResult GetPrePopJobDetails(string jobId)
        {
            PrePopJobModel prePopJob;
            prePopJob = _prePopJobsBAL.GetPrePopJobDetails(jobId, prePopJobFormat);
            if (prePopJob == null)
            {
                return BadRequest("No Result Found for JobId:" + jobId);
            }
            return Ok(prePopJob);

        }
        #endregion

        #region Create Update Api EndPoints
        /// <summary>
        /// Create Pre Population Job
        /// </summary>
        /// <param name="JobParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreatePrePopulationJob")]
        public ActionResult CreatePrePopulationJob(PrePopulationJob JobParams)
        {
            _prePopJobsBAL.CreatePrePopJob(JobParams, prePopJobFormat);
            return Ok();
        }

        /// <summary>
        /// Update the Pre Population Job
        /// </summary>
        /// <param name="JobParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdatePrePopulationJob")]
        public ActionResult UpdatePrePopulationJob(PrePopulationJob JobParams)
        {
            _prePopJobsBAL.UpdatePrePopJob(JobParams, prePopJobFormat);
            return Ok();
        }
        #endregion

        #region Delete Api EndPoints
        /// <summary>
        /// Delete All the jobs
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "DeleteAllPrePopJobs")]
        [HttpDelete]
        public ActionResult DeleteAllPrePopJobs()
        {
            _prePopJobsBAL.DeleteAllPrePopJobs(prePopJobFormat);
            return Ok();
        }

        /// <summary>
        /// Delete Job Based on Job Id
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [Route("{jobId}", Name = "DeletePrePop")]
        [HttpDelete]
        public ActionResult DeletePrePop(string jobId)
        {
            _prePopJobsBAL.DeletePrePopJobDetails(jobId, prePopJobFormat);
            return Ok();
        }
        #endregion
    }
}

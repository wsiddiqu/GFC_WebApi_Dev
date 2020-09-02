using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GFC.BAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GFC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceOprationController : ControllerBase
    {
        private readonly IServiceOprationBAL _serviceOprationBAL;

        public ServiceOprationController(IServiceOprationBAL serviceOprationBAL)
        {
            _serviceOprationBAL = serviceOprationBAL;
        }

        [HttpPost]
        [Route("StopService")]
        public ActionResult<string> StopService(string serviceName)
        {
            return _serviceOprationBAL.StopService(serviceName);
        }
        [HttpPost]
        [Route("StartService")]
        public ActionResult<string> StartService(string serviceName)
        {
            return _serviceOprationBAL.StartService(serviceName);
        }
        [HttpPost]
        [Route("RestartService")]
        public ActionResult<string> RestartService(string serviceName)
        {
            return _serviceOprationBAL.RestartService(serviceName);
        }
    }
}

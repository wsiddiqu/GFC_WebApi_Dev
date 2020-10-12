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
    public class ServiceOperationController : ControllerBase
    {
        private readonly IServiceOperationBAL _serviceOperationBAL;

        public ServiceOperationController(IServiceOperationBAL serviceOperationBAL)
        {
            _serviceOperationBAL = serviceOperationBAL;
        }

        [HttpPost]
        [Route("StopService")]
        public ActionResult<string> StopService(string serviceName)
        {
            return _serviceOperationBAL.StopService(serviceName);
        }
        [HttpPost]
        [Route("StartService")]
        public ActionResult<string> StartService(string serviceName)
        {
            return _serviceOperationBAL.StartService(serviceName);
        }
        [HttpPost]
        [Route("RestartService")]
        public ActionResult<string> RestartService(string serviceName)
        {
            return _serviceOperationBAL.RestartService(serviceName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading.Tasks;
using GFC.BAL;
using GFC.BAL.Interfaces;
using GFC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GFC.Api.Controllers.SystemInformation
{
    [Route("api/SystemInformation/[controller]")]
    [ApiController]
    public class SystemInfoController : ControllerBase
    {
        private readonly ISystemInfoBAL _systemInfoBO;
        private readonly IMemoryCache memoryCache;
        public SystemInfoController(ISystemInfoBAL systemInfoBO, IMemoryCache memoryCache)
        {
            _systemInfoBO = systemInfoBO;
            this.memoryCache = memoryCache;
        }

        // GET api/
        /// <summary>
        /// Get the ip address of the system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetIpAddress")]
        public ActionResult<string> GetIpAddress()
        {
            string hostName = _systemInfoBO.GetIpAddress();
            return hostName;
        }

        // GET api/
        /// <summary>
        /// Get the Host name of the system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetHostName")]
        public ActionResult<string> GetHostName()
        {
            String hostName = Dns.GetHostName();
            return "Computer name :" + hostName;
        }

        /// <summary>
        /// Method provide the information when the system last bootup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLastBootUpTime")]
        public ActionResult<string> GetLastBootUpTime()
        {
            DateTime currentTime;
            bool isExist = memoryCache.TryGetValue("CacheTime", out currentTime);
            if (!isExist)
            {
                currentTime = DateTime.Now;

                memoryCache.Set("CacheTime", currentTime);
            }
            return _systemInfoBO.GetLastBootUpTime() + " LastVisitedDateTime : " + currentTime;
        }

        /// <summary>
        /// Method provide the information about the service status
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetServiceInfo")]
        public ActionResult<List<ServiceInfo>> GetServiceInfo(string serviceName)
        {
            List<ServiceInfo> serviceInfo = new List<ServiceInfo>();
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName)
                {
                    ServiceInfo srvInfo = new ServiceInfo();
                    srvInfo.ServiceName = service.ServiceName;
                    srvInfo.ServiceDisplayName = service.DisplayName;
                    srvInfo.ServiceStatus = service.Status.ToString();
                    srvInfo.ServiceStartupType = service.StartType.ToString();
                    serviceInfo.Add(srvInfo);
                }
            }
            return serviceInfo;
        }

        /// <summary>
        /// Get Domain infor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDomain")]
        public ActionResult<string> GetDomain()
        {
            string Domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            return Domain;
        }
    }
}

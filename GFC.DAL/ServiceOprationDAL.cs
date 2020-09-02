using GFC.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace GFC.DAL
{
    public class ServiceOprationDAL : IServiceOprationDAL
    {
        public string RestartService(string serviceName)
        {
            string serviceStatus = string.Empty;
            serviceStatus = CommonServiceHelper.RestartService(serviceName);
            return serviceStatus;
        }

        public string StartService(string serviceName)
        {
            string serviceStatus = string.Empty;
            serviceStatus = CommonServiceHelper.StartService(serviceName);
            return serviceStatus;
        }

        public string StopService(string serviceName)
        {
            string serviceStatus = string.Empty;
            serviceStatus = CommonServiceHelper.StopService(serviceName);
            return serviceStatus;
        }
    }
}

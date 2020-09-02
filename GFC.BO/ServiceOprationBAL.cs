using GFC.BAL.Interfaces;
using GFC.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL
{
    /// <summary>
    /// Class is created to call data access layer & execute business logic for any Service Operation
    /// </summary>
    public class ServiceOprationBAL : IServiceOprationBAL
    {
        private readonly IServiceOprationDAL _serviceOprationDAL;

        public ServiceOprationBAL(IServiceOprationDAL serviceOprationDAL)
        {
            _serviceOprationDAL = serviceOprationDAL;
        }

        public string RestartService(string serviceName)
        {
            return _serviceOprationDAL.RestartService(serviceName);
        }

        public string StartService(string serviceName)
        {
            return _serviceOprationDAL.StartService(serviceName);
        }

        public string StopService(string serviceName)
        {
            return _serviceOprationDAL.StopService(serviceName);
        }
    }
}

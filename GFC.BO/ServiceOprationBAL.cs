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
    public class ServiceOperationBAL : IServiceOperationBAL
    {
        private readonly IServiceOperationDAL _serviceOperationDAL;

        public ServiceOperationBAL(IServiceOperationDAL serviceOperationDAL)
        {
            _serviceOperationDAL = serviceOperationDAL;
        }

        public string RestartService(string serviceName)
        {
            return _serviceOperationDAL.RestartService(serviceName);
        }

        public string StartService(string serviceName)
        {
            return _serviceOperationDAL.StartService(serviceName);
        }

        public string StopService(string serviceName)
        {
            return _serviceOperationDAL.StopService(serviceName);
        }
    }
}

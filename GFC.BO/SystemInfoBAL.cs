using GFC.BAL.Interfaces;
using GFC.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL
{
    /// <summary>
    /// Class is created to call data access layer & execute business logic for Any System level information
    /// </summary>
    public class SystemInfoBAL : ISystemInfoBAL
    {
        private readonly ISystemInfoDAL _systemInfoDAL;

        public SystemInfoBAL(ISystemInfoDAL systemInfoDAL)
        {
            _systemInfoDAL = systemInfoDAL;
        }
        public string GetIpAddress()
        {
           return _systemInfoDAL.GetIpAddress();
        }

        public string GetLastBootUpTime()
        {
            return _systemInfoDAL.GetLastBootUpTime();
        }

        public string SetRegistry()
        {
            return _systemInfoDAL.SetRegistry();
        }
    }
}

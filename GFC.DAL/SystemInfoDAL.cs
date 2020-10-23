using GFC.DAL.Interfaces;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GFC.DAL
{
    public class SystemInfoDAL : ISystemInfoDAL
    {
        private DateTime dtBootTime;

        /// <summary>
        /// Get the IP Address
        /// </summary>
        /// <returns></returns>
        public string GetIpAddress()
        {
            return CommonServiceHelper.GetIpAddress();
        }

        /// <summary>
        /// Get the last Boot up time of the system
        /// </summary>
        /// <returns></returns>
        public string GetLastBootUpTime()
        {
            string lastBoostUptime = string.Empty;
            // define a select query
            
            SelectQuery query =
                new SelectQuery("SELECT LastBootUpTime FROM Win32_OperatingSystem WHERE Primary = 'true'");

            // create a new management object searcher and pass it
            // the select query

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            // get the datetime value and set the local boot
            // time variable to contain that value

            foreach (ManagementObject mo in searcher.Get())
            {

                dtBootTime = ManagementDateTimeConverter.ToDateTime(mo.Properties["LastBootUpTime"].Value.ToString());
                TimeSpan ts = DateTime.Now - dtBootTime;

                // display the start time and date
                lastBoostUptime = "BoostUpDateTime :" + dtBootTime.ToLongDateString() + dtBootTime.ToLongTimeString() + "  last boot time :" + ts.ToString();
            }
            return lastBoostUptime;
        }
    }
}

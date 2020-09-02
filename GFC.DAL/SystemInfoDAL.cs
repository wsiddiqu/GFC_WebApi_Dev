using GFC.DAL.Interfaces;
using Microsoft.Win32;
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
            String hostName = Dns.GetHostName();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "Computer name :" + hostName;
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

        public string SetRegistry()
        {
            
                string returnResult = string.Empty;
                var test = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OurSettings");

                //storing the values  
                test.SetValue("Setting1", "This is our setting 1");
                test.SetValue("Setting2", "This is our setting 2");
                test.Close();

                //opening the subkey  
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OurSettings");
                RegistryKey key1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Talon\LicenseClient");

                if (key1 != null)
                {
                    returnResult = "CustomerID : " + key1.GetValue("CustomerID").ToString();
                    key1.Close();
                }
                
                return returnResult;
            
        }

        public string GetRegistry()
        {

            string returnResult = string.Empty;
            var test = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OurSettings");

            //storing the values  
            test.SetValue("Setting1", "This is our setting 1");
            test.SetValue("Setting2", "This is our setting 2");
            test.Close();

            //opening the subkey  
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OurSettings");
            RegistryKey key1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Talon\LicenseClient");

            if (key1 != null)
            {
                returnResult = "CustomerID : " + key1.GetValue("CustomerID").ToString();
                key1.Close();
            }
            
            return returnResult;

        }
    }
}

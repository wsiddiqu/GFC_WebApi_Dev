using GFC.DAL.Interfaces;
using GFC.Models;
using GFC.Utility.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using static GFC.Utility.Common.Helper;
using ServiceType = GFC.Utility.Common.Helper.ServiceType;

namespace GFC.DAL
{
    public class CommonServiceHelper
    {
        /// <summary>
        /// Method is responsible to stop Tum Service
        /// </summary>
        /// <returns></returns>
        public bool StopTumServer()
        {
            bool result = false;
            result = SendControlCode(Constants.TUM_SERVICE,SERVICE_CONTROL.SERVICE_CONTROL_TERMINATE_TUM_CLIENT);
            return result;
        }

        /// <summary>
        /// method used to change the service configuration 
        /// </summary>
        /// <returns></returns>
        public static bool ConfigureService(ConfigureServiceModel configureServiceModel)
        {
            bool bRetVal = false;
            IntPtr scmHandle = IntPtr.Zero;
            IntPtr scmLockHandle = IntPtr.Zero;
            IntPtr serviceHandle = IntPtr.Zero;
            SERVICE_FAILURE_ACTIONS serviceFailureActions;
            SC_ACTION[] scActions = new SC_ACTION[3];


            //Check to see if this really makes a difference.
            string serviceStartName = string.Empty;
            if(!string.IsNullOrEmpty(configureServiceModel.UserName))
            {
                serviceStartName = string.Format("{0}\\{1}", configureServiceModel.UserDomain, configureServiceModel.UserName);
            }
                
            bool bSuccess = false;

            bSuccess = GetServiceHandle(configureServiceModel.ServiceName, ref serviceHandle, ref scmHandle, ref scmLockHandle);
            try
            {
                if (false != bSuccess)
                {
                    /* The service exists, reconfigure it */
                    if (ChangeServiceConfig(serviceHandle,
                        configureServiceModel.ServiceType,
                        (int)configureServiceModel.ServiceStartType,
                        (int)ServiceErrorControl.SERVICE_ERROR_IGNORE,
                        configureServiceModel.ServicePath,
                        null,
                        0,
                        configureServiceModel.ServiceDependencies,
                        serviceStartName,
                        configureServiceModel.UserPassword,
                        configureServiceModel.ServiceDisplayName) == false)
                    {
                        throw new Exception(string.Format("{0}. _configureService.  Unable to change service {1} configuration, error is {2}", DateTime.Now, configureServiceModel.ServiceDisplayName, Marshal.GetLastWin32Error()));
                    }


                    if (!string.IsNullOrEmpty(configureServiceModel.ServiceDescription))
                    {
                        SERVICE_DESCRIPTION desc;
                        desc.lpDescription = configureServiceModel.ServiceDescription;

                        if (ChangeServiceConfig2(serviceHandle,InfoLevel.SERVICE_CONFIG_DESCRIPTION,ref desc) == false)
                        {
                            throw new Exception(string.Format("{0}. _configureService. Unable to change service {1} description, error is {2}", DateTime.Now, configureServiceModel.ServiceDisplayName, Marshal.GetLastWin32Error()));
                        }
                    }
                    if (true == configureServiceModel.ChangeFailureActions)
                    {
                        /* one year between counter resets */
                        serviceFailureActions.dwResetPeriod = 31536000;
                        serviceFailureActions.lpCommand = null;
                        serviceFailureActions.lpRebootMsg = "Service failure, rebooting !!!";
                        serviceFailureActions.cActions = scActions.Length;
                        /* restart the service in 1 sec upon all failures */
                        scActions[0].Delay = 1000;
                        scActions[0].SCActionType = SC_ACTION_TYPE.SC_ACTION_RESTART;
                        scActions[1].Delay = 1000;
                        scActions[1].SCActionType = SC_ACTION_TYPE.SC_ACTION_RESTART;
                        scActions[2].Delay = 1000;
                        scActions[2].SCActionType = SC_ACTION_TYPE.SC_ACTION_RESTART;

                        IntPtr scActionsPtr = new IntPtr();
                        scActionsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(new SC_ACTION()) * 3);
                        CopyMemory(scActionsPtr, scActions, Marshal.SizeOf(new SC_ACTION()) * 3);
                        serviceFailureActions.lpsaActions = scActionsPtr;
                        if (ChangeServiceConfig2(serviceHandle, InfoLevel.SERVICE_CONFIG_FAILURE_ACTIONS, ref serviceFailureActions) == false)
                        {
                            if (scActionsPtr != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(scActionsPtr);
                                scActionsPtr = IntPtr.Zero;
                            }
                            throw new Exception(string.Format("{0}. _configureService. Unable to change service {1} failure actions, error is {2}", DateTime.Now, configureServiceModel.ServiceDisplayName, Marshal.GetLastWin32Error()));
                        }
                        if (scActionsPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(scActionsPtr);
                            scActionsPtr = IntPtr.Zero;
                        }
                    }
                    bRetVal = true;
                }
            }
            catch(Exception ex)
            {
            //    m_log.WriteEntry(ex.Message);
            //TODO: Fix Error Handling
            }
            finally
            {
                if (IntPtr.Zero != serviceHandle)
                    CloseServiceHandle(serviceHandle);
                if (IntPtr.Zero != scmLockHandle)
                    UnlockServiceDatabase(scmLockHandle);
                if (IntPtr.Zero != scmHandle)
                    CloseServiceHandle(scmHandle);
            }
            return bRetVal;
        }

        /// <summary>
        /// Method is used to check id service exist & send control to the service by calling ControlService advapi32 dll
        /// </summary>
        /// <param name="sServiceName"></param>
        /// <param name="iType"></param>
        /// <returns></returns>
        private bool SendControlCode(string sServiceName, SERVICE_CONTROL iType)
        {
            bool bRetVal = false;
            IntPtr pServiceHandle = IntPtr.Zero;
            IntPtr pServiceCtrlMgr = IntPtr.Zero;
            IntPtr pLockServiceDB = IntPtr.Zero;

            SERVICE_STATUS ss = new SERVICE_STATUS();
            if (!DoesServiceExist(sServiceName))
            {
                throw new Exception(string.Format("{0}. SendControlCode. Service control {1} on service {2} bypassed with success code", DateTime.Now, iType, sServiceName));
            }
            try
            {
                if (false == GetServiceHandle(sServiceName, ref pServiceHandle, ref pServiceCtrlMgr, ref pLockServiceDB))
                {
                    bRetVal = false;
                    throw new Exception(string.Format("{0}. SendControlCode. Failed to open the service handle for {1}", DateTime.Now, sServiceName));
                }

                ServiceController pServiceController = new ServiceController();
                pServiceController.ServiceName = sServiceName;
                if (pServiceController.Status == ServiceControllerStatus.Running)
                {
                    if (!ControlService(pServiceHandle, iType, ref ss))
                    {
                        throw new Exception(string.Format("{0}. SendControlCode. ControlService call failed.  Error {1}", DateTime.Now, sServiceName));
                    }
                    else
                    {
                        bRetVal = true;
                    }
                }
            }
            catch
            {
                //TODO: Fix exception handling
            }
            finally
            {
                if (IntPtr.Zero != pServiceHandle)
                    CloseServiceHandle(pServiceHandle);
                if (IntPtr.Zero != pLockServiceDB)
                    UnlockServiceDatabase(pLockServiceDB);
                if (IntPtr.Zero != pServiceCtrlMgr)
                    CloseServiceHandle(pServiceCtrlMgr);
            }
            return bRetVal;
        }

        #region Private HelperMethods
        //This method is used to open, lock and return a pointer to the requested Service.  
        //it is up to the calling methods to close and unlock the input pointers.
        private static bool GetServiceHandle(string sServiceName, ref IntPtr pServiceHandle, ref IntPtr pServiceCtrlMgr, ref IntPtr pLockServiceDB)
        {
            bool bRetVal = false;
            int err;
            pServiceCtrlMgr = OpenSCManager(null, null, ServiceControlManagerType.SC_MANAGER_ALL_ACCESS);
            if (IntPtr.Zero == pServiceCtrlMgr)
            {
                throw new Exception(string.Format("{0}. GetServiceHandle. Unable to open services manager, error is {1}", DateTime.Now, Marshal.GetLastWin32Error()));
            }

            pLockServiceDB = LockServiceDatabase(pServiceCtrlMgr);
            if (IntPtr.Zero == pLockServiceDB)
            {
                throw new Exception(string.Format("{0}. GetServiceHandle. Unable to lock services manager database. error is {1}", DateTime.Now, Marshal.GetLastWin32Error()));
            }

            pServiceHandle = OpenService(pServiceCtrlMgr, sServiceName, SERVICE_ACCESS.SERVICE_ALL_ACCESS);
            if (IntPtr.Zero == pServiceHandle)
            {
                err = Marshal.GetLastWin32Error();
                if (Constants.ERROR_SERVICE_DOES_NOT_EXIST == err)
                {
                    throw new Exception(string.Format("{0}. GetServiceHandle. The service {1} does not exist", DateTime.Now, sServiceName));
                }
                else
                {
                    throw new Exception(string.Format("{0}. GetServiceHandle. Unable to open service {1}, error is {2}", DateTime.Now, sServiceName, Marshal.GetLastWin32Error()));
                }
            }
            bRetVal = true;
            return bRetVal;
        }
        #endregion

        #region ImportedDLLs

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, ServiceControlManagerType dwAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr LockServiceDatabase(IntPtr hSCManager);

        [DllImport("advapi32.dll", EntryPoint = "OpenServiceA", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, SERVICE_ACCESS dwDesiredAccess);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, SC_ACTION[] src, int count);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig(
                    IntPtr hService, ServiceType dwServiceType, int dwStartType,
                    int dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup,
                    int lpdwTagId, string lpDependencies, string lpServiceStartName,
                    string lpPassword, string lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(IntPtr hService,InfoLevel dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, InfoLevel dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnlockServiceDatabase(IntPtr hSCManager);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ControlService(IntPtr hService, SERVICE_CONTROL dwControl, ref SERVICE_STATUS lpServiceStatus);

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern uint NetJoinDomain(string lpServer,string lpDomain,string lpAccountOU,string lpAccount,string lpPassword,JoinOptions NameType);

        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int NetUnjoinDomain(string lpServer,string lpAccount,string lpPassword,UnJoinOptions fUnjoinOptions);

        #endregion

        /// <summary>
        /// finds if service exists in OS
        /// </summary>
        public static bool DoesServiceExist(string serviceName)
        {
            var obj = ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(serviceName));
            return obj;
        }

        /// <summary>
        /// Common method to Restart the service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string RestartService(string serviceName)
        {
            string serviceStatus = string.Empty;
            ServiceController service = new ServiceController(serviceName);

            if (CommonServiceHelper.DoesServiceExist(serviceName))
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    // Start the service, and wait until its status is "Running".
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    // Start the service, and wait until its status is "Running".
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    serviceStatus = serviceName + "has been Restarted Successfully";
                }
            }
            else
            {
                serviceStatus = serviceName + " Service not available";
            }
            return serviceStatus;
        }

        /// <summary>
        /// Common method to Start the service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string StartService(string serviceName)
        {
            string serviceStatus = string.Empty;
            if (CommonServiceHelper.DoesServiceExist(serviceName))
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    // Start the service, and wait until its status is "Running".
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    serviceStatus = serviceName + "has been started Successfully";
                }
            }
            else
            {
                serviceStatus = serviceName + "Service not available";
            }
            return serviceStatus;
        }

        /// <summary>
        /// Common method to Stop the service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string StopService(string serviceName)
        {

            string serviceStatus = string.Empty;
            if (CommonServiceHelper.DoesServiceExist(serviceName))
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status == ServiceControllerStatus.Running)
                {
                    // Start the service, and wait until its status is "Running".
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    serviceStatus = serviceName + "has been Stopped Successfully";
                }
            }
            else
            {
                serviceStatus = serviceName + "Service not available";
            }
            return serviceStatus;
        }

        public static string GetIpAddress()
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
        /// Get Default Install Dir
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultInstallDir()
        {
            string installPath = Constants.INSTALL_PATH;

            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                var subRegKey = hklm.OpenSubKey("Software/Talon");
                if (subRegKey != null)
                {
                    string regValue = (string)subRegKey.GetValue("InstallationDir");
                    if (!string.IsNullOrWhiteSpace(regValue))
                    {
                        installPath = regValue;
                    }
                }
            }

            return installPath;
        }
    }
}

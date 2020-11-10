using GFC.DAL.Interfaces;
using GFC.Models;
using GFC.Utility.Common;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static GFC.Utility.Common.Helper;

namespace GFC.DAL
{
    public class RegistryInfoDAL : IRegistryInfoDAL
    {
        Dictionary<string, string> values = new Dictionary<string, string>();

        /// <summary>
        /// Method return if entry exist for the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public long GetAutoDFS(string path, string name)
        {
            long AutoDFSCount = 0;
            AutoDFSCount = GetRegistryValue(path, name) != null && (int)GetRegistryValue(path, name) > 0 ? 1 : 0;
            return AutoDFSCount;
        }

        /// <summary>
        /// Method used to provide all the registry entry present for the given root
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetRegistryInfo(RegistryKey root)
        {
            string ressult = string.Empty;
            foreach (var child in root.GetSubKeyNames())
            {
                using (var childKey = root.OpenSubKey(child))
                {
                    GetRegistryInfo(childKey);
                }
            }
            foreach (var value in root.GetValueNames())
            {
                values.Add(string.Format("{0}\\{1}", root, value), (root.GetValue(value) ?? "").ToString());
            }
            return values;
        }

        /// <summary>
        /// Method return the value of registry by name
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private object GetRegistryValue(string path, string name)
        {
            object result = null;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path);

            if (registryKey != null)
            {
                result = registryKey.GetValue(name);
                registryKey.Close();
            }
            return result; ;
        }

        /// <summary>
        /// Method provide list of the backed server
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetBackendServerList(string path)
        {
            return GetRegistryInfoList(path);
        }

        /// <summary>
        /// Method provide list of registry entires for the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetRegistryInfoList(string path)
        {
            string[] list = null;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path);
            if (registryKey != null)
            {
                list = registryKey.GetSubKeyNames();
                registryKey.Close();
            }
            return list;
        }

        /// <summary>
        /// Method is used for core configuration to add backend server
        /// </summary>
        /// <param name="backendServerModel"></param>
        public void AddBackendServer(BackendServerModel backendServerModel)
        {
            string subRegKey = Constants.REGKEY_SA_NASDB + "\\" + backendServerModel.BackendServerName;
            CreateKey(Constants.REGKEY_SA_SERVER_SYSID);
            CreateKey(subRegKey);
            Log.Information((string.Format("New CIFS Server {0} is added.", backendServerModel.BackendServerName)));
            if ((backendServerModel.UserName != string.Empty) && (backendServerModel.Password != string.Empty))
            {
                SetValue(subRegKey, Constants.REGSTR_VAL_USERNAME, backendServerModel.UserName);
                //SetValue(subRegKey, Constants.REGSTR_VAL_PASSWORD, backendServerModel.Password);
                var result = Encoding.Unicode.GetBytes(backendServerModel.Password);
                var EncryptedPassword = ProtectedData.Protect(result, null, DataProtectionScope.LocalMachine);
                //SetValue(subRegKey, Constants.REGSTR_VAL_PASSWORD, EncryptedPassword);
                SetBinary(subRegKey, Constants.REGSTR_VAL_PASSWORD, EncryptedPassword);
            }

            string sServer = (string)GetRegistryValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_SYSID);
            SetCustomerID(Dns.GetHostName());
            if (backendServerModel.LocalPath != string.Empty)
            {
                SetValue(subRegKey, Constants.REGSTR_VAL_LOCALPATH, backendServerModel.LocalPath);
            }

            //Restart the LMC so that the new configurations take immediate effect.

            CommonServiceHelper.RestartService(Constants.LMCLIENT_SERVICE);

            ConfigureServiceModel configureServiceModel = new ConfigureServiceModel();
            configureServiceModel.ServiceName = Constants.TUM_SERVICE;
            configureServiceModel.ServiceType = ServiceType.SERVICETYPE_NO_CHANGE;
            configureServiceModel.ServiceStartType = ServiceStartType.SERVICE_AUTO_START;

            if (string.IsNullOrEmpty(CommonServiceHelper.RestartService(Constants.TUM_SERVICE)))
            {
                throw new Exception("Unable to Start Talon User Module Service");
            }
        }

        /// <summary>
        /// Method is used for Core configuratino to delete any exsiting backend server
        /// </summary>
        /// <param name="server"></param>
        public void DeleteBackendServer(string server)
        {
            CommonServiceHelper commonServiceHelper = new CommonServiceHelper();
            StringBuilder sbRegKey = new StringBuilder();
            sbRegKey.Append(Constants.REGKEY_SA_NASDB);
            sbRegKey.Append("\\");
            sbRegKey.Append(server);
            DeleteKey(sbRegKey.ToString());
            bool removeCoreConfiguration = CheckIfConfiguredForCore();
            if (removeCoreConfiguration)
            {
                SetValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_ISCLIENT, "");
                SetValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_ISSERVER, "");
            }
            Log.Information((string.Format("Deleted Server {0} successfully.", server)));
            commonServiceHelper.StopTumServer();
        }

        /// <summary>
        /// Method is used to delete any exsiting key
        /// </summary>
        /// <param name="path"></param>
        private void DeleteKey(string path)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path);
            if (registryKey != null)
            {
                Registry.LocalMachine.DeleteSubKeyTree(path);
            }
        }

        /// <summary>
        /// Get count of cores under serverdb
        /// </summary>
        /// <returns></returns>
        public bool CheckIfConfiguredForCore()
        {
            bool result = false;
            try
            {
                //Open registry location
                RegistryKey reg = Registry.LocalMachine;
                //RegistryKey subKey = reg.OpenSubKey(@"Talon\tum\Client\Serverdb");
                RegistryKey subKey = reg.OpenSubKey(Constants.REGKEY_SA_NASDB);
                //Get count of cores under serverdb
                string countOfDBString = subKey.SubKeyCount.ToString();
                int countOfDB = Int32.Parse(countOfDBString);
                //If count is null return true else false
                if (countOfDB == 1)
                    result = true;
            }
            catch (Exception e)
            {
                //do nothing
            }
            return result;
        }

        /// <summary>
        /// Create new entry to registry 
        /// </summary>
        /// <param name="path"></param>
        public static void CreateKey(string path)
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(path);
            if (registryKey == null)
            {
                throw new Exception(string.Format("Unable to create registry path {0}.", path));
            }
            registryKey.Close();
        }

        /// <summary>
        /// set registry value
        /// </summary>
        /// <param name="path"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        public static void SetValue(string path, string valueName, object value)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path, true);

            if (registryKey == null)
            {
                CreateKey(path);
                registryKey = Registry.LocalMachine.OpenSubKey(path, true);
            }

            registryKey.SetValue(valueName, value);
            registryKey.Close();
        }

        /// <summary>
        /// Set the Customer entries in registry
        /// </summary>
        /// <param name="CustomerID"></param>
        private void SetCustomerID(string CustomerID)
        {
            CommonServiceHelper commonServiceHelper = new CommonServiceHelper();

            Log.Debug("[Set Customer] The Backend Server Customer Info  for Customer Id [{0}].", CustomerID);

            SetValue(Constants.REGKEY_SA_TUM_SERVER, Constants.REGKEY_SA_CUSTOMERID, CustomerID);
            SetValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_SYSID, CustomerID);
            SetValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_ISCLIENT, "No");
            SetValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_ISSERVER, "Yes");
            SetValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_SERVER_PORT, Constants.Port);
            SetValue(Constants.REGKEY_SA_SERVER_SYSID, Constants.REGSTR_VAL_SERVER_SSLPORT, Constants.SSLPort);

            commonServiceHelper.StopTumServer();
        }

        public static void SetBinary(string path, string valueName, Byte[] value)
        {
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(path, true);

            if (rk == null)
            {
                //throw new TalonException(string.Format("Registry Path {0} does not exist.", path));
                CreateKey(path);
                rk = Registry.LocalMachine.OpenSubKey(path, true);
            }

            rk.SetValue(valueName, value, RegistryValueKind.Binary);
            rk.Close();
        }

        /// <summary>
        /// Methos is used to call to join the machine to the domain
        /// </summary>
        /// <param name="joinDomainModel"></param>
        /// <returns>int</returns>
        public int JoinDomain(JoinDomainModel joinDomainModel)
        {
            Log.Information("[Join Domain] required info has been pass to join the Domain.");
            uint result = CommonServiceHelper.NetJoinDomain(joinDomainModel.Server, joinDomainModel.Domain, joinDomainModel.OU, joinDomainModel.Account, joinDomainModel.Password, (JoinOptions.NETSETUP_JOIN_DOMAIN | JoinOptions.NETSETUP_DOMAIN_JOIN_IF_JOINED | JoinOptions.NETSETUP_ACCT_CREATE));
            if (result == 0 && (!string.IsNullOrEmpty(joinDomainModel.RestartNow) && joinDomainModel.RestartNow.ToLower() == "yes"))
            {
                Log.Debug("[Join Domain] Restart the system if join the domain");
                SystemRestart(joinDomainModel.Server);
            }
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Methos is used to unjoin the machine to the domain
        /// </summary>
        /// <param name="joinDomainModel"></param>
        /// <returns></returns>
        public int UnJoinDomain(JoinDomainModel joinDomainModel)
        {

            Log.Information("[UnJoin Domain] required info has been pass to unjoin the Domain.");
            int result = CommonServiceHelper.NetUnjoinDomain(joinDomainModel.Server, joinDomainModel.Account, joinDomainModel.Password, (UnJoinOptions.NETSETUP_ACCT_DELETE | UnJoinOptions.NONE));
            if (result == 0 && (!string.IsNullOrEmpty(joinDomainModel.RestartNow) && joinDomainModel.RestartNow.ToLower() == "yes"))
            {
                Log.Debug("[UnJoin Domain] Restart the system if unjoin the domain");
                SystemRestart(joinDomainModel.Server);
            }
            return Convert.ToInt32(result);

        }

        /// <summary>
        /// Method is responsible to restart the machine 
        /// </summary>
        /// <param name="systemAddress"></param>
        public void SystemRestart(string systemAddress)
        {
            Log.Information("[System Restart] Restarting the system {0}", systemAddress);
            Process commandProcess = new Process();
            try
            {
                commandProcess.StartInfo.FileName = "cmd.exe";
                commandProcess.StartInfo.UseShellExecute = false;
                commandProcess.StartInfo.CreateNoWindow = true;
                commandProcess.StartInfo.RedirectStandardError = true;
                commandProcess.StartInfo.RedirectStandardInput = true;
                commandProcess.StartInfo.RedirectStandardOutput = true;
                commandProcess.Start();
                commandProcess.StandardInput.WriteLine("shutdown /r /m " + systemAddress + " /t 1 /f");
                commandProcess.StandardInput.WriteLine("exit");
                for (; !commandProcess.HasExited;)//wait executed  
                {
                    System.Threading.Thread.Sleep(1);
                }
                //error output  
                string tmpout = commandProcess.StandardError.ReadToEnd();
                string tmpout1 = commandProcess.StandardOutput.ReadToEnd();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            finally
            {
                if (commandProcess != null)
                {
                    commandProcess.Dispose();
                    commandProcess = null;
                }
            }
        }
    }
}

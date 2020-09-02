using GFC.BAL.Interfaces;
using GFC.DAL.Interfaces;
using GFC.Models;
using GFC.Utility.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL
{
    /// <summary>
    /// Class is created to call data access layer & execute business logic for Registry Info & configuration
    /// </summary>
    public class RegistryInfoBAL : IRegistryInfoBAL
    {
        private readonly IRegistryInfoDAL _registryInfoDAL;

        public RegistryInfoBAL(IRegistryInfoDAL registryInfoDAL)
        {
            _registryInfoDAL = registryInfoDAL;
        }
        public Dictionary<string, string> GetRegistryInfo(RegistryKey root)
        {
           return _registryInfoDAL.GetRegistryInfo(root);
        }

        public long GetAutoDFS()
        {
            return _registryInfoDAL.GetAutoDFS(Constants.REGKEY_SA_Talon,Constants.REGKEY_VAL_AUTO_DFS);
        }

        public string[] GetBackendServerList()
        {
            return _registryInfoDAL.GetBackendServerList(Constants.REGKEY_SA_NASDB);
        }

        public void AddBackendServer(BackendServerModel backendServerModel)
        {
            _registryInfoDAL.AddBackendServer(backendServerModel);
        }

        public void DeleteBackendServer(string server)
        {
            _registryInfoDAL.DeleteBackendServer(server);
        }

        public int JoinDomain(JoinDomainModel joinDomainModel)
        {
            return _registryInfoDAL.JoinDomain(joinDomainModel);
        }

        public int UnJoinDomain(JoinDomainModel joinDomainModel)
        {
            return _registryInfoDAL.UnJoinDomain(joinDomainModel);
        }

        public void SystemRestart(string systemAddress)
        {
            _registryInfoDAL.SystemRestart(systemAddress);
        }
    }
}

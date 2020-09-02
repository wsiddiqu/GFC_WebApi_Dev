using GFC.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL.Interfaces
{
    public interface IRegistryInfoBAL
    {
        Dictionary<string, string> GetRegistryInfo(RegistryKey root);
        long GetAutoDFS();
        string[] GetBackendServerList();
        void AddBackendServer(BackendServerModel backendServerModel);

        void DeleteBackendServer(string server);
        int JoinDomain(JoinDomainModel joinDomainModel);
        int UnJoinDomain(JoinDomainModel joinDomainModel);
        void SystemRestart(string systemAddress);
    }
}

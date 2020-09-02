using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GFC.BAL.Interfaces;
using GFC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace GFC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistryInfoController : Controller
    {
        private readonly IRegistryInfoBAL _registryInfoBAL;

        public RegistryInfoController(IRegistryInfoBAL registryInfoBAL)
        {
            _registryInfoBAL = registryInfoBAL;
        }

        [HttpGet]
        public ActionResult<Dictionary<string, string>> SetRegistryInfo()
        {
            return _registryInfoBAL.GetRegistryInfo(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Talon"));
        }

        /// <summary>
        /// Get all the registry info under talon
        /// </summary>
        /// <returns>Dictionary object</returns>
        [HttpGet]
        [Route("GetTalonRegistryInfo")] 
        public ActionResult<Dictionary<string, string>> GetTalonRegistryInfo()
        {
            return _registryInfoBAL.GetRegistryInfo(Registry.LocalMachine.OpenSubKey(@"Talon"));
        }

        /// <summary>
        /// Get the info count if the key present in mentioned path
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAutoDFS")]
        public ActionResult<long> GetAutoDFS()
        {
            return _registryInfoBAL.GetAutoDFS();
        }

        /// <summary>
        /// Method is use to Get backend server List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBackendServerList")]
        public ActionResult<string[]> GetBackendServerList()
        {
            return _registryInfoBAL.GetBackendServerList();
        }

        /// <summary>
        /// Method is use to Add backend server create new entry to the server list
        /// </summary>
        /// <param name="backendServerModel"></param>
        [HttpPost]
        [Route("AddBackendServer")]
        public void AddBackendServer(BackendServerModel backendServerModel)
        {
            _registryInfoBAL.AddBackendServer(backendServerModel);
        }

        /// <summary>
        /// Method take Server name and responsible to delete the server
        /// </summary>
        /// <param name="server"></param>
        [HttpPost]
        [Route("DeleteBackendServer")]
        public void DeleteBackendServer(string server)
        {
            _registryInfoBAL.DeleteBackendServer(server);
        }

        /// <summary>
        /// Method is used to join the machine to the domain
        /// </summary>
        /// <param name="joinDomainModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("JoinDomain")]
        public ActionResult<int> JoinDomain(JoinDomainModel joinDomainModel)
        {
            var result = _registryInfoBAL.JoinDomain(joinDomainModel);
            return result;
        }

        /// <summary>
        /// Method is used unjoin the machine domain
        /// </summary>
        /// <param name="joinDomainModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UnJoinDomain")]
        public ActionResult<int> UnJoinDomain(JoinDomainModel joinDomainModel)
        {
            var result = _registryInfoBAL.UnJoinDomain(joinDomainModel);
            return result;
        }

        /// <summary>
        /// Method is used to reset the system 
        /// </summary>
        /// <param name="systemAddress"></param>
        [HttpPost]
        [Route("SystemRestart")]
        public void SystemRestart(string systemAddress)
        {
           _registryInfoBAL.SystemRestart(systemAddress);
        }

    }
}

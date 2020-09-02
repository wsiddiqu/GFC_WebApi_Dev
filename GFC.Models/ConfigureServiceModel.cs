using System;
using System.Collections.Generic;
using System.Text;
using static GFC.Utility.Common.Helper;

namespace GFC.Models
{
    public class ConfigureServiceModel  
    {
        public string ServiceName{ get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserDomain { get; set; }
        public string ServicePath { get; set; }
        public string ServiceDisplayName { get; set; }
        public ServiceType ServiceType { get; set; }
        public ServiceStartType ServiceStartType { get; set; }
        public string ServiceDependencies { get; set; }
        public string ServiceDescription { get; set; }
        public bool ChangeFailureActions { get; set; }
    }
}

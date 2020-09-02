using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL.Interfaces
{
    public interface IServiceOprationBAL
    {
        string StopService(string serviceName);

        string StartService(string serviceName);

        string RestartService(string serviceName);
    }
}

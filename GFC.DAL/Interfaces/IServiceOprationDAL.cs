﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.DAL.Interfaces
{
    public interface IServiceOperationDAL
    {
        string StopService(string serviceName);

        string StartService(string serviceName);

        string RestartService(string serviceName);

        //bool DoesServiceExist(string serviceName);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.DAL.Interfaces
{
    public interface ISystemInfoDAL
    {
        string GetIpAddress();

        string GetLastBootUpTime();
    }
}

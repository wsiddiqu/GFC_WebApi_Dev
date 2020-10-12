using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL.Interfaces
{
    public interface ISystemInfoBAL
    {
        string GetIpAddress();
        string GetLastBootUpTime();
    }
}

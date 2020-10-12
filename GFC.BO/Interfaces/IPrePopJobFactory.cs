using GFC.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.BAL.Interfaces
{
    public interface IPrePopJobFactory
    {
        public IPrePopJobsDAL CreatePrePopJobOperation(string type);
    }
}

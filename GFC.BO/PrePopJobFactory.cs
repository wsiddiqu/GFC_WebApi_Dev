using GFC.BAL.Interfaces;
using GFC.DAL;
using GFC.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static GFC.Utility.Common.Helper;

namespace GFC.BAL
{
    public class PrePopJobFactory : IPrePopJobFactory
    {
        public IPrePopJobsDAL CreatePrePopJobOperation(string type)
        {
            PrePopJobType format;
            IPrePopJobsDAL prePopJobOperation = null;
            if (Enum.TryParse(type, out format))
            {
                switch (format)
                {
                    case PrePopJobType.XML:
                        prePopJobOperation = new PrePopJobsDAL();
                        break;
                }
            }

            return prePopJobOperation;
        }
    }
}

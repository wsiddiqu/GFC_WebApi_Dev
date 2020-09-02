using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GFC.Api
{
    public class AppException : Exception
    {
        public AppException()
        { }

        public AppException(string message)
            : base(message)
        { }

        public AppException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

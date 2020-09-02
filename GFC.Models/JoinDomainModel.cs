using System;
using System.Collections.Generic;
using System.Text;

namespace GFC.Models
{
    public class JoinDomainModel
    {
        public string Server{ get; set; }
        public string Domain { get; set; }
        public string OU { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }
}

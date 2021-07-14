using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class DabaseConnectionInfo
    {
        public string Server { get; set; }
        public uint Port { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Database { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MCHS_Resender
{
    public class CFG
    {
        public string ConnectionString = ConfigurationManager.AppSettings["ConnString"];
        public string Path = ConfigurationManager.AppSettings["Path"];
    }
}

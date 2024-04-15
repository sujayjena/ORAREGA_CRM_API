using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OraRegaAV.Models.Constants
{
    public static class OrderTypeCodes
    {
        public static string Both
        { 
            get { return "C"; }
        }

        public static string SalesOrder
        {
            get { return "S"; }
        }

        public static string WorkOrder
        {
            get { return "W"; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupplierReg.Web.BLL
{
    public class Formatter
    {
        public static string Date(DateTime? d)
        {
            if (d == null) return "";
            return ((DateTime)d).ToString("yyyy-MM-dd");
        }
    }
}
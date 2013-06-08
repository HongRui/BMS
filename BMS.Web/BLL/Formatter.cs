using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMS.Web.BLL
{
    public class Formatter
    {
        public static string Date(DateTime? d)
        {
            if (d == null) return "";
            return ((DateTime)d).ToString("yyyy-MM-dd");
        }
        public static Newtonsoft.Json.Converters.IsoDateTimeConverter TimeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter{DateTimeFormat = "yyyy-MM-dd HH:mm:ss"};
        public static Newtonsoft.Json.Converters.IsoDateTimeConverter DateConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };
       
    }

    
}
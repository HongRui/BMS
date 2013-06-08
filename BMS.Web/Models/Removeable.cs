using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMS.Web.Models
{
    public class Removeable<T>
    {
        public T obj;
        public bool IsRemoved { get; set; }
    }
}
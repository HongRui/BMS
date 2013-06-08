using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMS.Web.Models
{
    public class ViewMaterialOutListReport
    {
        public int Id { get; set; }
        public DateTime OutDate { get; set; }
        public string Department { get; set; }
        public int DepartmentId { get; set; }
        public string UserName { get; set; }
        public int CatalogId { get; set; }
        public string CatalogName { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public decimal OutNum { get; set; }
        public string Remark { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class ViewMaterialOutReport
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int MaterialId;
        public int CatalogId { get; set; }
        public string CatalogName { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public decimal OutNum { get; set; }
        public decimal TotalPrice { get; set; }
        
    }
    
    //public class ViewMaterialOutDetailReport
    //{
    //    public int Id;
    //    public string UserName { get; set; }
    //    public string CatalogName { get; set; }
    //    public string Name { get; set; }
    //    public string Spec { get; set; }
    //    public decimal OutNum { get; set; }
    //}

}
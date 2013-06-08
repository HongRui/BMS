using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMS.Web.Models
{
    public class ViewMaterialIn
    {
        public static string sql = @"select i.Id,i.Code ,convert(nvarchar,i.InType) InType,i.Supplier,i.InDate,i.Remark
,m.name,m.spec,item.unitprice,item.num,item.unitprice*item.num as price,d.Name as Department ,u.Name as UserName
from materialins i
join materialinitems item on i.id=item.inid
join materials m on item.materialid=m.id
join departments d on m.departmentid=d.id
join systemusers u on i.personid=u.id";
        public int id { get; set; }
        public string Department { get; set; }
        public string UserName { get; set; }
        public string Code { get; set; }
        public string InType { get; set; }
        public string Supplier { get; set; }
        public DateTime InDate { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Num { get; set; }
        public decimal Price { get; set; }
    }
   

}
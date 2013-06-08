using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class Building
    {
        public static string LogClass = "项目";
        public int Id { get; set; }
        [DisplayName("公司")]
        public int CompanyId { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("名称")]
        public string Name { get; set; }
        [DisplayName("地址")]
        [MaxLength(50)]
        public string Address { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }
        public static string GetName(int id)
        {
            using (OUContext db = new OUContext())
            {
                var c = db.Buildings.Find(id);
                if (c != null) return c.Name;
            }
            return "";
        }
    }
    public class BuildingView
    {
        public static string sql = "select b.*,c.name as company from buildings b join companies c on b.companyid=c.id";
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
        public string Company { get; set; }
    }   
}
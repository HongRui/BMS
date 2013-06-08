using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SupplierReg.Web.Models;
using OUDAL;
namespace SupplierReg.Web.BLL
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        
        public static UserInfo CurUser
        {
            get
            {
                if(HttpContext.Current.User.Identity.IsAuthenticated!=false)
                {
                     int id = 0;
                    int.TryParse(HttpContext.Current.User.Identity.Name, out id);
                    OUContext db=new OUContext();
                    Supplier s = (from o in db.Suppliers where o.Id == id select o).FirstOrDefault();
                    if(s!=null)
                    {
                        return new UserInfo{Id=s.Id,Name = s.Name,FullName = s.FullName};
                    }
                }
                return new UserInfo {Id = 0, Name = "",FullName = ""};
            }
            
        }
    }
}
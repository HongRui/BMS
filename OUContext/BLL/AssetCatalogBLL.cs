using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace OUDAL
{
    
    public class AssetCatalogBLL
    {
        static OUContext db = new OUContext();
        static public List<AssetCatalog> AssetCatalogs;
        static AssetCatalogBLL()
        {
            Update();   
        }
        public static void Update()
        {
            AssetCatalogs = (from o in db.AssetCatalogs.AsNoTracking() select o).ToList();
        }
        public static AssetCatalog GetById(int id)
        {
            return AssetCatalogs.Find(o => o.Id == id);
        }
        public static string GetNameById(int id)
        {
            AssetCatalog d = GetById(id);
            if (d == null) return "";
            return d.Name;
        }
        static string _GetFullNameById(int id, string name)
        {
            AssetCatalog c = GetById(id);
            if (c == null) return "";
            if (c.PId == 0)
            {
            }
            else
            {
                if(name=="")
                {
                    name = c.Name;
                }else{name=c.Name+"-"+ name ;}
                AssetCatalog p = (from o in AssetCatalogs where o.Id == c.PId select o).FirstOrDefault();
                if (p != null) return _GetFullNameById(p.Id, name);
            }
            return name;
        }
        public static string GetFullNameById(int id)
        {
            return _GetFullNameById(id, "");
        }
        static void _GetIdString(List<int> list, int id)
        {
            AssetCatalogs.ForEach(o =>
            {
                if (o.PId == id)
                {
                    list.Add(o.Id);
                    _GetIdString(list, o.Id);
                }
            });
        }
        /// <summary>
        /// 获取类型，用,分开
        /// </summary>
        /// <param name="i">parent id</param>
        /// <returns></returns>
        static public string GetIdString(int i)
        {
            List<int> list = new List<int>();
            _GetIdString(list, i);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(i);
            list.ForEach(o => { sb.Append(","); sb.Append(o); });
            return sb.ToString();
        }
        static public bool IsParnet(int idp, int idc)
        {
            AssetCatalog d = GetById(idc);
            if (d.PId == idp) return true;
            if (d.PId == 0) return false;
            return IsParnet(idp, d.PId);
        }
    }
}
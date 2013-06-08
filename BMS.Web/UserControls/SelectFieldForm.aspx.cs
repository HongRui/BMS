using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mall;
namespace BMS.Web.UserControls
{
    public partial class SelectFieldForm : System.Web.UI.Page
    {
        protected int colNum = 3;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string queryname = Request["type"] + "";
                Query query = QueryBuilder.GetQuery(queryname);

                if (query != null)
                {
                    string filelist = query.GetFieldList(Context);
                    string[] filenames = filelist.Split(',');
                    foreach (Field f in query.FieldList)
                    {
                        if (!f.IsHidden)
                        {

                            ListItem item = new ListItem(f.Name, f.FullName);
                            CheckBoxList1.Items.Add(item);
                            if (f.IsForce)
                            {
                                item.Enabled = false;
                                item.Selected = true;
                            }
                            else
                            {
                                foreach (string s in filenames)
                                {
                                    if (s == f.FullName)
                                    {
                                        item.Selected = true; break;
                                    }

                                }
                            }
                        }
                    }

                }
            }

        }
        protected void btnSave_ServerClick(object sender, EventArgs e)
        {
            bool i = Context.Request.Browser.Cookies;
            bool isSelectOne = false;

            string list = "";
            string queryname = Request["type"] + "";
            Query query = QueryBuilder.GetQuery(queryname);

            if (query != null)
            {
                foreach (Field f in query.FieldList)
                {
                    if (f.IsHidden == true)
                    {
                        if (list.Length > 0)
                        {
                            list += ",";
                        }
                        list += f.FullName;
                    }
                }
                foreach (ListItem item in CheckBoxList1.Items)
                {
                    if (item.Selected == true)
                    {
                        if (list.Length > 0)
                        {
                            list += ",";
                        }
                        list += item.Value;
                        isSelectOne = true;
                    }
                }
                if (!isSelectOne)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "select", "<script>alert('请选择至少一个栏目');</script>");
                    return;
                }
                //if (list.Length == 0)
                //{
                //    ClientScript.RegisterStartupScript(this.GetType(), "select", "<script>alert('请选择至少一个栏目');</script>");
                //    return;
                //}
                query.SetFieldList(Context, list);
            }

            ClientScript.RegisterStartupScript(this.GetType(), "select", "<script>window.opener.location.reload();window.close();</script>");
        }
    }
}
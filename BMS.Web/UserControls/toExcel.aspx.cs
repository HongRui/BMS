using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
namespace Mall
{
    public partial class toExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void btn_Click(object sender, EventArgs e)
        {
            Response.Clear();

            Response.Buffer = false;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            //Response.Charset = "GB2312";
            //Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}{1}.{2}", Request["name"], DateTime.Today.ToShortDateString(), "xls"));
            //Response.HeaderEncoding = System.Text.Encoding.GetEncoding("utf-8");
            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            Response.ContentType = "application/ms-excel";
            //Response.ContentType = "text/plain";
            this.EnableViewState = false;
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            this.RenderControl(oHtmlTextWriter);

            //强制输出bom 这样避免excel打开时乱码
            Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            Response.Write(this.tbl.Value);
            Response.End();
        }
    }
}

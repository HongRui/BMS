using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;
namespace SupplierReg.Web
{
    public static  class MyExtensions
    {
       public static MvcHtmlString AttachmentUrl(this HtmlHelper helper,OUDAL.Attachment attachment)
       {
           string s = "<a href='{0}/Attachment/Download?fileid={1}'>{2}</a>";
           return
               MvcHtmlString.Create(string.Format(HttpRuntime.AppDomainAppPath, attachment.FileName, attachment.Id));

       }
        public static MvcHtmlString MyValidation(this HtmlHelper helper, string name)
        {
            return MvcHtmlString.Create(string.Format("<span class=\"field-validation-valid\" data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", name));
        }
        
        public static MvcHtmlString MyDisplayFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string text)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1}</td>", metadata.DisplayName, helper.Encode(text)));

        }
        public static MvcHtmlString MyDisplayFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string text = "";
            if (metadata.Model == null)
            {
                
            }
            else if (metadata.Model.GetType() == typeof(DateTime))
            {
                text = BLL.Formatter.Date((DateTime)metadata.Model);
            }
            else
            {
                text = metadata.SimpleDisplayText;
                //if (text.Contains("\n")) text = text.Replace("\n", "<br/>");
            }
            return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1}</td>", metadata.DisplayName, helper.Encode(text).Replace("\n","<br/>")));

        }
        public static MvcHtmlString MyTextFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);                       
            
            MvcHtmlString m2 = EditorExtensions.EditorFor<TModel, TValue>(helper, expression);
            MvcHtmlString m3 = ValidationExtensions.ValidationMessageFor<TModel, TValue>(helper, expression);
            string isrequest = "";
            if (m3 != null && metadata.IsRequired == true) isrequest = "<span style='color:red'>*</span>";
            return MvcHtmlString.Create(string.Format("{0} {1} {2}",  m2.ToHtmlString(), m3 == null ? "" : m3.ToHtmlString(), isrequest));
            
        }
        public static MvcHtmlString MyInt(this HtmlHelper helper,string name, int? Value)
        {
            string s = "<input id='{0}' name='{0}' value='{1}' onkeyup=\"this.value=this.value.replace(/[^0-9]/g,'')\" />";

            return MvcHtmlString.Create(string.Format(s,name,Value==null?"":((int)Value).ToString() ));
             
        }
        public static MvcHtmlString MyDecimal(this HtmlHelper helper, string name, decimal? Value)
        {
            string s = "<input class=\"text-box single-line\" data-val=\"true\" data-val-number=\"请输入数值\" data-val-required=\"请输入数值。\" id=\"{0}\" name=\"{0}\"  type=\"text\" value=\"{1}\" onkeyup=\"this.value=this.value.replace(/[^0-9.]/g,'')\"/>";
            return MvcHtmlString.Create(string.Format(s, name, Value == null ? "" : ((decimal)Value).ToString()));

        }
        //onkeyup="this.value=this.value.replace(/[^0-9]/g,'')"
        public static MvcHtmlString MyTextForTR<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool isReadonly,string remark)
        {
            
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);   
            string isreadonly="";
            if (isReadonly)
            {
                isreadonly = "readonly='readonly'";
                //return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1}</td>", metadata.DisplayName, helper.Encode(metadata.SimpleDisplayText)));
            }
            MvcHtmlString m1 = LabelExtensions.LabelFor<TModel, TValue>(helper, expression);            
            MvcHtmlString m2 = EditorExtensions.EditorFor<TModel, TValue>(helper, expression,isreadonly);
            MvcHtmlString m3 = ValidationExtensions.ValidationMessageFor<TModel, TValue>(helper, expression);
            string isrequest = "";
            if (m3!=null && metadata.IsRequired == true ) isrequest = "<span style='color:red'>*</span>";
            return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1} {3} {2} {4}</td>", m1.ToHtmlString(), m2.ToHtmlString(),m3==null?"":m3.ToHtmlString(),isrequest,remark));
            
        }
        public static MvcHtmlString MyTextForTR<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool isReadonly)
        {
            return MyTextForTR(helper, expression, isReadonly, "");
        }
        public static MvcHtmlString MyTextForTR<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression,string remark)
        {
            return MyTextForTR(helper, expression, false,remark);
        }
        public static MvcHtmlString MyTextForTR<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            return MyTextForTR(helper, expression, false);
        }
        public static MvcHtmlString MyTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool isReadonly, string remark)
        {

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string isreadonly = "";
            if (isReadonly)
            {
                isreadonly = "readonly='readonly'";
                //return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1}</td>", metadata.DisplayName, helper.Encode(metadata.SimpleDisplayText)));
            }
            MvcHtmlString m1 = LabelExtensions.LabelFor<TModel, TValue>(helper, expression);
            MvcHtmlString m2 = TextAreaExtensions.TextAreaFor<TModel, TValue>(helper, expression, isreadonly);
            MvcHtmlString m3 = ValidationExtensions.ValidationMessageFor<TModel, TValue>(helper, expression);
            string isrequest = "";
            if (m3 != null && metadata.IsRequired == true) isrequest = "<span style='color:red'>*</span>";
            return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1} {3} {2} {4}</td>", m1.ToHtmlString(), m2.ToHtmlString(), m3 == null ? "" : m3.ToHtmlString(), isrequest, remark));

        }
        public static MvcHtmlString MyEnumForTR<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression,Type enumType,bool isReadonly)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            StringBuilder sb=new StringBuilder();
            sb.Append(string.Format("<td class='tdRight'>{0}</td>", LabelExtensions.LabelFor<TModel, TValue>(helper, expression).ToHtmlString()));
            sb.Append("<td>");
            if (isReadonly)
            {
                sb.Append(helper.Encode(Enum.GetName(enumType,int.Parse( metadata.SimpleDisplayText))));
            }
            else
            {
                foreach (string s in Enum.GetNames(enumType))
                {

                    sb.Append(InputExtensions.RadioButtonFor(helper, expression, (int)Enum.Parse(enumType, s)));
                    sb.Append(s);
                   
                }
            }
            sb.Append("</td>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString MyEnumFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, Type enumType, bool isReadonly)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            StringBuilder sb = new StringBuilder();
            if (isReadonly)
            {
                sb.Append(helper.Encode(Enum.GetName(enumType, int.Parse(metadata.SimpleDisplayText))));
            }
            else
            {
                foreach (string s in Enum.GetNames(enumType))
                {

                    sb.Append(InputExtensions.RadioButtonFor(helper, expression, (int)Enum.Parse(enumType, s)));
                    sb.Append(s);
                    sb.Append("&nbsp;&nbsp;");

                }
            }
           
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString MyDateForTr<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression,  bool isReadonly)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<td class='tdRight'>{0}</td>", LabelExtensions.LabelFor<TModel, TValue>(helper, expression).ToHtmlString()));
            
            sb.Append("<td>");
            if (isReadonly)
            {
                sb.Append(helper.Encode(((DateTime)metadata.Model).ToString("yyyy-MM-dd")));
            }
            else
            {
                TagBuilder tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttribute("class", "text60 datepicker");
                tagBuilder.MergeAttribute("type", "text");
                string name=ExpressionHelper.GetExpressionText(expression);
                tagBuilder.MergeAttribute("name",name);
                string fullname=helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
                tagBuilder.MergeAttribute("id", fullname);
                tagBuilder.MergeAttribute("value", metadata.Model==null?"":((DateTime)metadata.Model).ToString("yyyy-MM-dd"));
                sb.Append(tagBuilder.ToString(TagRenderMode.Normal));
                if (metadata.IsRequired == true)
                {
                    sb.Append("<span style='color:red'>*</span>");
                }
                MvcHtmlString m3 = ValidationExtensions.ValidationMessageFor<TModel, TValue>(helper, expression);
                
                sb.Append(m3==null?"":m3.ToHtmlString());
            }
            sb.Append("</td>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString MyDateFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool isReadonly)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            StringBuilder sb = new StringBuilder();
           
            if (isReadonly)
            {
                if(metadata.Model is DateTime)
                sb.Append(helper.Encode(((DateTime)metadata.Model).ToString("yyyy-MM-dd")));
            }
            else
            {
                TagBuilder tagBuilder = new TagBuilder("input");

                tagBuilder.MergeAttribute("class", "text60 datepicker");
                tagBuilder.MergeAttribute("type", "text");
                string name = ExpressionHelper.GetExpressionText(expression);
                tagBuilder.MergeAttribute("name", name);
                string fullname = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
                tagBuilder.MergeAttribute("id", fullname);
                tagBuilder.MergeAttribute("value", metadata.Model== null ||((DateTime)metadata.Model)==DateTime.MinValue ? "" : ((DateTime)metadata.Model).ToString("yyyy-MM-dd"));
                sb.Append(tagBuilder.ToString(TagRenderMode.Normal));
                if (metadata.IsRequired == true)
                {
                    sb.Append("<span style='color:red'>*</span>");
                }
                MvcHtmlString m3 = ValidationExtensions.ValidationMessageFor<TModel, TValue>(helper, expression);

                sb.Append(m3 == null ? "" : m3.ToHtmlString());
            }           
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString MyDropdownForTR<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression,bool isReadonly,List<SelectListItem> list)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (isReadonly)
            {
                return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1}</td>", metadata.DisplayName, helper.Encode(metadata.SimpleDisplayText)));
            }
            MvcHtmlString m1 = LabelExtensions.LabelFor<TModel, TValue>(helper, expression);
            if (list == null) list = new List<SelectListItem>();
            if(list.Count>0&&list[0].Value!="")list.Insert(0, new SelectListItem() { Value="", Text="----" });
            MvcHtmlString m2 = SelectExtensions.DropDownListFor <TModel, TValue>(helper, expression,list);
            MvcHtmlString m3 = ValidationExtensions.ValidationMessageFor<TModel, TValue>(helper, expression);
            string isrequest = "";
            if (metadata.IsRequired == true) isrequest = "<span style='color:red'>*</span>";
            return MvcHtmlString.Create(string.Format("<td class='tdRight'>{0}</td><td>{1} {3} {2}</td>", m1.ToHtmlString(), m2.ToHtmlString(),m3==null?"": m3.ToHtmlString(),isrequest));

        }
        public static MvcHtmlString MyDropdown(this HtmlHelper helper,string name,List<SelectListItem> list)
        {

            if (list.Count==0 || list.Count > 0 && list[0].Value != "") list.Insert(0, new SelectListItem() { Value = "", Text = "----" });
            MvcHtmlString m2 = SelectExtensions.DropDownList(helper ,name,list);
            return MvcHtmlString.Create(m2.ToHtmlString());

        }
        public static MvcHtmlString MyDropdown(this HtmlHelper helper, string name, List<SelectListItem> list,string defaultValue)
        {

            if (list.Count == 0 || list.Count > 0 && list[0].Value != "") list.Insert(0, new SelectListItem() { Value = "", Text = "----" });
            foreach (var selectListItem in list)
            {
                if (selectListItem.Value == defaultValue) selectListItem.Selected = true;
            }
            MvcHtmlString m2 = SelectExtensions.DropDownList(helper, name, list);
            return MvcHtmlString.Create(m2.ToHtmlString());

        }
       
        //static List<SelectListItem> GetEnumSelectList(int val, Type enumType)
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    foreach (string s in Enum.GetNames(enumType))
        //    {
        //        SelectListItem item = new SelectListItem();
        //        item.Text = s;
        //        item.Value = ((int)Enum.Parse(enumType, s)).ToString();
        //        list.Add(item);
        //    }
        //    return list;
        //}
        public static MvcHtmlString MyRadioBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool isReadonly,List<SelectListItem> list)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<td class='tdRight'>{0}</td>", LabelExtensions.LabelFor<TModel, TValue>(helper, expression).ToHtmlString()));
            sb.Append("<td>");
            if (isReadonly)
            {
                foreach(SelectListItem item in list)
                {
                    if(metadata.SimpleDisplayText==item.Value)
                    {
                        sb.Append(item.Text);break;
                    }
                }
            }
            else
            {
                foreach(SelectListItem item in list)
                {

                    sb.Append(InputExtensions.RadioButtonFor(helper, expression, item.Value));
                    sb.Append(item.Text);
                }
            }
            sb.Append("</td>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString MyBoolFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool isReadonly)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<td class='tdRight'>{0}</td>", LabelExtensions.LabelFor<TModel, TValue>(helper, expression).ToHtmlString()));
            sb.Append("<td>");
            if (isReadonly)
            {
                if (metadata.SimpleDisplayText == "1")
                {
                    sb.Append("是");
                }
                else
                {
                    sb.Append("否");
                }
                
            }
            else
            {                
                sb.Append(InputExtensions.RadioButtonFor(helper, expression, 1));
                sb.Append("是");
                sb.Append(InputExtensions.RadioButtonFor(helper, expression, 0));
                sb.Append("否");
            }
            sb.Append("</td>");
            return MvcHtmlString.Create(sb.ToString());
        }
        /*-----------------------------------------------------------------------------*/
        public static MvcHtmlString ShortInput(this HtmlHelper helper,string name,string label)
        {
            string temp =
                "<label class=\"len60\" for=\"{0}\">{1}</label><input class=\"text60\" id=\"{0}\" name=\"{0}\" type=\"text\" />";
            return MvcHtmlString.Create(string.Format(temp, name, label));
        }
        public static MvcHtmlString SearchDate(this HtmlHelper helper, string name, string label)
        {
            string temp =
                "<label class=\"len60\" for=\"DateFrom{0}\">{1}</label><input class=\"text60 datepicker\" id=\"DateFrom{0}\" name=\"DateFrom{0}\" type=\"text\" {2} /> - <input class=\"text60 datepicker\" id=\"DateTo{0}\" name=\"DateTo{0}\" type=\"text\" {3} />";
            return MvcHtmlString.Create(string.Format(temp, name, label
                ,helper.ViewData["DateFrom"+name]==null?"":string.Format("value='{0}'",helper.ViewData["DateFrom"+name])
                ,helper.ViewData["DateTo"+name]==null?""  :string.Format("value='{0}'",helper.ViewData["DateTo"+name])));
            
        }
        public static MvcHtmlString RendYear(this HtmlHelper helper, string id)
        {
            string s = "<select id='{0}' name='{0}'>";
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(s, id));
            int year = DateTime.Today.Year;
            for (int i = year - 4; i < year; i++)
            {
                sb.Append("<option value='"); sb.Append(i); sb.Append("'>"); sb.Append(i); sb.Append("</option>");
            }
            sb.Append("<option value='"); sb.Append(year); sb.Append("' selected>"); sb.Append(year); sb.Append("</option>");
            year++;
            sb.Append("<option value='"); sb.Append(year); sb.Append("'>"); sb.Append(year); sb.Append("</option>");
            sb.Append("</select>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString RendMonth(this HtmlHelper helper, string id)
        {
            string s = "<select id='{0}' name='{0}'>";
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(s, id));
            int month = DateTime.Today.Month;
            for (int i = 1; i <= 12; i++)
            {
                string s1 = "";
                if (i == month) s1 = " selected";
                sb.Append(string.Format("<option value={0}{1}>{0}</option>", i, s1));
            }
            sb.Append("</select>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString RendSeason(this HtmlHelper helper, string id)
        {
            string s = "<select id='{0}' name='{0}'>";
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(s, id));
            int month = DateTime.Today.Month;
            for (int i = 1; i <= 4; i++)
            {
                string s1 = "";
                if (i*3>=month&&(i-1)*3<month) s1 = " selected";
                sb.Append(string.Format("<option value={0}{1}>{0}</option>", i, s1));
            }
            sb.Append("</select>");
            return MvcHtmlString.Create(sb.ToString());
        }
    }
    
}
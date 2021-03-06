﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BMS.Web
{
   

    public class JqGridSimple
    {
        static private string script =
            @"var grid{0}=jQuery('#listGrid{0}').jqGrid({{
    url: '{1}',
    datatype: 'json',
    rownumbers: true,
    {2}
    height: '100%',
    autowidth: false,
    shrinkToFit: true,
    viewrecords: true,
    jsonReader: {{ repeatitems: false }},
    caption: '',
    mtype: 'POST',
    postData: PostData,
    afterInsertRow: AfterInsertRow,
    colModel: colModelGrid      ,
    pager: jQuery('#pagerGrid{0}'),
    rowNum: 20,
    rowList: [20, 30, 50,200,10000]            
}});";
        static  string tag = @"<table id='listGrid{0}' class='scroll' cellpadding='0' cellspacing='0' ></table>
<div id='pagerGrid{0}' ></div>";
        public static string OutGrid(string path, int num)
        {
            return string.Format(script, num, path);
        }
        public static string OutGrid(string path,bool isMultiSelect=false)
        {
            string multiSelect = "";
            if (isMultiSelect) multiSelect = "multiselect:true,";
            return string.Format(script,"",path,multiSelect);
        }
        public static string OutTable(int num)
        {
            return string.Format(tag, num);
        }
        public static string OutTable()
        {
            return string.Format(tag, "");
        }
        
    }
     public static class JqGridExtensions
     {
         public static MvcHtmlString EnumNameFunction(this HtmlHelper helper, string name, Type enumType)
        {
            string temp = "function {0}Name(val){{switch(val){{{1}}}}}";
            StringBuilder sb = new StringBuilder("");
            
            foreach (var v in Enum.GetValues(enumType))
            {

                sb.Append(string.Format("case {0}:return '{1}';",(int)v, v));
            }
            return MvcHtmlString.Create(string.Format(temp,name,sb.ToString()));

        }
     }
}
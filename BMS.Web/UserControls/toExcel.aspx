<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="toExcel.aspx.cs" Inherits="BMS.Web.toExcel" ValidateRequest="false" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>
<title></title>
</head>
<body>
<form id="Form1" runat="server">
    <input type="hidden" runat="server" id="tbl" value="" />
    <asp:button runat="server" id="btn" onclick="btn_Click" style="display:none" />

    <script type="text/javascript">
        document.getElementById("<%=tbl.ClientID %>").value = window.opener.document.getElementById('<%=Request["cid"] %>').value;
        document.getElementById("<%=btn.ClientID %>").click();
    </script>

</form></body>
</html>

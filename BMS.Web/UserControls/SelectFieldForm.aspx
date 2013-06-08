<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectFieldForm.aspx.cs" Inherits="BMS.Web.UserControls.SelectFieldForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function Set(list) {

        }
        function Cancel() {
            window.close();
            return;
        }
</script>
</head>
<body>
    <form id="form1" runat="server">
   <div><br />请选择需要显示的字段：<br /><br />
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <asp:CheckBoxList ID="CheckBoxList1" runat="server" RepeatColumns="3" RepeatDirection="horizontal" Width="400"></asp:CheckBoxList>
        <br /><div align="Center"><input class="submit" id="btnSave" type="button" value="确 定" name="btnSave" runat="server"
         onserverclick ="btnSave_ServerClick" />
    &nbsp;
    <input class="submit" id="btnCancel" type="button" value="取 消" name="btnCancel" runat="server"
        onclick="Cancel();return false;" /></div>


    </div>
    </form>
</body>
</html>

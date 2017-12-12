<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GraphTester.aspx.vb" Inherits="Reporting.GraphTester" %>
<%@ Register Assembly="DTIGrid" Namespace="DTIGrid" TagPrefix="cc1" %>
<%@ Register Assembly="DTIControls" Namespace="DTIMiniControls" TagPrefix="DTI" %>
<%@ Register Assembly="DTIControls" Namespace="JqueryUIControls" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SQl Tester</title>
    	<style type="text/css">
html,body, form {
	margin:0;
	padding:0;
	height:100%; /* needed for container min-height */
	border:0;
}
.CodeMirror {
    font-size: 15px;
    line-height: 16px;
}

	</style>
	<script language="javascript" type="text/javascript">
	$(window).bind('resize',function(){
	//$('#ListBox1').height(50);
	//$('#ListBox1').height($('#ListBox1').parent().height()-5);
	}).trigger('resize');

$(function(){
//$("#ddTheme").combobox();
//$('#ListBox1').height($('#ListBox1').parent().height()-5);
    $('#ListBox1').dblclick(function () { 
      editor_tbSqlStmt.replaceSelection($('#ListBox1').val());
    });
    
$("#ddTheme").change(function () {
          selectTheme();
        }).change();
});

  function selectTheme() {
    var theme =  $("#ddTheme option:selected").text();
    editor_tbSqlStmt.setOption("theme", theme );
  }
	</script>
</head>
<body style="border:0;">
    <form id="form1" runat="server">
<table border=1 style="width:100%;height:100%;border:1px">
<tr><td width="160" rowspan=2>
<asp:ListBox ID="ListBox1" runat="server" Height="100%"></asp:ListBox>
</td><td style="vertical-align: top;">
<DTI:HighlighedEditor language="plsql" Width="100%" ID="tbSqlStmt" runat="server"> </DTI:HighlighedEditor>
<cc2:Tabs ID="tabs" runat="server" Width="95%"></cc2:Tabs>
</td>
</tr>
<tr><td height="20" style="height: 20px" valign="bottom">
    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; height: 20px;">
        <tr>
            <td style="width: 100px">
<asp:Button ID="btnRun" runat="server" Text="Run" /></td>
            <td align="right" style="width: 100px">
                &nbsp; &nbsp;<span style="font-size: 9pt; font-family: Arial"> Editor theme:</span><asp:DropDownList ID="ddTheme"
    runat="server">
</asp:DropDownList>
            </td>
        </tr>
    </table></td></tr>
</table>
    
    </form>
</body>
</html>

<%@ Page Language="vb" ValidateRequest="false" AutoEventWireup="false" CodeBehind="ReportExport.aspx.vb" Inherits="Reporting.ReportExport" %>
<%@ Register Assembly="DTIControls" Namespace="DTIMiniControls" TagPrefix="DTI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Export</title>
     <script type="text/javascript">

         $(function () {
             $(".idlst").hide();
             $('#checkAll').click(function () {
                 $('input:checkbox').prop('checked', this.checked);
                 addFromParent('#ThemeCheckBoxes');
             });
             $(".doExport").click(setIds);
             
         });

         function setIds() {
             var idlst = "";
             $('.cbID').each(function () {
                 if ($(this).is(':checked'))
                     idlst += $(this).attr("dbid") + ",";
             })
             $(".idlst").val(idlst);
         }
     </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
    <input type="checkbox" id="checkAll" > Check All <br /><br />
    <%For Each r In dsReportList.DTIReports%>
    <%=r.Name%> <input type="checkbox" class="cbID" dbid="<%=r.id %>" /><br />
    <%Next %>
    <asp:TextBox ID="tbReportList" CssClass="idlst" runat="server"></asp:TextBox>
    <asp:Button ID="btnExport" runat="server" OnClientClick="setIds();" CssClass="expBtn" Text="Export" OnClick="btnExport_Click" />

    </div>
    </form>
</body>
</html>

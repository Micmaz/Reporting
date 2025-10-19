<%@ Page Language="vb" ValidateRequest="false" AutoEventWireup="false" CodeBehind="ReportImport.aspx.vb" Inherits="Reporting.ReportImport" %>
<%@ Register Assembly="DTIControls" Namespace="DTIMiniControls" TagPrefix="DTI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Import</title>
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
             var idlst = ",";
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

    <asp:Panel ID="pnlUpload" runat="server" Visible="true">
        Please select a file to import. You can check each report that you wish to import. <br />
            <asp:FileUpload ID="fuReportsFile" accept=".xml" runat="server" />
        <asp:Button ID="btnULFile" runat="server" CssClass="btnUpl" Text="Upload File" OnClick="btnUpload_Click" />
    </asp:Panel>

    <asp:Panel ID="pnlSelectReports" runat="server" Visible="false">
    <input type="checkbox" id="checkAll" > Check All <br /><br />
    <%For Each r In dsReportList.DTIReports%>
    <%=r.Name%> <input type="checkbox" class="cbID" dbid="<%=r.id %>" />
        <%If dtExistingReports.select("name like '" & EscapeFilter(r.Name) & "'").Length > 0 then%> (Rename Existing Report) <%End If %>
        <br />
    <%Next %>
         <asp:TextBox ID="tbReportList" CssClass="idlst" runat="server"></asp:TextBox>

    <asp:Button ID="btnImport" runat="server" OnClientClick="setIds();" CssClass="expBtn" Text="Import Selected" OnClick="btnImport_Click" />
    </asp:Panel>

    </form>
</body>
</html>

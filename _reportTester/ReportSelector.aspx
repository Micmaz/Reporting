<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportSelector.aspx.cs" Inherits="_reportTester.ReportSelector" %>

<%@ Register assembly="Reporting" namespace="Reporting" tagprefix="DTI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
			<style type="text/css">
       .DTIGraph {
    float: left;
    margin: 8px;
}

			</style>
    <br />
	 <asp:Button ID="btnEnable" runat="server" Text="Toggle Report Editing" 
        onclick="btnToggleEditing_Click" CssClass="btnToggle"/>
    <DTI:ReportSelector ID="ReportSelector1" runat="server">
    </DTI:ReportSelector>
      <script>
        $(function () {
            $(".btnToggle").hide();
            $(document).keypress(function (e) {
                if (e.which == 13) {
                    $(".btnToggle").fadeIn();

                }
            });
        })
    </script>
    <div style="float:right;" visible="false" runat="server" id="edit">
        
		<a target="_blank" href="/~/res/Reporting/QueryBuilder.aspx">SQL Builder</a>
    </div>
        </div>
    </form>
</body>
</html>

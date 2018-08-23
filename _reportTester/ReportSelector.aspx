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
			
        	<asp:Button ID="btnToggleEditing" runat="server" OnClick="btnToggleEditing_Click" Text="Toggle report editing" />
			<br />


			<%@ Register assembly="Reporting" namespace="Reporting" tagprefix="DTI" %>
			<DTI:ReportSelector ID="ReportSelector1" runat="server"></DTI:ReportSelector>
        </div>
    </form>
</body>
</html>

# Reporting
Drop in reporting tool with sql query builder

Typical use:

      <asp:Button ID="btnToggleEditing" runat="server" OnClick="btnToggleEditing_Click" Text="Toggle report editing" />
			<br />
			<%@ Register assembly="Reporting" namespace="Reporting" tagprefix="DTI" %>
			<DTI:ReportSelector ID="ReportSelector1" runat="server"></DTI:ReportSelector>

Code behind:

		protected void btnToggleEditing_Click(object sender, EventArgs e)
		{
			Reporting.Report.isGlobalAdmin = !Reporting.Report.isGlobalAdmin;
			Response.Redirect("Reports.aspx");
		}

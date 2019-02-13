# Reporting
Drop in reporting tool with sql query builder

Typical use:
```HTML
<asp:Button ID="btnToggleEditing" runat="server" OnClick="btnToggleEditing_Click" Text="Toggle report editing" />
<br />
<%@ Register assembly="Reporting" namespace="Reporting" tagprefix="DTI" %>
<DTI:ReportSelector ID="ReportSelector1" runat="server"></DTI:ReportSelector>
```

Code behind:
```C#
protected void btnToggleEditing_Click(object sender, EventArgs e)
{
	Reporting.Report.isGlobalAdmin = !Reporting.Report.isGlobalAdmin;
	Response.Redirect(Request.Url.AbsoluteUri);
}
```

Web.config:
```XML
<add name="ConnectionString" connectionString="Data Source=SQLServerName;Initial Catalog=WhereTheDataIs;Integrated Security=True" providerName="System.Data.SqlClient"/>
```


Note: THIS WILL ADD TABLES TO THE DATABASE. If you want to use SQLite to store the reports set the report connection like:
In Global.asax.cs

```C#
protected void Session_Start(object sender, EventArgs e)
{
	Reporting.Report.ReportDataConnectionShared = BaseClasses.DataBase.createHelperFromConnectionName("CONNECTION NAME").defaultConnection;
}
```
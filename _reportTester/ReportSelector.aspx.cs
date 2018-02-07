using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;

namespace _reportTester
{
	public partial class ReportSelector : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Reporting.Report.ReportDataConnectionShared = new System.Data.SqlClient.SqlConnection(WebConfigurationManager.ConnectionStrings["phData"].ConnectionString);
		}

		protected void btnToggleEditing_Click(object sender, EventArgs e)
		{
			Reporting.Report.isGlobalAdmin = !Reporting.Report.isGlobalAdmin;
			Response.Redirect(Request.Url.AbsoluteUri, true);
		}
	}
}
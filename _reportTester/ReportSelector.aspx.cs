using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _reportTester
{
	public partial class ReportSelector : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnToggleEditing_Click(object sender, EventArgs e)
		{
			Reporting.Report.isGlobalAdmin = !Reporting.Report.isGlobalAdmin;
			Response.Redirect("ReportSelector.aspx", true);
		}
	}
}
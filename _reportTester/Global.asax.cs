using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;

namespace _reportTester
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            DTIControls.Share.initializePathProvider();
			
		}

		protected void Session_Start(object sender, EventArgs e)
		{
            //Reporting.Report.isGlobalAdmin = true;
            if(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ReportConnection"] !=null)
                Reporting.Report.ReportDataConnectionShared = new System.Data.SqlClient.SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ReportConnection"].ConnectionString);
			Reporting.Report.defaultParameters.Add("election_id", 105);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}
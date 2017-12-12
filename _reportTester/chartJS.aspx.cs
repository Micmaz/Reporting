using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Chart.js;

namespace _reportTester
{
	public partial class chartJS : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var dt = new DataTable();
			dt.Columns.Add("val");
			dt.Columns.Add("Label");
			dt.Columns.Add("val2");

			dt.Rows.Add(new Object[] { 20, "January", 20});
			dt.Rows.Add(new Object[] { 26,"Febuary", 30 });
			dt.Rows.Add(new Object[] { 44, "March", 40 });
			var c = new Chart.js.Chart();
				c.data = dt;
			c.numberOfSeries = 2;
			PlaceHolder1.Controls.Add(c);
			//var c1 = new System.Data.SQLite.SQLiteConnection();
			//var c2 = new SQLiteHelper.SQLiteHelper();
			
		}
	}
}
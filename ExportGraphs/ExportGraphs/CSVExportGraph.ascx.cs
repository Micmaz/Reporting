using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace ExportGraphs
{
	public partial class CSVExportGraph : Reporting.BaseGraph
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			bindToDisplay();
		}
        protected new DataTable dt
        {
            get
            {
                return new DataTable();
            }
        }
        public override void bindToDisplay()
		{
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + MakeValidFileName(this.parentReport.ReportName + ".csv"));
            HttpContext.Current.Response.ContentType = "text/csv";
            HttpContext.Current.Response.AddHeader("Pragma", "public");
            var conString = this.ReportDataConnection.ConnectionString;
            if (!conString.ToLower().Contains("pooling"))
                conString += ";Pooling=false";

            Regex regex = new Regex("\\x40\\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            List<SqlParameter> parms = new List<SqlParameter>();
            List<object> parmNames = new List<object>();
            foreach (object obj in regex.Matches(base.graph.SQLStmt))
            {
                string key = ((Match)obj).ToString().ToLower().Substring(1);
                if (!parmNames.Contains(key))
                {
                    parmNames.Add(key);
                    if (base.graph.clickedvals.ContainsKey(key))
                        parms.Add(new SqlParameter(key, base.graph.clickedvals[key]));
                    else
                        parms.Add(new SqlParameter(key, DBNull.Value));
                }
            }
            string sqlstmt = base.graph.SQLStmt;
            SqlParameter[] array = parms.ToArray();

            using (var cn = new SqlConnection(conString))
            using (var cmd = new SqlCommand(this.graph.SQLStmt,cn))
            {
                foreach (SqlParameter o in array)
                    cmd.Parameters.Add(o);

                cmd.CommandTimeout = 300;
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    var columns = new List<string>();
                    var row = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row += quoteEncode(reader.GetName(i)) + ",";
                    }
                    Response.Write(row.Trim(",".ToCharArray()) + "\n");
                    while (reader.Read())
                    {
                        row = "";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row += quoteEncode(reader.GetValue(i).ToString()) + ",";
                        }
                        Response.Write(row.Trim(",".ToCharArray())+"\n");
                    }
                }
            }
            Reporting.ReportSelector.clearReport(); //Must be done so if a page with a report selector is reloaded it dosen't still have the export report selected.
            Response.End();
        }

        public string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            string replace = Regex.Replace(name, invalidReStr, "_").Replace(";", "").Replace(",", "");
            return replace;
        }

        public static string quoteEncode(string s) {
            if (s == null) return "\"\"";
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }

	}
}
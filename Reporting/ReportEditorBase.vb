Imports BaseClasses
Public Class ReportEditorBase
    Inherits BaseClasses.BaseSecurityPage

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Report.isGlobalAdmin Then Response.End()
		jQueryLibrary.ThemeAdder.AddThemeToIframe(Me, True)
		jQueryLibrary.jQueryInclude.addStyleBlock(Me, ".ui-widget,body {font-size: 12px !important;}")
		jQueryLibrary.jQueryInclude.addScriptFile(Page, "SummerNote/css/font-awesome.min.css")
		JqueryUIControls.Dialog.registerControl(Me)
    End Sub

    Private _helper As BaseHelper
    Public Shadows Property sqlhelper() As BaseClasses.BaseHelper
        Get
            If _helper Is Nothing Then
                If Not Session("ReportSettingsConnection") Is Nothing Then
                    _helper = BaseClasses.DataBase.createHelper(Session("ReportSettingsConnection"))
                Else
                    _helper = BaseClasses.DataBase.getHelper
                End If
            End If
            Return _helper
        End Get
        Set(ByVal value As BaseClasses.BaseHelper)
            _helper = value
        End Set
    End Property

	Public Property ds() As dsReports
		Get
			If Session("DTIReportDATAsetForEditingGraphsOfAPartciularReport") Is Nothing Then
				Session("DTIReportDATAsetForEditingGraphsOfAPartciularReport") = New dsReports
			End If
			Return Session("DTIReportDATAsetForEditingGraphsOfAPartciularReport")
		End Get
		Set(ByVal value As dsReports)
			Session("DTIReportDATAsetForEditingGraphsOfAPartciularReport") = value
		End Set
	End Property

	Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        BaseClasses.DataBase.setCssClasstoId(Me)
    End Sub

	Public Function addGraph(reportID As Integer) As Integer
		Dim cnt As Integer = ds.DTIGraphs.Count + 1
		Dim ReportName As String = sqlhelper.FetchSingleValue("select name from DTIReports where id= @id", reportID)
		If cnt > 0 Then
			ReportName = ReportName & " " & cnt
		End If
		ds.DTIGraphs.AddDTIGraphsRow(Integer.Parse(Request.QueryString("repid")), ReportName,
			"--Select Y-Axis, X-Axis, OtherAxisOrParm " & vbCrLf &
			"--Ex: " & vbCrLf &
			"-- Select Count(*) as ct, user.username as Name, user.userid " & vbCrLf &
			"-- from users left outer join userLogins " & vbCrLf &
			"--   on userLogins.userid= users.userid" & vbCrLf &
			"-- Group by username,userid", cnt, ds.DTIGraphTypes(0).Id, False, False)
	End Function

	Public Function CopyReport(reportID As Integer, Optional newName As String = "") As Integer
        Dim dtRep As New Reporting.dsReports.DTIReportsDataTable
        Dim dtGraphs As New Reporting.dsReports.DTIGraphsDataTable
        sqlhelper.FillDataTable("select * from DTIReports where id = @id", dtRep, reportID)
        sqlhelper.FillDataTable("select * from DTIGraphs where Report_id = @id", dtGraphs, reportID)
        If dtRep.Count > 0 Then
            Dim repRow As dsReports.DTIReportsRow = dtRep(0)
            Dim dtnewRep As New Reporting.dsReports.DTIReportsDataTable
            Dim newRepRow As dsReports.DTIReportsRow = dtnewRep.AddDTIReportsRow(repRow.Name, repRow.Height, repRow.Width, repRow.Scrollable, repRow.showHistory, False)
            If Not newName = "" Then newRepRow.Name = newName
            sqlhelper.Update(dtnewRep)
            For Each rwGraph As dsReports.DTIGraphsRow In dtGraphs
                CopyGraph(rwGraph.Id, newRepRow.Id)
            Next
            Return newRepRow.Id
        End If
        Return -1
    End Function

    Public Function CopyGraph(GraphID As Integer, Optional newReportID As Integer = -1, Optional newName As String = "") As Integer
        Dim graphdt As New Reporting.dsReports.DTIGraphsDataTable
        sqlhelper.FillDataTable("select * from DTIGraphs where id = @id", graphdt, GraphID)
        If graphdt.Count > 0 Then
            Dim sourceRow As Reporting.dsReports.DTIGraphsRow = graphdt(0)
            Dim newGraphs As Reporting.dsReports.DTIGraphsDataTable = CopyDataTable(graphdt)
            Dim newGraphRow As Reporting.dsReports.DTIGraphsRow = newGraphs(0)
            If Not newName = "" Then newGraphRow.Name = newName
            If newReportID > -1 Then newGraphRow.Report_Id = newReportID
            sqlhelper.Update(newGraphs)

            Dim propertiesDt As New DataTable
            sqlhelper.FillDataTable("select * from DTIPropDifferences where mainID = 0 and objectKey = @objectKey", propertiesDt, "Graph_" & sourceRow.Id)
            Dim newPropertiesDt As DataTable = CopyDataTable(propertiesDt)
            For Each row As DataRow In newPropertiesDt.Rows
                row("objectKey") = "Graph_" & newGraphRow.Id
            Next
            sqlhelper.Update(newPropertiesDt)
            Return newGraphRow.Id
        End If
        Return -1
    End Function

    Public Function CopyDataTable(sourceDt As DataTable, Optional ByRef destDt As DataTable = Nothing) As DataTable
        If destDt Is Nothing Then
            destDt = sourceDt.Clone()
            destDt.Clear()
        End If

        For Each row As DataRow In sourceDt.Rows
            Dim newrow As DataRow = destDt.NewRow()
            For Each col As DataColumn In sourceDt.Columns
                If Not col.ReadOnly Then
                    newrow(col.ColumnName) = row(col.ColumnName)
                End If
            Next
            destDt.Rows.Add(newrow)
        Next

        Return destDt
    End Function

End Class

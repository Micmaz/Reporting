Public Partial Class ReportsEdit
	Inherits ReportEditorBase

	Overloads Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        jQueryLibrary.ThemeAdder.AddThemeToIframe(Me)
        If Not IsPostBack Then
            If Request.QueryString("cpid") IsNot Nothing Then
                Dim id As Integer = -1
                If Integer.TryParse(Request.QueryString("cpid"), id) Then
                    CopyReport(id, Request.QueryString("NewName"))
                End If
                Response.Redirect("ReportsEdit.aspx")

            End If
            ds.Clear()
			sqlhelper.FillDataTable("Select * from DTIReports", ds.DTIReports)
			If ds.DTIReports.Count = 0 Then
				addreport()
			End If
			loadgrid()
        End If
    End Sub

    Private Sub loadgrid()
        repeater1.DataSource = ds
        repeater1.DataMember = "DTIReports"
        repeater1.DataBind()
    End Sub

    Private Sub ReorderList1_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles repeater1.ItemCommand
        If (e.CommandName = "btnDelete") Then
            Dim id As Integer = Integer.Parse(e.CommandArgument)
            ds.DTIReports.FindById(id).Delete()
            loadgrid()
        End If
    End Sub

    Private Sub ReorderList1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles repeater1.ItemDataBound
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Then

            Dim id As Integer = Integer.Parse(CType(e.Item.FindControl("HiddenField1"), HiddenField).Value)
            Dim tbReportName As TextBox = CType(e.Item.FindControl("tbReportName"), TextBox)
            Dim tbHeight As TextBox = CType(e.Item.FindControl("tbHeight"), TextBox)
            Dim tbWidth As TextBox = CType(e.Item.FindControl("tbWidth"), TextBox)
            Dim cbScrollable As CheckBox = CType(e.Item.FindControl("cbScrollable"), CheckBox)
            Dim cbShowHistory As CheckBox = CType(e.Item.FindControl("cbShowHistory"), CheckBox)
            Dim cbPublished As CheckBox = CType(e.Item.FindControl("cbPublished"), CheckBox)

            Dim reportRow As dsReports.DTIReportsRow = ds.DTIReports.FindById(id)
            
            With reportRow
                tbReportName.Text = .Name
                tbHeight.Text = .Height
                tbWidth.Text = .Width
                cbScrollable.Checked = Not (.IsScrollableNull OrElse Not .Scrollable)
                cbShowHistory.Checked = Not (.IsshowHistoryNull OrElse Not .showHistory)
                cbPublished.Checked = Not (.IsPublishedNull OrElse Not .Published)
            End With
        End If
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
		addreport()
		loadgrid()
    End Sub

	Public Function addreport() As dsReports.DTIReportsRow
		Dim cnt As Integer = ds.DTIReports.Count + 1
		Dim repRow As dsReports.DTIReportsRow = ds.DTIReports.AddDTIReportsRow("Report-" & cnt, 400, 400, False, False, False)
		sqlhelper.Update(ds.DTIReports)
		addGraph(repRow.Id)
		sqlhelper.Update(ds.DTIGraphs)
	End Function

	'Public Sub copyReport(ByVal id As Integer, ByVal newReportName As String, Optional ByVal graphName As String = Nothing, Optional ByVal newSql As String = Nothing)
	'    Dim ds As New Reporting.dsReports
	'    Dim newDs As New Reporting.dsReports
	'    sqlHelper.FillDataSetMultiSelect("select * from DTIReports where id = @id; select * from DTIGraphs where report_ID= @id;", _
	'        ds, New String() {"DTIReports", "DTIGraphs"}, id)
	'    'sqlHelper.FillDataSetMultiSelect("select * from DTIReports where name like @name; select * from DTIGraphs where report_ID in (select id from DTIReports where name like @name);", _
	'    '    newDs, New String() {"DTIReports", "DTIGraphs"}, graphName)

	'    Dim reprow As Reporting.dsReports.DTIReportsRow = ds.DTIReports(0)
	'    If reprow.IsPublishedNull Then reprow.Published = True
	'    If newDs.DTIReports.Count = 0 Then
	'        newDs.DTIReports.AddDTIReportsRow(newReportName, reprow.Height, reprow.Width, reprow.Scrollable, reprow.showHistory, True)
	'        sqlHelper.Update(newDs.DTIReports)
	'    End If
	'    newDs.DTIReports(0).Published = False

	'    sqlHelper.Update(newDs.DTIReports)
	'    If newDs.DTIGraphs.Count = 0 Then
	'        For Each graph As Reporting.dsReports.DTIGraphsRow In ds.DTIGraphs
	'            If graphName Is Nothing Then graphName = newReportName & " - " & graph.Name
	'            If newSql Is Nothing Then newSql = graph.SelectStmt
	'            newDs.DTIGraphs.AddDTIGraphsRow(newDs.DTIReports(0).Id, graphName, newSql, graph.Order, graph.Graph_Type, graph.Drillable, graph.Export)
	'            Dim props As New PropertiesEditorvb.ComparatorDS
	'            sqlhelper.FillDataTable("select * from DTIPropDifferences where ObjectKey = @graphName", props.DTIPropDifferences, graph.Name)
	'            Dim newprops As New PropertiesEditorvb.ComparatorDS
	'            For Each r As PropertiesEditorvb.ComparatorDS.DTIPropDifferencesRow In props.DTIPropDifferences
	'                newprops.DTIPropDifferences.AddDTIPropDifferencesRow(r.PropertyPath, r.PropertyValue, r.PropertyType, graphName, r.mainID)
	'            Next
	'            sqlhelper.Update(newprops.DTIPropDifferences)
	'            'sqlhelper.SafeExecuteNonQuery( _
	'            '    "insert into DTIPropDifferences(PropertyPath, PropertyValue, PropertyType, ObjectKey, mainID) " & vbCrLf & _
	'            '    "select PropertyPath, PropertyValue, PropertyType, '" & graphName & "', mainID from DTIPropDifferences " & vbCrLf & _
	'            '    "where ObjectKey = @graphName ", New Object() {graph.Name})
	'        Next
	'        sqlhelper.Update(newDs.DTIGraphs)
	'    End If

	'End Sub

	Protected Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim reportRow As dsReports.DTIReportsRow = Nothing
        For Each item As RepeaterItem In repeater1.Items
            If (item.ItemType = ListItemType.Item) Or (item.ItemType = ListItemType.AlternatingItem) Then
                Dim id As Integer = Integer.Parse(CType(item.FindControl("HiddenField1"), HiddenField).Value)
                Dim tbReportName As TextBox = CType(item.FindControl("tbReportName"), TextBox)
                Dim tbHeight As TextBox = CType(item.FindControl("tbHeight"), TextBox)
                Dim tbWidth As TextBox = CType(item.FindControl("tbWidth"), TextBox)
                Dim cbScrollable As CheckBox = CType(item.FindControl("cbScrollable"), CheckBox)
                Dim cbShowHistory As CheckBox = CType(item.FindControl("cbShowHistory"), CheckBox)
                Dim cbPublished As CheckBox = CType(item.FindControl("cbPublished"), CheckBox)

                reportRow = ds.DTIReports.FindById(id)

                With reportRow
                    .Name = tbReportName.Text
                    .Height = tbHeight.Text
                    .Width = tbWidth.Text
                    .Scrollable = cbScrollable.Checked
                    .showHistory = cbShowHistory.Checked
                    .Published = cbPublished.Checked
                End With
            End If
        Next
       
        If reportRow IsNot Nothing Then
            sqlHelper.Update(reportRow.Table)
            loadgrid()
        End If
    End Sub
End Class
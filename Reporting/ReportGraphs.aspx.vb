Public Partial Class ReportGraphs
    Inherits ReportEditorBase

    Private _repId As Integer
    Public Property ReportID() As Integer
        Get
            Return _repId
        End Get
        Set(ByVal value As Integer)
            _repId = value
        End Set
    End Property


	Overloads Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        jQueryLibrary.ThemeAdder.AddThemeToIframe(Me)
        If Not IsPostBack Then
            If Request.QueryString("repid") <> "" Then
                ReportID = Request.QueryString("repid")
            Else : ReportID = -1
            End If

            ds.Clear()
            If ReportID <> -1 Then
				Try
					sqlhelper.FillDataSetMultiSelect("Select * from DTIReports where id = " & ReportID & "; Select * from DTIGraphs where Report_Id = " & ReportID & " order by [order];Select * from DTIGraphTypes", ds, New String() {"DTIReports", "DTIGraphs", "DTIGraphTypes"})
				Catch ex As Exception
					Report.loadDSToDatabase(sqlhelper)
                    sqlhelper.FillDataSetMultiSelect("Select * from DTIReports where id = " & ReportID & "; Select * from DTIGraphs where Report_Id = " & ReportID & " order by [order];Select * from DTIGraphTypes", ds, New String() {"DTIReports", "DTIGraphs", "DTIGraphTypes"})
                End Try
            End If
			Report.getGraphTypeList(sqlhelper, ds.DTIGraphTypes)


			If ds.DTIReports.Count > 0 Then
                tbReportName.Text = ds.DTIReports(0).Name
            End If
            hidReportID.Value = ReportID
            loadgrid()
        End If
    End Sub

    Private Sub loadgrid()
        repeater1.DataSource = ds
        repeater1.DataMember = "DTIGraphs"
        repeater1.DataBind()
    End Sub

	Private Sub ReorderList1_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles repeater1.ItemCommand
		If (e.CommandName = "btnDelete") Then
			Dim id As Integer = Integer.Parse(e.CommandArgument)
			ds.DTIGraphs.FindById(id).Delete()
			loadgrid()
		End If
	End Sub

	Private Sub ReorderList1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles repeater1.ItemDataBound
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Then

            Dim id As Integer = Integer.Parse(CType(e.Item.FindControl("HiddenField1"), HiddenField).Value)
            Dim graphrow As dsReports.DTIGraphsRow = ds.DTIGraphs.FindById(id)
            Dim tbGraphName As TextBox = CType(e.Item.FindControl("tbGraphName"), TextBox)
            Dim tbSqlStmt As DTIMiniControls.HighlighedEditor = CType(e.Item.FindControl("tbSqlStmt"), DTIMiniControls.HighlighedEditor)
            Dim ddlType As DropDownList = CType(e.Item.FindControl("ddlType"), DropDownList)
            Dim tbOrder As TextBox = CType(e.Item.FindControl("tbOrder"), TextBox)
            Dim cbDrillable As CheckBox = CType(e.Item.FindControl("cbDrillable"), CheckBox)
            Dim cbExport As CheckBox = CType(e.Item.FindControl("cbExport"), CheckBox)

            With ddlType
                .DataSource = ds
                .DataMember = "DTIGraphTypes"
                .DataTextField = "Name"
                .DataValueField = "Id"
                .DataBind()
            End With

            With graphrow
                tbGraphName.Text = .Name
                tbSqlStmt.Text = .SelectStmt
                ddlType.SelectedValue = .Graph_Type
                tbOrder.Text = .Order

                cbDrillable.Checked = Not (.IsDrillableNull OrElse Not .Drillable)
                cbExport.Checked = Not (.IsExportNull OrElse Not .Export)
            End With
        End If
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim cnt As Integer = ds.DTIGraphs.Count + 1
		addGraph(Integer.Parse(Request.QueryString("repid")))
		loadgrid()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim graphrow As dsReports.DTIGraphsRow = Nothing
        For Each item As RepeaterItem In repeater1.Items
            If (item.ItemType = ListItemType.Item) Or (item.ItemType = ListItemType.AlternatingItem) Then
                Dim id As Integer = Integer.Parse(CType(item.FindControl("HiddenField1"), HiddenField).Value)
                graphrow = ds.DTIGraphs.FindById(id)
                Dim tbGraphName As TextBox = CType(item.FindControl("tbGraphName"), TextBox)
                Dim tbSqlStmt As DTIMiniControls.HighlighedEditor = CType(item.FindControl("tbSqlStmt"), DTIMiniControls.HighlighedEditor)
                Dim ddlType As DropDownList = CType(item.FindControl("ddlType"), DropDownList)
                Dim tbOrder As TextBox = CType(item.FindControl("tbOrder"), TextBox)
                Dim cbDrillable As CheckBox = CType(item.FindControl("cbDrillable"), CheckBox)
                Dim cbExport As CheckBox = CType(item.FindControl("cbExport"), CheckBox)

                With graphrow
                    .Graph_Type = ddlType.SelectedValue
                    .Name = tbGraphName.Text
                    .SelectStmt = tbSqlStmt.Text
                    .Order = tbOrder.Text
                    .Drillable = cbDrillable.Checked
                    .Export = cbExport.Checked
                End With
            End If
        Next
        If ds.DTIReports.Count > 0 Then
            ds.DTIReports(0).Name = tbReportName.Text
            sqlHelper.Update(ds.DTIReports)
        End If
        If graphrow IsNot Nothing Then
            sqlHelper.Update(graphrow.Table)
            loadgrid()
        End If
    End Sub

    Private Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit

    End Sub
End Class
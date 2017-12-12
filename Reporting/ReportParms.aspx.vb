Partial Public Class ReportParms
    Inherits ReportEditorBase

    Public ReadOnly Property GraphID() As Integer
        Get
            If Request.QueryString("graphID") <> "" Then
                Return Request.QueryString("graphID")
            End If
            Return -1
        End Get
    End Property

    Public Property ds() As dsReports
        Get
            If Session("DTIReportDATAsetForEditingParmsOfAPartciularGraph") Is Nothing Then
                Session("DTIReportDATAsetForEditingParmsOfAPartciularGraph") = New dsReports
            End If
            Return Session("DTIReportDATAsetForEditingParmsOfAPartciularGraph")
        End Get
        Set(ByVal value As dsReports)
            Session("DTIReportDATAsetForEditingParmsOfAPartciularGraph") = value
        End Set
    End Property

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Report.isGlobalAdmin Then Response.End()
    End Sub

    Overloads Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        jQueryLibrary.ThemeAdder.AddThemeToIframe(Me)
        If Not IsPostBack Then
            ds.Clear()
            If GraphID <> -1 Then
                sqlHelper.FillDataTable("Select * from DTIGraphs where id = @graphid", ds.DTIGraphs, GraphID)
                sqlHelper.FillDataTable("Select * from DTIGraphParms where graph_id = @graphid", ds.DTIGraphParms, GraphID)
            End If
            If ds.DTIGraphParms.Count = 0 Then
                generateParms()
            End If
            hidReportID.Value = GraphID
            loadgrid()
        End If
    End Sub

    Private Sub generateParms()
        For Each r As DataRow In ds.DTIGraphParms
            r.Delete()
        Next
        For Each parm As String In ParmDisplay.getParmsFromSql(ds.DTIGraphs(0).SelectStmt)
            ds.DTIGraphParms.AddDTIGraphParmsRow(parm, parm, 0, ds.DTIGraphs(0).Id, "")
        Next
    End Sub

    Private Sub loadgrid()
        repeater1.DataSource = ds
        repeater1.DataMember = "DTIGraphParms"
        repeater1.DataBind()
    End Sub

    Private Sub ReorderList1_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles repeater1.ItemCommand
        If (e.CommandName = "btnDelete") Then
            Dim id As Integer = Integer.Parse(e.CommandArgument)
            ds.DTIGraphParms.FindById(id).Delete()
            loadgrid()
        End If
    End Sub

    Private Sub ReorderList1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles repeater1.ItemDataBound
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Then

            Dim id As Integer = Integer.Parse(CType(e.Item.FindControl("HiddenField1"), HiddenField).Value)
            Dim parmRow As dsReports.DTIGraphParmsRow = ds.DTIGraphParms.FindById(id)
            Dim tbParmName As TextBox = CType(e.Item.FindControl("tbName"), TextBox)
            Dim tbDisplayName As TextBox = CType(e.Item.FindControl("tbDisplay"), TextBox)
            Dim ddlType As DropDownList = CType(e.Item.FindControl("ddlType"), DropDownList)
            Dim tbOptions As TextBox = CType(e.Item.FindControl("tbOptions"), TextBox)

            With parmRow
                tbDisplayName.Text = .DisplayName
                tbParmName.Text = .Name
                ddlType.SelectedValue = .Parm_Type
                tbOptions.Text = .ParmProperties
            End With
        End If
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim cnt As Integer = ds.DTIGraphParms.Count + 1
        ds.DTIGraphParms.AddDTIGraphParmsRow("parm0", "parm", 0, GraphID, "")
        loadgrid()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim parmRow As dsReports.DTIGraphParmsRow = Nothing
        For Each item As RepeaterItem In repeater1.Items
            If (item.ItemType = ListItemType.Item) Or (item.ItemType = ListItemType.AlternatingItem) Then
                Dim id As Integer = Integer.Parse(CType(item.FindControl("HiddenField1"), HiddenField).Value)
                parmRow = ds.DTIGraphParms.FindById(id)
                parmRow.Graph_Id = Me.GraphID
                Dim tbParmName As TextBox = CType(item.FindControl("tbName"), TextBox)
                Dim tbDisplayName As TextBox = CType(item.FindControl("tbDisplay"), TextBox)
                Dim ddlType As DropDownList = CType(item.FindControl("ddlType"), DropDownList)
                Dim tbOptions As TextBox = CType(item.FindControl("tbOptions"), TextBox)

                With parmRow
                    .DisplayName = tbDisplayName.Text
                    .Name = tbParmName.Text
                    .Parm_Type = ddlType.SelectedValue
                    .ParmProperties = tbOptions.Text
                End With
            End If
        Next
        If parmRow IsNot Nothing Then
            sqlHelper.Update(parmRow.Table)
            loadgrid()
        End If
    End Sub

    Protected Sub btnRegen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRegen.Click
        generateParms()
        loadgrid()
    End Sub
End Class
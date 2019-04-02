Imports BaseClasses
'Imports Telerik.Web.UI

Partial Public Class BaseGraph
    Inherits System.Web.UI.UserControl
    Public Event drilldown(ByVal row As DataRow, ByVal ctrl As BaseGraph)
	Protected WithEvents propgrid As New DynamicPropertyEditor

#Region "ReportData"



	Public ReadOnly Property ReportDataConnection() As System.Data.Common.DbConnection
        Get
            Return parentReport.ReportDataConnection
        End Get
    End Property

    Private _sqlhelper As BaseClasses.BaseHelper
    Protected ReadOnly Property sqlHelper() As BaseClasses.BaseHelper
        Get
            If _sqlhelper Is Nothing Then
                _sqlhelper = BaseClasses.DataBase.getHelper()
            End If
            Return _sqlhelper
        End Get
    End Property

#End Region

#Region "Properties"

    Protected _dt As DataTable
	Public err As New JqueryUIControls.InfoDiv
	Public exportLink As New LiteralControl

	Protected ReadOnly Property dt() As DataTable
        Get
            If Me.Visible = False Then
                Return Nothing
            End If
            If _dt Is Nothing Then
                Try
                    Dim regex1 As Regex = New Regex("\x40\w+", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)
                    Dim i As Integer = 0
					Dim parms As New Generic.List(Of Object)
					Dim parmNames As New Generic.List(Of Object)
					For Each singleMatch As Match In regex1.Matches(graph.SQLStmt)
						Dim key As String = singleMatch.ToString.ToLower.Substring(1)
						If Not parmNames.Contains(key) Then
							parmNames.Add(key)
							If graph.clickedvals.ContainsKey(key) Then
								parms.Add(graph.clickedvals(key))
							Else
								parms.Add(DBNull.Value)
							End If
						End If
					Next
					'If Session("ReportDataConnection") Is Nothing Then
					'sqlHelper.SafeFillTable(graph.SQLStmt, _dt, parms.ToArray)
					'Else
					DataBase.createHelper(ReportDataConnection).SafeFillTable(graph.SQLStmt, _dt, parms.ToArray)
					'End If
					If graph.ExportExcel Then
                        excellHash(graph.GraphID) = _dt
                    End If
                Catch ex As Exception
                    graph.handelError(ex)
                    'err.isError = True
                    'err.text = ex.Message
                    'err.Visible = True
                    _dt = New DataTable("testTable")
                    _dt.Columns.Add("id")
                    _dt.Columns.Add("testcol1")
                    _dt.Columns.Add("testcol2")
                End Try
            End If
            Return _dt
        End Get
    End Property

    Protected Property dynamicObject() As Control
        Get
            Return propgrid.startingControl
        End Get
        Set(ByVal value As Control)
            propgrid.startingControl = value
            propgrid.objectKey = "Graph_" & Me.graph.GraphRowId
            propgrid.secondaryKey = Me.graph.GraphName
        End Set
    End Property

    Public ReadOnly Property parentReport() As Report
        Get
            Return Me.graph.parentReport
        End Get
    End Property

    Public Property excellHash() As Hashtable
        Get
            If Session("Listoftablesavailableforexcellexport") Is Nothing Then
                Session("Listoftablesavailableforexcellexport") = New Hashtable
            End If
            Return Session("Listoftablesavailableforexcellexport")
        End Get
        Set(ByVal value As Hashtable)
            Session("Listoftablesavailableforexcellexport") = value
        End Set
    End Property

#End Region

    Public Sub cleardata()
        _dt = Nothing
    End Sub

    Public Overridable Sub bindToDisplay()

    End Sub

    Private _graphPanel As Graph
    Protected Friend Property graph() As Graph
        Get
            Return _graphPanel
        End Get
        Set(ByVal value As Graph)
            _graphPanel = value
        End Set
    End Property

    Private Sub Page_Error(sender As Object, e As System.EventArgs) Handles Me.Error
        Dim ex As Exception = Server.GetLastError()
        If Not ex Is Nothing Then
            graph.handelError(ex)
        End If
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Try
            propgrid.sqlhelper = BaseClasses.DataBase.createHelper(Me.parentReport.ReportSettingsConnection)
            If graph.isadmin Then
                propgrid.AdminOn = True
            End If
            If propgrid.AdminOn Then
                propgrid.Height = New Unit(600)
                propgrid.Width = New Unit(600)
                propgrid.propeditor.xShowNonPublicB = False
                propgrid.propeditor.xViewOnlyB = False
                If propertyList IsNot Nothing Then
					propgrid.propeditor.xBrowsableAttributeMode = BrowsableAttributeMode.OnlyListed
					propgrid.propeditor.propertyList = propertyList
                Else
					propgrid.propeditor.xBrowsableAttributeMode = BrowsableAttributeMode.VisualStudio
				End If
            End If
            err.Visible = False

			Me.Controls.Add(err)
			Me.Controls.Add(propgrid)
			Me.Controls.Add(exportLink)
			If Me.graph.Visible Then
                bindToDisplay()
            End If
        Catch ex As Exception
            graph.handelError(ex)
        End Try

    End Sub

    Public Overridable ReadOnly Property propertyList() As String
        Get
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If graph.ExportExcel Then
			Dim url As String = "~/res/Reporting/ExcellExport.aspx?iasdf=" & HttpUtility.UrlEncode(graph.GraphID) & "&filename=" & HttpUtility.UrlEncode(graph.GraphName)
			'Dim lit As New LiteralControl
			exportLink.Text = "<a href=""#"" onclick=""return window.open('" & url & "','Window1','menubar=no,titlebar=no,status=no,location=no,width=250,height=100,toolbar=no');""><img src='~/res/BaseClasses/Scripts.aspx?f=Reporting/excel.gif' border=0></a> "
			'Me.Controls.Add(lit)
		Else
			exportLink.Text = ""
		End If
    End Sub

    'Private Sub ExcelExport(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try
    '        CType(Me.Parent, Reporting.Graph).ExcelExport(dt)
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Protected Sub doDrilldown(ByVal row As DataRow)
        Try
            If Not row Is Nothing Then
                Me.parentReport.graphClick(Me.graph, row)
                RaiseEvent drilldown(row, Me)
            End If
        Catch ex As Exception
            graph.handelError(ex)
        End Try
    End Sub

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        try
            If dynamicObject Is Nothing Then dynamicObject = Me
            If Not parentReport Is Nothing Then
                Dim i As Integer = Me.parentReport.graphsDT(0).Export
            End If
        Catch ex As Exception
            graph.handelError(ex)
        End Try

    End Sub


End Class
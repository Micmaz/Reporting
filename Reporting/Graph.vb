Imports System
Imports System.ComponentModel
Imports System.Security.Permissions
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Drawing
Imports System.Web.UI.HtmlTextWriterTag
Imports Reporting.Spider
Imports Reporting.dsReports
Imports System.Enum
Imports BaseClasses

<AspNetHostingPermission(SecurityAction.Demand, _
    Level:=AspNetHostingPermissionLevel.Minimal), _
AspNetHostingPermission(SecurityAction.InheritanceDemand, _
    Level:=AspNetHostingPermissionLevel.Minimal), _
DefaultProperty("GraphType"), _
ToolboxData("<{0}:Graph runat=""server""/>"), _
ToolboxBitmapAttribute("Icons\columnchart.gif")> _
Public Class Graph
    Inherits Panel
    Public Event errorEvent(ByVal ex As Exception)

#Region "Properties"

    Private _isadmin As Boolean = False
    Public Property isadmin() As Boolean
        Get
            If Report.isGlobalAdmin Then Return True
            If Not parentReport Is Nothing AndAlso parentReport.isadmin Then Return True
            Return _isadmin
            'Return BaseClasses.DataBase.httpSession(PropertiesEditorvb.DynamicPropertyEditor.PropEditorOnDefaultKey)
        End Get
        Set(ByVal value As Boolean)
            _isadmin = value
            'BaseClasses.DataBase.httpSession(PropertiesEditorvb.DynamicPropertyEditor.PropEditorOnDefaultKey) = value
        End Set
    End Property

    Protected Overrides ReadOnly Property TagKey() As HtmlTextWriterTag
        Get
            Return HtmlTextWriterTag.Div
        End Get
    End Property

    Public ReadOnly Property GraphRowId() As Integer
        Get
            For Each row As dsReports.DTIGraphsRow In Me.parentReport.graphsDT
                If row.Name = Me.GraphName Then
                    Return row.Id
                End If
            Next
        End Get
    End Property

    Private _helper As BaseHelper
    Friend ReadOnly Property sqlhelper() As BaseClasses.BaseHelper
        Get
            Try
                Return parentReport.sqlhelper
            Catch ex As Exception
                If _helper Is Nothing Then
                    _helper = BaseClasses.DataBase.getHelper
                End If
                If _helper Is Nothing Then
                    If session("ReportSettingsConnection") Is Nothing Then
                        _helper = BaseClasses.DataBase.getHelper
                    Else
                        _helper = BaseClasses.DataBase.createHelper(session("ReportSettingsConnection"))
                    End If
                End If

                Return _helper
                'Throw New Exception("Parent Report sqlhelper not found.")
            End Try
        End Get
    End Property

    Private _type As GraphType
    <Bindable(True), _
    Category("Graph Data"), _
    DefaultValue(GraphType.Fusion), _
    Description("The type of graph to render.  Used only when the graph doesn't exist in data."), _
    Localizable(True)> _
    Public Property GraphType() As GraphType
        Get
            Return _type
        End Get
        Set(ByVal value As GraphType)
            _type = value
        End Set
    End Property

    Private _customType As String = ""
    <Bindable(True), _
    Category("Graph Data"), _
    DefaultValue(""), _
    Description("The type of custom graph to render.  Used only when the graph doesn't exist in data."), _
    Localizable(True)> _
    Public Property CustomType() As String
        Get
            Return _customType
        End Get
        Set(ByVal value As String)
            _customType = value
        End Set
    End Property

    Public ReadOnly Property RenderType() As String
        Get
            If CustomType = "" Then
                Return System.Enum.GetName(GetType(GraphType), GraphType)
            Else
                Return CustomType
            End If
        End Get
    End Property

    Private _RenderTypeId As Integer
    Public Property RenderTypeId() As Integer
        Get
            If _RenderTypeId = Nothing Then
                For Each Type As DTIGraphTypesRow In graphTypesDT
                    If RenderType = Type.Name Then
                        _RenderTypeId = Type.Id
                        Exit For
                    End If
                Next
            End If
            Return _RenderTypeId
        End Get
        Set(ByVal value As Integer)
            For Each Type As DTIGraphTypesRow In graphTypesDT
                If value = Type.Id Then
                    _RenderTypeId = value
                    Try
                        GraphType = DirectCast(System.Enum.Parse(GetType(GraphType), Type.Name), GraphType)
                    Catch ex As Exception
                        CustomType = Type.Name
                    End Try
                    Exit For
                End If
            Next
        End Set
    End Property

    Private _order As Integer
    <Bindable(True), _
    Category("Graph Data"), _
    Description("The order to apply to the graph.  Used only when the graph doesn't exist in data."), _
    Localizable(True)> _
    Public Property Order() As Integer
        Get
            Return _order
        End Get
        Set(ByVal value As Integer)
            _order = value
        End Set
    End Property

    Private _sqlStmt As String
    <Bindable(True), _
    Category("Graph Data"), _
    Description("The sql statment to query data for the graph.  Used only when the graph doesn't exist in data." & _
    "  The graph binds to the first two columns returned."), _
    Localizable(True)> _
    Public Property SQLStmt() As String
        Get
            Return _sqlStmt
        End Get
        Set(ByVal value As String)
            _sqlStmt = value
        End Set
    End Property

    Private _name As String
    <Bindable(True), _
    Category("Graph Data"), _
    DefaultValue(""), _
    Description("The name associated to the graph.  This is what the graph's title binds to.  Used only when the graph doesn't exist in data."), _
    Localizable(True)> _
    Public Property GraphName() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
            If value Is Nothing Then
                Me.Attributes("GraphName") = "null"
            Else
                Me.Attributes("GraphName") = value
            End If

        End Set
    End Property

    Private _exportExcel As Boolean
    <Bindable(True), _
    Category("Graph Data"), _
    DefaultValue(""), _
    Description("The visiblilitly of and export to excel link associated to the graph.  Used only when the graph doesn't exist in data."), _
    Localizable(True)> _
    Public Property ExportExcel() As Boolean
        Get
            Return _exportExcel
        End Get
        Set(ByVal value As Boolean)
            _exportExcel = value
        End Set
    End Property

    Private _drillable As Boolean
    <Bindable(True), _
    Category("Graph Data"), _
    DefaultValue("true"), _
    Description("Detrmines weather the graph can be clicked to display another level of data. All undrillable graphs will be displayed untill the first drillable one is encountered. Used only when the graph doesn't exist in data."), _
    Localizable(True)> _
    Public Property drillable() As Boolean
        Get
            Return _drillable
        End Get
        Set(ByVal value As Boolean)
            _drillable = value
        End Set
    End Property

    Public ReadOnly Property graphTypesDT() As DTIGraphTypesDataTable
        Get
            If Me.session("AllGraphTypes") Is Nothing Then
                Dim dt As New DTIGraphTypesDataTable
                Try
                    sqlhelper.FillDataTable("select * from DTIGraphTypes", CType(dt, DataTable))
                Catch ex As Exception
                    'If ex.Message.Contains("Invalid object name") Then
                    Report.loadDSToDatabase(sqlhelper)
                    sqlhelper.FillDataTable("select * from DTIGraphTypes", CType(dt, DataTable))
                    'Else : Throw New Exception(ex.Message)
                    'End If
                End Try
                Me.session("AllGraphTypes") = dt
            End If
            Return CType(Me.session("AllGraphTypes"), DTIGraphTypesDataTable)
        End Get
    End Property


    Public ReadOnly Property GraphID() As String
        Get
            Dim out As String = "DTIGraph_"
            If Not parentReport Is Nothing Then
                out &= parentReport.ReportName & "_"
            End If
            out &= Me.GraphName
            Return out
        End Get
    End Property

    Private ReadOnly Property session() As System.Web.SessionState.HttpSessionState
        Get
            If Page Is Nothing Then Return System.Web.HttpContext.Current.Session Else Return Me.Page.Session
        End Get
    End Property

    Public ReadOnly Property clickedvals() As Hashtable
        Get
            If Me.parentReport IsNot Nothing Then
                Return Me.parentReport.clickedvals
            End If
            Dim idstr As String = "DTIReportClickedVals_Graph" & GraphID
            If Me.session(idstr) Is Nothing Then
                Me.session(idstr) = New Hashtable
            End If
            Return Me.session(idstr)
        End Get
    End Property

    Private ReadOnly Property MySessionName() As String
        Get
            Return "DTIGraph_" & DirectCast(Me.Parent, Report).reportId & "_" & GraphName
        End Get
    End Property

    Private _report As Report
    Public Property parentReport() As Report
        Get
            If _report Is Nothing AndAlso Parent.GetType Is GetType(Report) Then _report = Parent
            Return _report
        End Get
        Set(value As Report)
            _report = value
        End Set
    End Property

    'Public Overrides Property Visible() As Boolean
    '    Get
    '        If session(GraphID & "_isvis") Is Nothing Then
    '            session(GraphID & "_isvis") = False
    '        End If
    '        Return session(GraphID & "_isvis")
    '    End Get
    '    Set(ByVal value As Boolean)
    '        'Dim doload As Boolean = Not Visible And value
    '        session(GraphID & "_isvis") = value
    '        MyBase.Visible = value
    '        'If doload Then loadData()
    '    End Set
    'End Property

#End Region

    Private Sub Graph_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'verify that the graph is held by a Report
        'Eh why?
        'If Me.Parent.GetType IsNot GetType(Reporting.Report) Then
        '    Throw New Exception("All graphs need to be contained within a report.")
        'End If
        BaseClasses.Spider.spiderRemoveLiterals(Me)
        Me.CssClass = "DTIGraph"
        jQueryLibrary.jQueryInclude.addScriptFile(Me.Page, "/Reporting/reportRefresher.js")
        jQueryLibrary.jQueryInclude.addScriptFile(Me.Page, "/Reporting/reports.css")
    End Sub

    Public baseGraph As BaseGraph
    Public parms As New ParmDisplay
    Public Sub loadControl()
        'If Me.Visible Then
        Me.Controls.Clear()

        Try

            Me.Controls.Add(parms)
            'If Not parms.hasparms Then
            baseGraph = CType(Me.Page.LoadControl(getGraphPath(RenderType)), BaseGraph)
            baseGraph.graph = Me
            baseGraph.Visible = Not parms.hasparms
            Me.Controls.Add(baseGraph)
            'End If
        Catch ex As Exception
            handelError(ex)
        End Try
        'End If
    End Sub

    Private savedException As Exception = Nothing
    Public Sub handelError(ByVal ex As Exception)
        Try
            Dim errorStr As String = String.Format( _
            "<a href='#' onclick=""$(this).next().toggle('slow');"">Error in report: {0}</a><div id='err' style='display:none;'>{1}<br>{2}</div>", getGraphPath(RenderType), ex.Message, ex.StackTrace.Replace(vbCrLf, "<br/>"))
            Me.Controls.Add(New LiteralControl(errorStr))
        Catch ex1 As Exception
            savedException = ex1
        End Try
    End Sub

    Public Sub loadData()
        Try
            If Not baseGraph Is Nothing Then _
                Me.baseGraph.bindToDisplay()
        Catch ex As Exception
            handelError(ex)
        End Try

    End Sub

    Private Sub Graph_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'load the appropriate web user control
        loadControl()
    End Sub

    Private Function getGraphPath(ByVal cmnName As String) As String
        For Each row As DTIGraphTypesRow In graphTypesDT.Rows
			If row.Name = cmnName Then
				If row.Control_Name.ToLower.EndsWith("/fusiongraph.ascx") Then
					Return "~/res/FusionCharts/FusionGraph.ascx"
				End If
				Return row.Control_Name
			End If
		Next
        Throw New Exception("No Graph of that type found")
    End Function

    Public Overrides Sub DataBind()
        MyBase.DataBind()
    End Sub

    'Friend Sub ExcelExport(ByRef dt As DataTable)
    '    'Dim dg As New DataGrid()
    '    'Me.Controls.Add(dg)
    '    'dg.DataSource = dt
    '    'dg.DataBind()

    '    Me.Page.Response.Clear()
    '    'Me.Page.Response.ContentType = "application/vnd.ms-excel"
    '    'Me.Page.Response.AddHeader("Content-Disposition", "attachment; filename=report.xls")
    '    'Me.Page.Response.Charset = ""
    '    'Me.Page.EnableViewState = False

    '    'Dim oStringWriter As System.IO.StringWriter = New System.IO.StringWriter()
    '    'Dim oHtmlTextWriter As System.Web.UI.HtmlTextWriter = New System.Web.UI.HtmlTextWriter(oStringWriter)
    '    'dg.RenderControl(oHtmlTextWriter)
    '    'Me.Page.Response.Write(oStringWriter.ToString())
    '    'Me.Page.Response.End()
    '    Me.Page.Response.ContentType = "application/csv"
    '    Me.Page.Response.AddHeader("Content-Disposition", "inline;filename=test.csv")

    '    Dim dr As DataRow, ary() As Object, i As Integer
    '    Dim iCol As Integer

    '    'Output Column Headers
    '    For iCol = 0 To dt.Columns.Count - 1
    '        Me.Page.Response.Write(dt.Columns(iCol).ToString & vbTab)
    '    Next
    '    Me.Page.Response.Write(vbCrLf)

    '    'Output Data
    '    For Each dr In dt.Rows
    '        ary = dr.ItemArray
    '        For i = 0 To UBound(ary)
    '            Me.Page.Response.Write(ary(i).ToString & vbTab)
    '        Next
    '        Me.Page.Response.Write(vbCrLf)
    '    Next
    '    Page.Response.End()
    'End Sub

    Public Shared Function ParseGraphType(ByVal value As Integer) As String
        Return System.Enum.GetName(GetType(GraphType), DirectCast(System.Enum.Parse(GetType(GraphType), value.ToString), GraphType))
    End Function

    Public ReadOnly Property myIndex() As Integer
        Get
            If parentReport IsNot Nothing Then
                Dim i As Integer = 0
                For Each g As Graph In parentReport.GraphsList
                    If Me Is g Then
                        Return i
                    End If
                    i += 1
                Next
            End If
            Return 0
        End Get
    End Property

    Public Sub New(ByVal myreport As Report)
        _report = myreport
    End Sub

    Public Sub New()
    End Sub


    Private Sub Graph_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If baseGraph Is Nothing Then
            parms.Visible = False
            Exit Sub
        End If
        If parms.hasparms Then
            baseGraph.Visible = False
        Else
            parms.Visible = False
        End If
    End Sub

    Private Sub Graph_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If savedException IsNot Nothing Then handelError(savedException)
    End Sub
End Class

Public Enum GraphType
    'Telerik
    Grid
    Fusion
    FullTableGrid
End Enum

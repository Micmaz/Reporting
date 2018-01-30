' Report.vb
Imports System
Imports System.ComponentModel
Imports System.Security.Permissions
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Drawing
Imports System.Web.UI.HtmlTextWriterTag
Imports BaseClasses
Imports Reporting.dsReports
Imports System.IO
Imports System.Xml
Imports System.XML.Serialization
Imports Microsoft.VisualBasic.FileIO
<Assembly: TagPrefix("Reporting", "DTI")> 

<AspNetHostingPermission(SecurityAction.Demand, _
    Level:=AspNetHostingPermissionLevel.Minimal), _
AspNetHostingPermission(SecurityAction.InheritanceDemand, _
    Level:=AspNetHostingPermissionLevel.Minimal), _
DefaultProperty("ReportName"), _
ToolboxData("<{0}:Report runat=""server""> </{0}:Report>"), _
ToolboxBitmapAttribute("Icons\report.gif")> _
Public Class Report
    Inherits Panel

#Region "Properties"

    Public Shared Property ReportDataConnectionShared() As System.Data.Common.DbConnection
        Get
            Return Sharedsession("ReportDataConnection")
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            Sharedsession("ReportDataConnection") = value
        End Set
    End Property

    Public Shared Property ReportSettingsConnectionShared() As System.Data.Common.DbConnection
        Get
            Return Sharedsession("ReportSettingsConnection")
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            Sharedsession("ReportSettingsConnection") = value
        End Set
    End Property

    Private _ReportDataConnection As System.Data.Common.DbConnection
    Public Property ReportDataConnection() As System.Data.Common.DbConnection
        Get
            If session("ReportDataConnection") Is Nothing Then Return BaseClasses.DataBase.getHelper.defaultConnection
            Return session("ReportDataConnection")
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            session("ReportDataConnection") = value
        End Set
    End Property

    Public Property ReportSettingsConnection() As System.Data.Common.DbConnection
        Get
            If session("ReportSettingsConnection") Is Nothing Then Return sqlhelper.defaultConnection
            Return session("ReportSettingsConnection")
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            If ReportDataConnection.ToString = sqlhelper.defaultConnection.ToString Then
                ReportDataConnection = sqlhelper.defaultConnection
            End If
            session("ReportSettingsConnection") = value
            _helper = BaseClasses.DataBase.createHelper(value)
        End Set
    End Property

    Private _helper As BaseHelper
    Public Property sqlhelper() As BaseClasses.BaseHelper
        Get
            If _helper Is Nothing Then
                If session("ReportSettingsConnection") Is Nothing Then
                    _helper = BaseClasses.DataBase.getHelper
                Else
                    _helper = BaseClasses.DataBase.createHelper(session("ReportSettingsConnection"))
                End If
            End If
            Return _helper
        End Get
        Set(ByVal value As BaseClasses.BaseHelper)
            _helper = value
        End Set
    End Property

    Protected Overrides ReadOnly Property TagKey() _
        As HtmlTextWriterTag
        Get
            Return HtmlTextWriterTag.Div
        End Get
    End Property

    Private _ReportTitleLabel As New Label
    Public Property ReportTitleLabel() As Label
        Get
            Return _ReportTitleLabel
        End Get
        Set(ByVal value As Label)
            _ReportTitleLabel = value
        End Set
    End Property

    <Bindable(True), _
    Category("Report Data"), _
    DefaultValue(""), _
    Description("The name of the report."), _
    Localizable(True)> _
    Public Property ReportName() As String
        Get
            Dim s As String = CStr(ViewState("ReportName"))
            If s Is Nothing Then s = String.Empty
            Return s
        End Get
        Set(ByVal value As String)
            ViewState("ReportName") = value
        End Set
    End Property

    Private _height As Integer
    <Bindable(True), _
    Category("Report Data"), _
    DefaultValue(500), _
    Description("The limiting height of the report."), _
    Localizable(True)> _
    Public Property ReportHeight() As Integer
        Get
            Return _height
        End Get
        Set(ByVal value As Integer)
            _height = value
        End Set
    End Property

    Private _width As Integer
    <Bindable(True), _
    Category("Report Data"), _
    DefaultValue(500), _
    Description("The limiting width of the report."), _
    Localizable(True)> _
    Public Property ReportWidth() As Integer
        Get
            Return _width
        End Get
        Set(ByVal value As Integer)
            _width = value
        End Set
    End Property

    Private _scrollable As Boolean
    <Bindable(True), _
    Category("Report Data"), _
    DefaultValue(False), _
    Description("The enabling makes the div keep in its bounds by scrolling."), _
    Localizable(True)> _
    Public Property Scrollable() As Boolean
        Get
            Return _scrollable
        End Get
        Set(ByVal value As Boolean)
            _scrollable = value
        End Set
    End Property

    Private _history As Boolean
    <Bindable(True), _
    Category("Report Data"), _
    DefaultValue(True), _
    Description("Enable scrolls the report to the last graph clicked after drill down"), _
    Localizable(True)> _
    Public Property ShowHistory() As Boolean
        Get
            Return _history
        End Get
        Set(ByVal value As Boolean)
            _history = value
        End Set
    End Property

    Private _reportRow As DTIReportsRow
    Public ReadOnly Property reportRow() As DTIReportsRow
        Get
            If _reportRow Is Nothing Then
                Dim dt As New DTIReportsDataTable
                Try
                    sqlhelper.SafeFillTable("select * from DTIReports where name = @name", CType(dt, DataTable), New Object() {ReportName})
                Catch ex As Exception
                    If ex.Message.Contains("Invalid object name") Then
                        Report.loadDSToDatabase(sqlhelper)
                    Else : Throw New Exception(ex.Message)
                    End If
                End Try
                If dt.Rows.Count > 0 Then
                    _reportRow = CType(dt.Rows(0), DTIReportsRow)
                Else
                    _reportRow = dt.NewDTIReportsRow
                    With _reportRow
                        .Name = ReportName
                        .Height = ReportHeight
                        .Width = ReportWidth
                        .Scrollable = Scrollable
                        .showHistory = ShowHistory
                    End With
                    dt.AddDTIReportsRow(_reportRow)
                    sqlhelper.Update(CType(_reportRow.Table, DTIReportsDataTable))
                End If
            End If
            Return _reportRow
        End Get
    End Property

    Public ReadOnly Property reportId() As Integer
        Get
            Return reportRow.Id
        End Get
    End Property

    Private mygraphsDT As DTIGraphsDataTable
    Public ReadOnly Property graphsDT() As DTIGraphsDataTable
        Get
            'If session("DTIReportGraphs_" & ReportName) Is Nothing Then
            If mygraphsDT Is Nothing Then
                Dim dt As New DTIGraphsDataTable
                Try
                    sqlhelper.SafeFillTable("select * from DTIGraphs where report_id in (select id from DTIReports where name = @name) order by [Order]", CType(dt, DataTable), New Object() {ReportName})
                Catch ex As Exception
                    If ex.Message.Contains("Invalid object name") Then
                        Report.loadDSToDatabase(sqlhelper)
                    Else : Throw New Exception(ex.Message)
                    End If
                End Try
                'session("DTIReportGraphs_" & ReportName) = dt
                mygraphsDT = dt
            End If
            'Return DirectCast(session("DTIReportGraphs_" & ReportName), DTIGraphsDataTable)
            Return mygraphsDT
        End Get
    End Property

    Private mygraphParmsDT As DTIGraphParmsDataTable
    Public ReadOnly Property graphParmsDT() As DTIGraphParmsDataTable
        Get
            'If session("DTIReportGraphParms_" & ReportName) Is Nothing Then
            If mygraphParmsDT Is Nothing Then
                Dim dt As New DTIGraphParmsDataTable
                Try
                    sqlhelper.SafeFillTable("select * from DTIGraphParms where graph_id in (select id from DTIGraphs where report_id in (select id from DTIReports where name = @name))", CType(dt, DataTable), New Object() {ReportName})
                Catch ex As Exception
                    If ex.Message.Contains("Invalid object name") OrElse ex.Message.Contains("no such table") Then
                        Report.loadDSToDatabase(sqlhelper)
                    Else : Throw New Exception(ex.Message)
                    End If
                End Try
                'session("DTIReportGraphParms_" & ReportName) = dt
                mygraphParmsDT = dt
            End If
            Return mygraphParmsDT
            'Return DirectCast(session("DTIReportGraphParms_" & ReportName), DTIGraphParmsDataTable)
        End Get
    End Property

    Friend Sub cleargraphs()
        session.Remove("DTIReportGraphs_" & ReportName)
        session.Remove("DTIReportGraphParms_" & ReportName)
        session.Remove("DTIReportClickedVals_" & ReportName)
    End Sub

    Private _GraphsList As New List(Of Graph)
    Public ReadOnly Property GraphsList() As List(Of Graph)
        Get
            Return _GraphsList
            'If Session(ReportName & "_GraphsList") Is Nothing Then
            '    Session(ReportName & "_GraphsList") = New List(Of Graph)
            'End If
            'Return DirectCast(Session(ReportName & "_GraphsList"), List(Of Graph))
        End Get
    End Property

    Private Shared ReadOnly Property Sharedsession() As System.Web.SessionState.HttpSessionState
        Get
            Return System.Web.HttpContext.Current.Session
        End Get
    End Property

    Private ReadOnly Property session() As System.Web.SessionState.HttpSessionState
        Get
            If Page Is Nothing Then Return System.Web.HttpContext.Current.Session Else Return Me.Page.Session
        End Get
    End Property

    Public ReadOnly Property graphTypesDT() As DTIGraphTypesDataTable
        Get
            If session("AllGraphTypes") Is Nothing Then
                Dim dt As New DTIGraphTypesDataTable
                Try
                    sqlhelper.FillDataTable("select * from DTIGraphTypes", CType(dt, DataTable))
                Catch ex As Exception
                    If ex.Message.Contains("Invalid object name") Then
                        Report.loadDSToDatabase(sqlhelper)
                    Else : Throw New Exception(ex.Message)
                    End If
                End Try
                session("AllGraphTypes") = dt
            End If
            Return CType(session("AllGraphTypes"), DTIGraphTypesDataTable)
        End Get
    End Property

    'Public Property isadmin() As Boolean
    '    Get
    '        Return isadminShared
    '    End Get
    '    Set(ByVal value As Boolean)
    '        session(PropertiesEditorvb.DynamicPropertyEditor.PropEditorOnDefaultKey) = value
    '    End Set
    'End Property

    Private _isadmin As Boolean = False
    Public Property isadmin() As Boolean
        Get
            If Report.isGlobalAdmin Then Return True
            Return _isadmin
            'Return BaseClasses.DataBase.httpSession(PropertiesEditorvb.DynamicPropertyEditor.PropEditorOnDefaultKey)
        End Get
        Set(ByVal value As Boolean)
            _isadmin = value
            'BaseClasses.DataBase.httpSession(PropertiesEditorvb.DynamicPropertyEditor.PropEditorOnDefaultKey) = value
        End Set
    End Property

    Public Shared Property isGlobalAdmin() As Boolean
        Get
            Return BaseClasses.DataBase.httpSession("GlobalReportsAdmin")
        End Get
        Set(ByVal value As Boolean)
            BaseClasses.DataBase.httpSession("GlobalReportsAdmin") = value
        End Set
    End Property

#End Region

    Public Event PreInit()
    Public Event errorEvent(ByVal ex As Exception)

    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        MyBase.RenderContents(writer)
    End Sub

    'Public Sub initializeReport()
    '    cleargraphs()
    '    If graphsDT.Count = 0 Then
    '        Dim needUpdate As Boolean = False
    '        For Each grph As Graph In BaseClasses.Spider.spidercontrolforTypeArray(Me, GetType(Graph))
    '            Dim found As Boolean = False
    '            For Each row As DTIGraphsRow In graphsDT
    '                If row.Name = grph.GraphName Then
    '                    found = True
    '                    Exit For
    '                End If
    '            Next
    '            If Not found Then
    '                needUpdate = True
    '                Dim newGraph As DTIGraphsRow = graphsDT.NewDTIGraphsRow
    '                With newGraph
    '                    .Name = grph.GraphName
    '                    .Graph_Type = grph.RenderTypeId
    '                    .Report_Id = reportRow.Id
    '                    If grph.SQLStmt = Nothing Then Throw New Exception("Sql statement is required for .NET graphs")
    '                    .SelectStmt = grph.SQLStmt
    '                    .Order = grph.Order
    '                    .Export = grph.ExportExcel
    '                End With
    '                graphsDT.AddDTIGraphsRow(newGraph)
    '            End If
    '            Me.Controls.Remove(grph)
    '        Next
    '        'update data
    '        If needUpdate Then : sqlhelper.Update(graphsDT)
    '        End If
    '    End If
    '    'If GraphsList.Count = 0 AndAlso graphsDT.Count > 0 Then
    '    '    For Each row As dsReports.DTIGraphsRow In graphsDT
    '    '        Dim grph As New Graph(Me)
    '    '        GraphsList.Add(grph)
    '    '    Next
    '    'End If
    'End Sub

    Public  Property lastClickIndex() As Integer
        Get
            If ViewState("LastClickedIdx") Is Nothing Then Return 0
            Return ViewState("LastClickedIdx")
        End Get
        Set(ByVal value As Integer)
            ViewState("LastClickedIdx") = value
        End Set
    End Property

    Public Sub buildData()
        Dim i As Integer = 0
        If graphsDT.Count > 0 Then
            For Each grph As Graph In BaseClasses.Spider.spidercontrolforTypeArray(Me, GetType(Graph))
                Me.Controls.Remove(grph)
            Next
        End If
        Dim drillableVisited As Boolean = False
        For Each row As dsReports.DTIGraphsRow In graphsDT
            Dim grph As New Graph(Me)
            grph.parentReport = Me
            GraphsList.Add(grph)
            grph.ID = "graph_" & row.Id
            With row
                grph.SQLStmt = .SelectStmt
                grph.GraphName = .Name
				grph.GraphTypeId = .Graph_Type
				grph.Order = .Order
                grph.drillable = .Drillable
                grph.ExportExcel = Not (.IsExportNull OrElse Not .Export)
            End With
            If i <= lastClickIndex Then
                grph.Visible = True
                drillableVisited = grph.drillable
            Else
                grph.Visible = Not drillableVisited
                If Not drillableVisited Then drillableVisited = grph.drillable
            End If

            Me.Controls.Add(grph)
            'If grph.Visible Then
            '    grph.loadData()
            'End If
            i += 1
        Next
        Me.CssClass = "DTIReport"
        If isadmin Then
            ReportTitleLabel.Text = ReportName
        End If
        Controls.AddAt(0, ReportTitleLabel)
        Controls.AddAt(1, New LiteralControl("<br>"))
        If isadmin Then
            Report.reghsString(Me.Page)
			Me.Controls.AddAt(1, JqueryUIControls.Dialog.CreateDialogueUrl("~/res/Reporting/ReportGraphs.aspx?repid=" & Me.reportId,
				"<i class='fa fa-pie-chart'></i> Edit Report Graphs", JqueryUIControls.Dialog.DialogOpener.Link, 600, 600, "style='font-size: x-small;display:inline-block;margin-left: 5px;'"))
			'Dim literaladd As String = "&nbsp;&nbsp;&nbsp;<a style=""font-size: x-small;"" href=""~/res/Reporting/ReportGraphs.aspx?repid=" & Me.reportId & """ id=""btnUpload"" onclick=""return hs.htmlExpand(this, { objectType: 'iframe', width: '600', height: '720' } )"">Edit Report Graphs</a>"
			'Me.Controls.AddAt(1, New LiteralControl(literaladd))
			Controls.AddAt(1, New LiteralControl("&nbsp;&nbsp;&nbsp;"))

		End If

    End Sub

    Public Shared Sub reghsString(ByVal apage As Page)
        jQueryLibrary.jQueryInclude.RegisterJQueryUI(apage)
        'apage.ClientScript.RegisterStartupScript(GetType(Report), "highslide", "<link rel=""stylesheet"" type=""text/css"" href=""/res/DTICustomControls/highslide.css"" /><script type=""text/javascript"" src=""/res/DTICustomControls/highslide-full.js""></script>")
    End Sub

    Private Sub Report_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'If Me.Visible Then
        RaiseEvent PreInit()
        If Me.isadmin Then
		try
            Page.EnableEventValidation = False
		Catch ex As Exception
        End Try
        End If
        If Not Page.IsPostBack Then
            'initializeReport()
        End If
        buildData()
        'End If
    End Sub

    Public Sub graphClick(ByVal agraph As Graph, ByVal row As DataRow)
        Dim idx As Integer = agraph.myIndex
        idx = idx + 1
        Me.lastClickIndex = idx

        If GraphsList.Count > idx Then
            For Each col As DataColumn In row.Table.Columns
                clickedvals(col.ColumnName.ToLower) = row(col)
            Next
            GraphsList(idx).Visible = True
            If GraphsList(idx).baseGraph IsNot Nothing Then
                GraphsList(idx).baseGraph.cleardata()
            End If
            GraphsList(idx).loadData()
            agraph.Visible = True
            Dim nextdrillable As Boolean = GraphsList(idx).drillable
            idx = idx + 1
            While idx < GraphsList.Count
                'if GraphsList(idx).gr
                If nextdrillable Then
                    GraphsList(idx).Visible = False
                Else
                    GraphsList(idx).Visible = True
                    GraphsList(idx).loadData()
                End If
                If GraphsList(idx).drillable Then nextdrillable = True
                idx = idx + 1
            End While
        End If
        If Me.reportRow.showHistory Then
            jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, "document.getElementById('" & agraph.ClientID & "').scrollIntoView(true);")
        End If

    End Sub

    Public ReadOnly Property clickedvals() As Hashtable
        Get
            Dim idstr As String = "DTIReportClickedVals_" & Me.ReportName
            If Me.session(idstr) Is Nothing Then
                Me.session(idstr) = New Hashtable
            End If
            Return Me.session(idstr)
        End Get
    End Property

	Public Shared Sub loadDSToDatabase(ByRef myhelper As BaseHelper)
		Dim ds As New dsReports
		For Each dt As DataTable In ds.Tables
			If Not myhelper.checkDBObjectExists(dt.TableName) Then
				myhelper.checkAndCreateTable(dt)
				If dt.TableName = ds.DTIGraphTypes.TableName Then
					getGraphTypeList(myhelper, ds.DTIGraphTypes)

				End If
			End If
		Next
	End Sub

	Public Shared Function getGraphTypeList(myhelper As BaseHelper, Optional dtGraphTypes As dsReports.DTIGraphTypesDataTable = Nothing) As dsReports.DTIGraphTypesDataTable
		If dtGraphTypes Is Nothing Then dtGraphTypes = New dsReports.DTIGraphTypesDataTable
		If dtGraphTypes.Count = 0 Then 
			myhelper.FillDataTable("Select * from DTIGraphTypes", dtGraphTypes)
		End If
		Dim ctrlList As New List(Of String)
		'Dim tmpht As New Dictionary(Of String, List(Of Int32))
		'Dim delht As New Dictionary(Of String, List(Of dsReports.DTIGraphTypesRow))


		'Get Graph types from the database
		For Each row As dsReports.DTIGraphTypesRow In dtGraphTypes
			ctrlList.Add(row.Control_Name.ToLower())
		Next
		'For Each row As dsReports.DTIGraphTypesRow In dtGraphTypes
		'	Dim ctrlName As String = row.Control_Name.ToLower()
		'	If Not tmpht.ContainsKey(ctrlName) Then tmpht.Add(ctrlName, New List(Of Integer))
		'	tmpht(ctrlName).Add(row.Id)
		'	If Not delht.ContainsKey(ctrlName) Then delht.Add(ctrlName, New List(Of dsReports.DTIGraphTypesRow))
		'	delht(ctrlName).Add(row)
		'Next

		'Get Graph types from the Enum add them to the dt
		For Each graphName As String In System.Enum.GetNames(GetType(GraphType))
			Dim row As DTIGraphTypesRow = dtGraphTypes.NewDTIGraphTypesRow
			With row
				.Control_Name = "~/res/Reporting/" & graphName & "Graph.ascx"
				.Name = graphName
			End With
			If Not ctrlList.Contains(row.Control_Name.ToLower) OrElse dtGraphTypes.Select("Name = '" & row.Name & "'").Length = 0 Then
				dtGraphTypes.AddDTIGraphTypesRow(row)
				ctrlList.Add(row.Control_Name.ToLower())
			End If

			'If Not tmpht.ContainsKey(row.Control_Name.ToLower) Then
			'	dtGraphTypes.AddDTIGraphTypesRow(row)

			'	Dim ctrlName As String = row.Control_Name.ToLower
			'	If Not tmpht.ContainsKey(ctrlName) Then tmpht.Add(ctrlName, New List(Of Integer))
			'	tmpht(ctrlName).Add(row.Id)
			'	If Not delht.ContainsKey(ctrlName) Then delht.Add(ctrlName, New List(Of dsReports.DTIGraphTypesRow))
			'	delht(ctrlName).Add(row)
			'End If
		Next

		Dim fullList As New Dictionary(Of String, List(Of dsReports.DTIGraphTypesRow))
		Dim foundlist As New List(Of String)

		For Each row As dsReports.DTIGraphTypesRow In dtGraphTypes
			row.Control_Name = Graph.correctGraphPath(row.Control_Name)
			Dim ctrlName As String = row.Control_Name.ToLower
			If Not fullList.ContainsKey(ctrlName) Then fullList.Add(ctrlName, New List(Of DTIGraphTypesRow))
			fullList(ctrlName).Add(row)
			'If Not delht.ContainsKey(row.Control_Name.ToLower) AndAlso row.Control_Name.ToLower().StartsWith("~/res/asp/") Then
			'	Dim ctrlName As String = row.Control_Name.ToLower
			'	If Not delht.ContainsKey(ctrlName) Then delht.Add(ctrlName, New List(Of dsReports.DTIGraphTypesRow))
			'	delht(ctrlName).Add(row)
			'End If
		Next



		'Get Graph types from the Assembly cache
		For Each asm As System.Reflection.Assembly In AppDomain.CurrentDomain.GetAssemblies
			For Each tp As Type In asm.GetTypes()
				If tp.IsSubclassOf(GetType(BaseGraph)) Then
					Dim newGraph As DTIGraphTypesRow = dtGraphTypes.NewDTIGraphTypesRow
					newGraph.Control_Name = "~/res/" & tp.FullName.Replace(".", "/") & ".ascx"
					newGraph.Name = tp.Name.Replace("Graph", "")
					Dim ctrlName As String = newGraph.Control_Name.ToLower
					foundlist.Add(ctrlName)
					If Not fullList.ContainsKey(ctrlName) Then
						'ctrlList.Add(ctrlName)
						fullList.Add(ctrlName, New List(Of DTIGraphTypesRow))
						fullList(ctrlName).Add(newGraph)
						dtGraphTypes.AddDTIGraphTypesRow(newGraph)
					End If
				End If
			Next
		Next

		myhelper.Update(dtGraphTypes)

		'Remove rows not fount in the assembly cache, remove duplicate controls that may have different names.
		For Each ctrlName As String In fullList.Keys
			Dim rowlist As List(Of dsReports.DTIGraphTypesRow) = fullList(ctrlName)
			rowlist.Reverse()
			Dim skip As Boolean = False
			If foundlist.Contains(ctrlName) Then skip = True
			If ctrlName.StartsWith("~/res/asp/") Then skip = False
			For Each row As dsReports.DTIGraphTypesRow In rowlist
				If Not skip Then row.Delete()
				skip = False
			Next
		Next
		dtGraphTypes.AcceptChanges()
		Return dtGraphTypes
	End Function



	Private Sub Report_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		'renderParameters()
		Dim parms As New Label
		renderParameters(parms)
		Controls.AddAt(1, parms)
	End Sub

	Private Sub renderParameters(Optional destControl As Control = Nothing)
		If isadmin Then
			If clickedvals.Count > 0 Then
				If destControl Is Nothing Then destControl = Me
				Dim dlg As JqueryUIControls.Dialog = JqueryUIControls.Dialog.CreateDialogue("Show Current Parameters")
				dlg.OpenerAttributes = "style='font-size: x-small;display:inline-block;margin-left: 5px;'"
				dlg.OpenerText = "<i class='fa fa-list' aria-hidden='true'></i> Show Current Parameters"
				'dlg.Style = "font-size: x-small;"
				Dim str As String = ""
				For Each key As String In clickedvals.Keys
					str &= String.Format("@{0} = {1} <br/>", key, clickedvals(key))
				Next
				dlg.Controls.Add(New LiteralControl(str))
				destControl.Controls.Add(dlg)
			End If

		End If
	End Sub

#Region "Script render helpers"

	Public Shared Function getJSValue(val As Object, Optional returnEmptyString As Boolean = False) As String
		If val Is Nothing OrElse val Is DBNull.Value Then
			val = ""
		End If
		If TypeOf val Is Color Then
			val = getColorString(val)
		End If
		If TypeOf val Is Boolean Then
			If val = True Then
				Return "true"
			End If
			Return "false"
		End If
		If String.IsNullOrEmpty(val) Then
			If returnEmptyString Then Return "''"
			Return ""
		End If
		Dim outval As Double = 0
		If Double.TryParse(val, outval) Then
			Return outval.ToString()
		End If
		Return "'" & val.ToString().Replace("'", "\'") & "'"
	End Function

	Public Shared Function getBoolString(val As Boolean) As String
		If val Then Return "true"
		Return "false"
	End Function

	Public Shared Function getColorString(color As Drawing.Color, Optional nullVal As String = "") As String
		If color = Nothing Then Return nullVal
		Return String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B)
	End Function

	Public Shared Function getColorRGBString(color As Drawing.Color) As String
		If color = Nothing Then Return ""
		Return String.Format("rgba({0:X2},{1:X2},{2:X2},{3:X2})", color.R, color.G, color.B, color.A)
	End Function

	Public Shared Function UppercaseWords(ByVal value As String) As String
		Dim array() As Char = value.Replace("_", " ").ToCharArray
		' Handle the first letter in the string.
		If (array.Length >= 1) Then
			If Char.IsLower(array(0)) Then
				array(0) = Char.ToUpper(array(0))
			End If

		End If

		' Scan through the letters, checking for spaces.
		' ... Uppercase the lowercase letters following spaces.
		Dim i As Integer = 1
		Do While (i < array.Length)
			If (array((i - 1)) = Microsoft.VisualBasic.ChrW(32)) Then
				If Char.IsLower(array(i)) Then
					array(i) = Char.ToUpper(array(i))
				End If

			End If

			i = (i + 1)
		Loop

		Return New String(array)
	End Function
#End Region

End Class

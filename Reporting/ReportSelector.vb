Imports BaseClasses

Public Class ReportSelector
    Inherits Panel

    Public WithEvents ddlist As New DropDownList
    Public WithEvents shownreport As New Report
    Public WithEvents lbRevert As New LinkButton
    Public linkdiv As New Panel
    Public reportLink As New LiteralControl

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

    Public Property setParmsFromQueryString As Boolean = True

    Private _isadmin As Boolean = False
    Public Property isadmin() As Boolean
        Get
            If Report.isGlobalAdmin Then Return True
            Return _isadmin
        End Get
        Set(ByVal value As Boolean)
            _isadmin = value
        End Set
    End Property

    Private _ReportDataConnection As System.Data.Common.DbConnection
    Public Property ReportDataConnection() As System.Data.Common.DbConnection
        Get
            If _ReportDataConnection Is Nothing Then _ReportDataConnection = sqlHelper.defaultConnection
            Return _ReportDataConnection
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            _ReportDataConnection = value
            If shownreport IsNot Nothing Then
                shownreport.ReportDataConnection = value
            End If
        End Set
    End Property

    Public Property ReportSettingsConnection() As System.Data.Common.DbConnection
        Get
            Return sqlhelper.defaultConnection
        End Get
		Set(ByVal value As System.Data.Common.DbConnection)
			If value Is Nothing Then
				sqlhelper = Nothing
			Else
				sqlhelper = BaseClasses.DataBase.createHelper(value)
			End If
			If shownreport IsNot Nothing Then
				shownreport.ReportSettingsConnection = sqlhelper.defaultConnection
			End If
		End Set
	End Property



	Private _ReportSettingsConnectionName As String = Nothing
	Public Property ReportSettingsConnectionName As String
		Get
			Return _ReportSettingsConnectionName
		End Get
		Set(value As String)
			_ReportSettingsConnectionName = value
			If value Is Nothing Then
				ReportSettingsConnection = Nothing
			Else
				ReportSettingsConnection = BaseClasses.DataBase.createHelperFromConnectionName(value).defaultConnection
			End If
		End Set
	End Property

	Private _ReportDataConnectionName As String = Nothing
	Public Property ReportDataConnectionName As String
		Get
			Return _ReportDataConnectionName
		End Get
		Set(value As String)
			_ReportDataConnectionName = value
			If value Is Nothing Then
				ReportDataConnection = Nothing
			Else
				ReportDataConnection = BaseClasses.DataBase.createHelperFromConnectionName(value).defaultConnection
			End If
		End Set
	End Property


	Private ReadOnly Property session() As System.Web.SessionState.HttpSessionState
        Get
            If Page Is Nothing Then Return System.Web.HttpContext.Current.Session Else Return Me.Page.Session
        End Get
    End Property

    Public Property selectedReport() As String
        Get
            Try
                If HttpContext.Current.Request.QueryString("selectedreport") IsNot Nothing Then
                    If HttpContext.Current.Request.Form IsNot Nothing AndAlso HttpContext.Current.Request.Form.Count = 0 Then
                        session("selected Report") = HttpContext.Current.Request.QueryString("selectedreport")
                    End If
                Else
                    If HttpContext.Current.Request.QueryString("initialreport") IsNot Nothing AndAlso session("selected Report") Is Nothing Then
                        session("selected Report") = HttpContext.Current.Request.QueryString("initialreport")
                    End If
                End If
            Catch ex As Exception

            End Try

            Return session("selected Report")
        End Get
        Set(ByVal value As String)
            If value Is Nothing Then
                session.Remove("selected Report")
            Else
                session("selected Report") = value
            End If
        End Set
    End Property

	Private Sub ReportSelector_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
		jQueryLibrary.jQueryInclude.RegisterJQueryUIThemed(Me.Page)
		jQueryLibrary.jQueryInclude.addScriptFile(Page, "SummerNote/css/font-awesome.min.css")
		ddlist.AutoPostBack = True
		Dim dt As New dsReports.DTIReportsDataTable
		Try
			If Reporting.Report.isGlobalAdmin Then
				sqlhelper.FillDataTable("select * from DTIreports order by name", dt)
			Else
				sqlhelper.FillDataTable("select * from DTIreports where Published = 1 order by name", dt)
			End If
		Catch ex As Exception
			Report.loadDSToDatabase(sqlhelper)
		End Try
		ddlist.Items.Add(New ListItem("Please Select a Report", -1))
		For Each row As dsReports.DTIReportsRow In dt
			ddlist.Items.Add(New ListItem(row.Name, row.Id))
		Next
		Me.Controls.Add(ddlist)
        Me.Controls.Add(linkdiv)

        linkdiv.Style.Add("font-size", "x-small")
        linkdiv.Style.Add("display", "inline-block")
        linkdiv.Style.Add("margin-left", "5px")
        If shownreport.isadmin Then
            Report.reghsString(Me.Page)
            linkdiv.Controls.Add(JqueryUIControls.Dialog.CreateDialogueUrl("~/res/Reporting/ReportsEdit.aspx", "<i class='fa fa-book'></i> Edit Reports", JqueryUIControls.Dialog.DialogOpener.Link, 640, 600))
            'Me.Controls.Add(JqueryUIControls.Dialog.CreateDialogueUrl("~/res/Reporting/ReportsEdit.aspx", "<i class='fa fa-book'></i> Edit Reports", JqueryUIControls.Dialog.DialogOpener.Link, 640, 600, "style='font-size: x-small; display:inline-block;margin-left: 5px;'"))

            'Dim jqTester As JqueryUIControls.Dialog = JqueryUIControls.Dialog.CreateDialogueUrl("~/res/Reporting/GraphTester.aspx?sql=", _
            '    "SQL Tester", JqueryUIControls.Dialog.DialogOpener.Link, 800, 650, "style='font-size: x-small;display:none;'")
            'jqTester.ID = "sqlTester"
            'jqTester.Title = "SQL Tester"
            'Me.Controls.Add(jqTester)
            'Me.Controls.Add(New LiteralControl("&nbsp;&nbsp;&nbsp;<a style=""font-size: x-small;"" href=""~/res/Reporting/ReportsEdit.aspx"" id=""btneditRep"" onclick=""return hs.htmlExpand(this, { objectType: 'iframe', width: '600', height: '720' } )"">Edit Reports</a>"))
        End If

		If Not selectedReport Is Nothing Then
			shownreport.ReportName = selectedReport
            shownreport.ReportSettingsConnection = Me.ReportSettingsConnection
            shownreport.setParmsFromQueryString = setParmsFromQueryString
            lbRevert.Text = "<i class='fa fa-times-circle'></i> Clear Report "
            linkdiv.Controls.Add(New LiteralControl(" &nbsp;&nbsp;"))
            linkdiv.Controls.Add(lbRevert)
            linkdiv.Controls.Add(New LiteralControl(" &nbsp;&nbsp;"))
            If Page.Request.Params("__EVENTTARGET") = lbRevert.UniqueID Then
				If Not shownreport Is Nothing Then shownreport.cleargraphs()
				clearReport(selectedReport)
				selectedReport = Nothing
				Me.Page.Response.Redirect(Me.Page.Request.Url.PathAndQuery)
			End If
			Me.Controls.Add(New LiteralControl("<br>"))
			Me.Controls.Add(shownreport)
		End If
        linkdiv.Controls.Add(reportLink)


        'If Not Page.IsPostBack Then
        '    selectedReport = Nothing
        '    shownreport.Visible = False
        '    ddlist.Visible = True
        'End If
    End Sub

    Public Function getReportLink() As String
        Dim url As String = HttpContext.Current.Request.Url.AbsolutePath & "?initialreport=" & HttpUtility.UrlEncode(selectedReport)
        If shownreport IsNot Nothing Then
            url &= "&LastClickedIndex=" & shownreport.lastClickIndex
        End If
        For Each key As String In Me.shownreport.clickedvals.Keys
            If key.ToLower() = "initialreport" OrElse key.ToLower() = "selectedreport" Then
            Else
                Try
                    If (Me.shownreport.clickedvals(key) Is DBNull.Value) Then
                        url &= "&" & HttpUtility.UrlEncode(key) & "=NULL"
                    Else
                        url &= "&" & HttpUtility.UrlEncode(key) & "=" & HttpUtility.UrlEncode(Me.shownreport.clickedvals(key)).ToString()
                    End If


                Catch ex As Exception

                End Try
            End If
        Next
        Return url
    End Function

    Private Sub ddlist_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlist.SelectedIndexChanged
        If Not shownreport Is Nothing Then shownreport.cleargraphs()
        selectedReport = Nothing
        selectedReport = ddlist.SelectedItem.Text
        Me.Page.Response.Redirect(Me.Page.Request.Url.PathAndQuery.Replace("selectedreport=", "initialreport="))
        'shownreport.ReportName = ddlist.SelectedItem.Text
        'shownreport.Visible = True
        'ddlist.Visible = False
        'shownreport.initializeReport()
    End Sub

    Public Shared Sub clearReport(Optional ByVal reportname As String = Nothing)
        Dim s As HttpSessionState = HttpContext.Current.Session
        If Not s Is Nothing Then
            s.Remove("selected Report")
            If reportname Is Nothing Then
                Dim remkeys As New List(Of String)

                For Each key As String In s.Keys
                    If key.StartsWith("DTIReport") Then
                        remkeys.Add(key)
                    End If
                Next
                For Each key As String In remkeys
                    s.Remove(key)
                Next
            Else
                s.Remove("DTIReportGraphs_" & reportname)
                s.Remove("DTIReportGraphParms_" & reportname)
                s.Remove("DTIReportClickedVals_" & reportname)
            End If
        End If
    End Sub

    Private Sub lbRevert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbRevert.Click
        If Not shownreport Is Nothing Then shownreport.cleargraphs()
        selectedReport = Nothing
        Me.Page.Response.Redirect(Me.Page.Request.Url.PathAndQuery)
    End Sub

    Private Sub ReportSelector_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not selectedReport Is Nothing Then _
            reportLink.Text = "  <a target='_blank' href='" & getReportLink() & "'><i class='fa fa-link'></i> Report link</a>"

    End Sub
End Class

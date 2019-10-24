Partial Public Class GraphTester
	Inherits ReportEditorBase
	Dim DTIGrid1 As New DTIGrid.DTIGrid
    Dim alert As New JqueryUIControls.InfoDiv

    Public ReadOnly Property sqlStr() As String
        Get
            If Me.tbSqlStmt.Text = "" Then
				'Return DTIMiniControls.TextBoxEncoded.decodeFromURL(Request.QueryString("sql"))
				Return Request.QueryString("sql")
			End If
            Return Me.tbSqlStmt.Text
        End Get
    End Property

    Public ReadOnly Property reportname() As String
        Get
            Return DTIMiniControls.TextBoxEncoded.decodeFromURL(Request.QueryString("reportname"))
        End Get
    End Property

    Public ReadOnly Property clickedvals() As Hashtable
        Get
            Dim idstr As String = reportname & "_ClickedrowVals"
            Return Me.Session(idstr)
        End Get
    End Property

    Private _helper As BaseClasses.BaseHelper
    Public ReadOnly Property helper() As BaseClasses.BaseHelper
        Get
            If Not Report.ReportDataConnectionShared Is Nothing Then
                If _helper Is Nothing Then
                    _helper = BaseClasses.DataBase.createHelper(Report.ReportDataConnectionShared)
                End If
                Return _helper
            Else
                Return sqlhelper
            End If
        End Get
    End Property

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Report.isGlobalAdmin Then Response.End()
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		jQueryLibrary.ThemeAdder.AddThemeToIframe(Me)
        tbSqlStmt.allThemeCssAvailable = True
        Dim value As Integer = -1
        If Integer.TryParse(ddTheme.SelectedValue, value) Then
            tbSqlStmt.theme = ddTheme.SelectedValue
        End If
        If ddTheme.Items.Count = 0 Then
            For Each theme As Integer In [Enum].GetValues(tbSqlStmt.theme.GetType)
                ddTheme.Items.Add(New ListItem([Enum].GetName(tbSqlStmt.theme.GetType, theme), theme))
            Next
        End If
        ddTheme.SelectedValue = tbSqlStmt.theme
        tbSqlStmt.Text = sqlStr
        If tbSqlStmt.Text = "" Then
            tbSqlStmt.Text = "--Select * from Table " & vbCrLf & vbCrLf & vbCrLf
        End If
        DTIGrid1.DataSource = New DataTable
        DTIGrid1.Width = New WebControls.Unit("100%")
        Me.tabs.addTab("Grid", DTIGrid1)
        Me.tabs.addTab("Message", alert)
        Dim dt As DataTable = tableList()
        ListBox1.Items.Clear()
        For Each row As DataRow In dt.Rows
            ListBox1.Items.Add(row(0))
            tbSqlStmt.additionalAutocomp.Add(row(0))
        Next
        jQueryLibrary.ThemeAdder.AddTheme(Me)
    End Sub

    Public Function tableList() As DataTable
        Dim helpername As String = helper.GetType.Name.ToLower()
        If helpername.StartsWith("sqlite") Then
            Return helper.FillDataTable( _
            "SELECT name FROM  " & vbCrLf & _
            "   (SELECT * FROM sqlite_master UNION ALL " & vbCrLf & _
            "    SELECT * FROM sqlite_temp_master) " & vbCrLf & _
            "WHERE type='table' " & vbCrLf & _
            "ORDER BY name " & vbCrLf)
        ElseIf helpername.StartsWith("sqlhelper") Then
            Return helper.FillDataTable("SELECT name FROM sysobjects WHERE type = 'U' order by name")
        ElseIf helpername.StartsWith("mysql") Then
            Return helper.FillDataTable("show tables")
        End If
        Return Nothing
    End Function

    Private Sub btnRun_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRun.Click
        Try
            Dim regex1 As Regex = New Regex("\x40\w+", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)
            Dim i As Integer = 0
            Dim parms As New Generic.List(Of Object)
            For Each singleMatch As Match In regex1.Matches(Me.tbSqlStmt.Text)
                Dim key As String = singleMatch.ToString.ToLower.Substring(1)
                If clickedvals.ContainsKey(key) Then
                    parms.Add(clickedvals(key))
                Else
                    parms.Add(DBNull.Value)
                End If
            Next
            Dim dt As New DataTable
            helper.SafeFillTable(Me.tbSqlStmt.Text, dt, parms.ToArray)
            DTIGrid1.ShrinkToFit = False
            Me.DTIGrid1.DataSource = dt
            DTIGrid1.DataBind()
            alert.text = "Fetched " & dt.Rows.Count & " rows"
        Catch ex As Exception
            alert.isError = True
            alert.Text = ex.Message & vbCrLf & "<br>"
            jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me, "setTimeout(""$('#" & tabs.ClientID & "').tabs( 'select' , 1 );"",500);")
        End Try
    End Sub
End Class
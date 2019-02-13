Partial Public Class GridGraph
    Inherits BaseGraph
    Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled


    Public Overrides Sub bindToDisplay()
        '_dt = New DataTable()
        propgrid.manuallySetProperties = True

        DTIGrid1.DataSource = dt
        DTIGrid1.EnablePaging = False
        DTIGrid1.PageSize = 1000
        'DTIGrid1.DefaultConnection = graph.parentReport.ReportDataConnection
        'Dim regex1 As Regex = New Regex("\x40\w+", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant _
        '    Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)
        'Dim i As Integer = 0
        'Dim parmList As New List(Of Object)
        'For Each singleMatch As Match In regex1.Matches(graph.SQLStmt)
        '    parmList.Add(graph.parentReport.clickedvals(singleMatch.ToString().Trim("@").ToLower))
        'Next
        'DTIGrid1.ajaxEnable = False
        'DTIGrid1.DataTableParamArray = parmList.ToArray
        'DTIGrid1.DataTableName = graph.SQLStmt
        'DTIGrid1.ReadOnlyMode = True
        DTIGrid1.EnableSorting = True
        DTIGrid1.Title = ""
        Me.dynamicObject = DTIGrid1
        propgrid.setProperties()
        DTIGrid1.AutoPostBack = Me.graph.drillable

        DTIGrid1.DataBind()
        MyBase.bindToDisplay()

    End Sub

    Private Function getTablename() As String
        Dim regTableName As New Regex("from\s(?<table>\w*?)\s", options)
        Return regTableName.Match(graph.SQLStmt).Groups("table").ToString()
    End Function

    Private Function getQuery() As String
        Dim q As String = ""
        Dim regwhere As New Regex("where\s", options)
        Dim regsort As New Regex("order\sby\s", options)
        If graph.SQLStmt.Length > 0 And regwhere.Match(graph.SQLStmt).Success Then
            q = graph.SQLStmt.Substring(graph.SQLStmt.IndexOf("where ", StringComparison.CurrentCultureIgnoreCase) + 6)
            If regsort.Match(q).Success Then
                q = q.Substring(0, q.LastIndexOf("order by ", StringComparison.CurrentCultureIgnoreCase))
            End If
        End If
        Return q
    End Function

    Public Overrides ReadOnly Property propertyList() As String
		Get
			Return "ajaxEnable,BackColor,BorderColor,BorderStyle,BorderWidth,Columns,EnablePaging,EnableSorting,EnableSearching,ForeColor,Height,hiddenColumns,PageSize,renderAsTable,ShowDateAndTime,ShrinkToFit,Title,visibleColumns,Width"
		End Get
	End Property

    Private Function getsort() As String
        Dim regwhere As New Regex("order\sby\s", options)
        Dim q As String = ""
        If graph.SQLStmt.Length > 0 And regwhere.Match(graph.SQLStmt).Success Then
            q = graph.SQLStmt.Substring(graph.SQLStmt.LastIndexOf("order by ", StringComparison.CurrentCultureIgnoreCase) + 8)
        End If
        Return q
    End Function

    Private Sub DTIGrid1_Click(ByRef row As DTIGrid.DTIGridRow) Handles DTIGrid1.Click
        doDrilldown(row.dataRow)
    End Sub

End Class
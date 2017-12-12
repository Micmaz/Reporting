Imports DTIGrid

Partial Public Class FullTableGridGraph
    Inherits BaseGraph

    Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled

    Protected WithEvents DTIGrid1 As New Global.DTIGrid.DTIDataGrid
    Public Overrides Sub bindToDisplay()
        DTIGrid1.gridConnection = ReportDataConnection
        Dim tableName As String = getTablename()

        If Not tableName Is Nothing Then
            DTIGrid1.DataTableName = tableName
            DTIGrid1.Title = tableName
            DTIGrid1.Width = 700
            DTIGrid1.Height = 500

            'If Not Me.propgrid.AdminOn Then
            DTIGrid1.EnableEditing = False
            'End If
        End If

        Me.dynamicObject = DTIGrid1
    End Sub

    Private Function getTablename() As String
        Dim regTableName As New Regex("from\s(?<table>\w*?)\s", options)
        Dim tableName As String
        If Not graph.SQLStmt.ToLower.Contains("where") Then
            graph.SQLStmt &= " "
        End If

        tableName = regTableName.Match(graph.SQLStmt).Groups("table").ToString()
        Return tableName
    End Function

    Private Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        PlaceHolder1.Controls.Add(DTIGrid1)
    End Sub
End Class
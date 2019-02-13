Imports DTIGrid

Partial Public Class TableEditorGridGraph
	Inherits BaseGraph

	Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled

    Protected WithEvents DTIGrid1 As DTIDataGrid
    Public Overrides Sub bindToDisplay()
        If DTIGrid1 Is Nothing Then
            DTIGrid1 = New DTIDataGrid
            PlaceHolder1.Controls.Add(DTIGrid1)
        End If
        propgrid.manuallySetProperties = True
        DTIGrid1.gridConnection = ReportDataConnection
        Dim tableName As String = getTablename()

        If Not tableName Is Nothing Then
            DTIGrid1.DataTableName = tableName
            DTIGrid1.Title = tableName
            'DTIGrid1.Width = 700
            'DTIGrid1.Height = 500

            'If Not Me.propgrid.AdminOn Then
            'DTIGrid1.EnableEditing = True
            'End If
        End If

        DTIGrid1.Title = ""

        DTIGrid1.AutoPostBack = Me.graph.drillable
        Me.dynamicObject = DTIGrid1
        propgrid.setProperties()

        DTIGrid1.databind()
        MyBase.bindToDisplay()
    End Sub

    Public Overrides ReadOnly Property propertyList() As String
        Get
            Return "DataTableSearch,EnableEditing,DataTableName,DataTableKey,ajaxEnable,BackColor,BorderColor,BorderStyle,BorderWidth,ColumnTitles,EnablePaging,EnableSorting,EnableSearching,ForeColor,Height,hiddenColumns,PageSize,renderAsTable,ShowDateAndTime,ShrinkToFit,Title,visibleColumns,Width"
        End Get
    End Property

    Private Function getTablename() As String
        Dim regTableName As New Regex("from\s(?<table>.*?)\s", options)
        Dim tableName As String
		If Not graph.SQLStmt.ToLower.Contains("where") Then
			graph.SQLStmt &= " "
		End If

		tableName = regTableName.Match(graph.SQLStmt).Groups("table").ToString()
		Return tableName
	End Function



    Private Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub

    Private Sub DTIGrid1_gridBindingError(ex As Exception) Handles DTIGrid1.gridBindingError

    End Sub
End Class
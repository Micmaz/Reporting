Partial Public Class GroupedTableGraph
    Inherits Reporting.BaseGraph

    Public Overrides Sub bindToDisplay()
        groupedTbl.DataSource = dt
        Me.dynamicObject = Me.groupedTbl
        MyBase.bindToDisplay()
    End Sub

    Private Sub groupedTbl_hadError(ex As System.Exception) Handles groupedTbl.hadError
        graph.handelError(ex)
    End Sub

End Class
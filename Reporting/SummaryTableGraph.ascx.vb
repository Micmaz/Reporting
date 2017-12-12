Partial Public Class SummaryTableGraph
    Inherits Reporting.BaseGraph

    Public Overrides Sub bindToDisplay()
        groupedTbl.DataSource = dt
        Me.dynamicObject = Me.groupedTbl
        MyBase.bindToDisplay()
    End Sub

End Class
Partial Public Class HtmlTableGraph
	Inherits Reporting.BaseGraph

	Public Overrides Sub bindToDisplay()
		groupedTbl.DataSource = dt
		Me.dynamicObject = Me.groupedTbl
		MyBase.bindToDisplay()
	End Sub

	Public Overrides ReadOnly Property propertyList() As String
		Get
			Return "Caption,CaptionAlign,CellPadding,CellSpacing,VisibleCols,tableStyle,Height,Width,groupedCol,GridLines,BackColor,BorderColor,BorderWidth,ColumnTitles,CssClass,nullDisplay,removeRepeats,TotalColumnFormat,TotalFormat"
		End Get
	End Property

	Private Sub groupedTbl_hadError(ex As System.Exception) Handles groupedTbl.hadError
		graph.handelError(ex)
	End Sub

End Class
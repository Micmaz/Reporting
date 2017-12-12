Imports System.Text.RegularExpressions
Imports Reporting

Partial Public Class FusionGraph
	Inherits BaseGraph


	Public Overrides Sub bindToDisplay()
		Me.Chart1.data = Me.dt
		Me.dynamicObject = Chart1
		Chart1.chartTitle = Me.graph.GraphName
		If parentReport IsNot Nothing Then
			If parentReport.graphTypesDT.Count > graph.myIndex Then Me.Chart1.drillable = True
		End If
		Me.Chart1.drillable = Me.graph.drillable
		MyBase.bindToDisplay()
	End Sub

	Private Sub Chart1_DrillDownEvent(ByVal sender As FusionCharts.Chart, ByVal row As System.Data.DataRow, ByVal columnName As String, ByVal columnValue As String) Handles Chart1.DrillDownEvent
		doDrilldown(row)
	End Sub

	'Public Overrides ReadOnly Property propertyList() As String
	'	Get
	'		Return "chartTitle,chartType,formatNumberScale,Height,Width,numberOfSeries,numberPrefix,numberSuffix,palette,rotateValues,displayedTextColStr,showValues,Wrap,setAdaptiveYMin,xAxisMinValue,xAxisMaxValue,yAxisMinValue,yAxisMaxValue,xAxisName,yAxisName,addtionalChartProperties,baseFontSize,baseFont,baseFontColor,labelDisplay,roundEdges,showBorder,theme,showShadow,showGradient,showPlotBorder,animation,backgroundColor,canvasColor,captionColor,showLegend,plotSpacePercent,chartColors"
	'	End Get
	'End Property

	Private Sub DynamicPropertyEditor1_objectPropertiesSet(ByVal objectInstance As Object) Handles propgrid.objectPropertiesSet
		Dim regex1 As Regex = New Regex("\x40\w+", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)
		Dim i As Integer = 0
		Dim parms As New Generic.List(Of Object)
		If Chart1.chartTitle Is Nothing Then Chart1.chartTitle = ""
		For Each singleMatch As Match In regex1.Matches(Chart1.chartTitle)
			Dim key As String = singleMatch.ToString.ToLower.Substring(1)
			If graph.clickedvals.ContainsKey(key) Then
				Chart1.chartTitle = Chart1.chartTitle.Replace(singleMatch.Value, graph.clickedvals(key))
			End If
		Next
	End Sub
End Class
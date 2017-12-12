Imports System.Web.UI.WebControls
Imports System.Web.UI

Public Class Chart
    Inherits ChartBase

    Public Event DrillDownEvent(ByVal sender As Chart, ByVal row As DataRow, ByVal columnName As String, ByVal columnValue As String)


    Private Function getXml() As String
        Return outputXML()
    End Function

#Region "Filenames and enum"


    Private ReadOnly Property singleset() As Boolean
        Get
            If chartType < 9 Then Return True
            If chartType > 21 And chartType < 24 Then Return True
            If chartType = 31 Then Return True
            Return False
        End Get
    End Property

    Public Enum chartTypeEnum
        Area2D = 0
        Bar2D = 1
        Bubble = 2
        Column2D = 3
        Column3D = 4
        Doughnut2D = 5
        Doughnut3D = 6
        FCExporter = 7
        Line = 8
        MSArea = 9
        MSBar2D = 10
        MSBar3D = 11
        MSColumn2D = 12
        MSColumn3D = 13
        MSColumn3DLineDY = 14
        MSColumnLine3D = 15
        MSCombi2D = 16
        MSCombi3D = 17
        MSCombiDY2D = 18
        MSLine = 19
        MSStackedColumn2D = 20
        MSStackedColumn2DLineDY = 21
        Pie2D = 22
        Pie3D = 23
        Scatter = 24
        ScrollArea2D = 25
        ScrollColumn2D = 26
        ScrollCombi2D = 27
        ScrollCombiDY2D = 28
        ScrollLine2D = 29
        ScrollStackedColumn2D = 30
        SSGrid = 31
        StackedArea2D = 32
        StackedBar2D = 33
        StackedBar3D = 34
        StackedColumn2D = 35
        StackedColumn3D = 36
        StackedColumn3DLineDY = 37
        zoomline
        pareto2d
        pareto3d
        marimekko
        angulargauge
        bulb
        cylinder
        hled
        hlineargauge
        thermometer
        vled
        hbullet
        vbullet
        funnel
        pyramid
        gantt
    End Enum


#End Region

#Region "New Properties"

    Private addtionalChartValue As String = ""
    Public Property addtionalChartProperties() As String
        Get
            Return addtionalChartValue
        End Get
        Set(ByVal value As String)
            addtionalChartValue = value
        End Set
    End Property

    Private addtionalSetValue As String = ""
    Public Property addtionalItemLevelProperties() As String
        Get
            Return addtionalSetValue
        End Get
        Set(ByVal value As String)
            addtionalSetValue = value
        End Set
    End Property

    Public Property formatNumberScale() As String
        Get
            Return xmlprop("formatNumberScale")
        End Get
        Set(ByVal value As String)
            If Not Double.TryParse(value, New Double) Then
                removeValue("formatNumberScale")
            Else
                xmlprop("formatNumberScale") = value
            End If
        End Set
    End Property


    Public Property yAxisMinValue() As String
        Get
            Return xmlprop("yAxisMinValue")
        End Get
        Set(ByVal value As String)
            If Not Double.TryParse(value, New Double) Then
                removeValue("yAxisMinValue")
            Else
                xmlprop("yAxisMinValue") = value
            End If
        End Set
    End Property

    Public Property yAxisMaxValue() As String
        Get
            Return xmlprop("yAxisMaxValue")
        End Get
        Set(ByVal value As String)
            If Not Double.TryParse(value, New Double) Then
                removeValue("yAxisMaxValue")
            Else
                xmlprop("yAxisMaxValue") = value
            End If
        End Set
    End Property

    Public Property xAxisMinValue() As String
        Get
            Return xmlprop("xAxisMinValue")
        End Get
        Set(ByVal value As String)
            If Not Double.TryParse(value, New Double) Then
                removeValue("xAxisMinValue")
            Else
                xmlprop("xAxisMinValue") = value
            End If
        End Set
    End Property

    Public Property xAxisMaxValue() As String
        Get
            Return xmlprop("xAxisMaxValue")
        End Get
        Set(ByVal value As String)
            If Not Double.TryParse(value, New Double) Then
                removeValue("xAxisMaxValue")
            Else
                xmlprop("xAxisMaxValue") = value
            End If
        End Set
    End Property

    Public Property setAdaptiveYMin() As Boolean
        Get
            Return xmlBoolProp("setAdaptiveYMin")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("setAdaptiveYMin") = value
        End Set
    End Property

#End Region

#Region "Layout properties"

    Public Property theme As ChartTheme
        Get
            Return xmlprop("themeNum")
        End Get
        Set(value As ChartTheme)
            xmlprop("themeNum") = value

            If value = ChartTheme.None Then
                props.remove("theme")
            Else
                xmlprop("theme") = [Enum].GetName(value.GetType, value).ToLower
            End If
        End Set
    End Property

    Public Property plotSpacePercent() As Double
        Get
            Return xmlprop("plotspacepercent")
        End Get
        Set(ByVal value As Double)
            If value <= 0 Then
                props.remove("plotspacepercent")
            Else
                xmlprop("plotspacepercent") = value
            End If

        End Set
    End Property


    Public Property baseFontSize() As Double
        Get
            Return xmlprop("baseFontSize")
        End Get
        Set(ByVal value As Double)
            If value <= 0 Then
                props.remove("baseFontSize")
            Else
                xmlprop("baseFontSize") = value
            End If

        End Set
    End Property

    Public Property baseFont() As String
        Get
            Return xmlprop("baseFont")
        End Get
        Set(ByVal value As String)
            xmlprop("baseFont") = value
            If value = "" Then
                props.remove("baseFont")
            End If
        End Set
    End Property

    Public Property captionColor() As System.Drawing.Color
        Get
            Return props.xmlColorProperty("captionFontColor")
        End Get
        Set(ByVal value As System.Drawing.Color)
            props.xmlColorProperty("captionFontColor") = value
        End Set
    End Property


    Public Property baseFontColor() As System.Drawing.Color
        Get
            Return props.xmlColorProperty("baseFontColor")
        End Get
        Set(ByVal value As System.Drawing.Color)
            props.xmlColorProperty("baseFontColor") = value
        End Set
    End Property

    Public Property backgroundColor() As System.Drawing.Color
        Get
            Return props.xmlColorProperty("bgColor")
        End Get
        Set(ByVal value As System.Drawing.Color)
            props.xmlColorProperty("bgColor") = value
        End Set
    End Property

    Public Property canvasColor() As System.Drawing.Color
        Get
            Return props.xmlColorProperty("canvasBgColor")
        End Get
        Set(ByVal value As System.Drawing.Color)
            props.xmlColorProperty("canvasBgColor") = value
        End Set
    End Property

    Public Property chartTitle() As String
        Get
            Return xmlprop("caption")
        End Get
        Set(ByVal value As String)
            xmlprop("caption") = value
        End Set
    End Property


    Public Property chartColors() As String
        Get
            Return xmlprop("paletteColors")
        End Get
        Set(ByVal value As String)
            xmlprop("paletteColors") = value
        End Set
    End Property

    Public Property palette() As palettEnum
        Get
            Return xmlprop("palette")
        End Get
        Set(ByVal value As palettEnum)
            If value = palettEnum.Not_Set Then props.remove("palette")
            xmlprop("palette") = value
        End Set
    End Property

    Private _labelDisplay As labelDisplayEnum
    Public Property labelDisplay() As labelDisplayEnum
        Get
            Return _labelDisplay
        End Get
        Set(ByVal value As labelDisplayEnum)
            _labelDisplay = value
            If value = labelDisplayEnum.Not_Set Then
                props.remove("labelDisplay")
            Else
                If value = labelDisplayEnum.SLANT Then
                    xmlprop("labelDisplay") = labelDisplayEnum.ROTATE.ToString()
                    Me.slantLabels = True
                Else
                    slantLabels = False
                    xmlprop("labelDisplay") = value.ToString()
                End If

            End If
        End Set
    End Property

    Public Property xAxisName() As String
        Get
            Return xmlprop("xAxisName")
        End Get
        Set(ByVal value As String)
            xmlprop("xAxisName") = value
        End Set
    End Property

    Public Property yAxisName() As String
        Get
            Return xmlprop("yAxisName")
        End Get
        Set(ByVal value As String)
            xmlprop("yAxisName") = value
        End Set
    End Property

    Public Property showValues() As Boolean
        Get
            Return xmlBoolProp("showValues")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("showValues") = value
        End Set
    End Property

    Public Property rotateValues() As Boolean
        Get
            Return xmlBoolProp("rotateValues")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("rotateValues") = value
        End Set
    End Property

    Public Property roundEdges() As Boolean
        Get
            Return xmlBoolProp("useRoundEdges")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("useRoundEdges") = value
        End Set
    End Property

    Public Property animation() As Boolean
        Get
            Return xmlBoolProp("animation")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("animation") = value
        End Set
    End Property

    Public Property showLegend() As BooleanDefault
        Get
            Return props.xmlBoolDefault("showLegend")
        End Get
        Set(ByVal value As BooleanDefault)
            props.xmlBoolDefault("showLegend") = value
        End Set
    End Property


    Public Property showPlotBorder() As BooleanDefault
        Get
            Return props.xmlBoolDefault("showPlotBorder")
        End Get
        Set(ByVal value As BooleanDefault)
            props.xmlBoolDefault("showPlotBorder") = value
        End Set
    End Property

    Public Property showBorder() As Boolean
        Get
            Return xmlBoolProp("showBorder")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("showBorder") = value
        End Set
    End Property

    Public Property showGradient() As ChartBase.BooleanDefault
        Get
            Return props.xmlBoolDefault("useplotgradientcolor")
        End Get
        Set(ByVal value As ChartBase.BooleanDefault)
            'If value = False Then
            '    xmlBoolProp("plotGradientColor") = " "
            'Else
            '    props.remove("plotGradientColor")
            'End If
            props.xmlBoolDefault("useplotgradientcolor") = value
        End Set
    End Property

    Public Property showShadow() As ChartBase.BooleanDefault
        Get
            Return props.xmlBoolDefault("showShadow")
        End Get
        Set(ByVal value As ChartBase.BooleanDefault)
            If value = False Then
                xmlBoolProp("use3DLighting") = False
            Else
                props.remove("use3DLighting")
            End If
            props.xmlBoolDefault("showShadow") = value
        End Set
    End Property

    Public Property slantLabels() As Boolean
        Get
            Return xmlBoolProp("slantLabels")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("slantLabels") = value
        End Set
    End Property

    Public Property numberSuffix() As String
        Get
            Return xmlprop("numberSuffix")
        End Get
        Set(ByVal value As String)
            xmlprop("numberSuffix") = value
        End Set
    End Property

    Public Property numberPrefix() As String
        Get
            Return xmlprop("numberPrefix")
        End Get
        Set(ByVal value As String)
            xmlprop("numberPrefix") = value
        End Set
    End Property

    Private _additionalSeriesXML As New Generic.List(Of SeriesXML)
    Public Property seriesList() As Generic.List(Of SeriesXML)
        Get
            While _additionalSeriesXML.Count < Me.numberOfSeries
                _additionalSeriesXML.Add(New SeriesXML(Me))
            End While
            Return _additionalSeriesXML
        End Get
        Set(ByVal value As Generic.List(Of SeriesXML))
            _additionalSeriesXML = value
        End Set
    End Property

    Public Property series(ByVal i As Integer) As SeriesXML
        Get
            While _additionalSeriesXML.Count < i
                _additionalSeriesXML.Add(New SeriesXML(Me))
            End While
            Return seriesList(i)
        End Get
        Set(ByVal value As SeriesXML)
            While _additionalSeriesXML.Count < i
                _additionalSeriesXML.Add(New SeriesXML(Me))
            End While
            _additionalSeriesXML(i) = value
        End Set
    End Property



#End Region

#Region "Other Proerties"

    Private dt As DataTable
    Public Property data() As DataTable
        Get
            Return dt
        End Get
        Set(ByVal value As DataTable)
            dt = value
        End Set
    End Property

    Private _labelCol As String = Nothing
    Public Property labelCol() As String
        Get
            Return _labelCol
        End Get
        Set(ByVal value As String)
            _labelCol = value
        End Set
    End Property

    Private _drillable As Boolean = False
    Public Property drillable() As Boolean
        Get
            Return _drillable
        End Get
        Set(ByVal value As Boolean)
            _drillable = value
        End Set
    End Property

    Private SerieseNamesValue As String = Nothing
    Public Property SeriesNames() As String
        Get
            Return SerieseNamesValue
        End Get
        Set(ByVal value As String)
            SerieseNamesValue = value
        End Set
    End Property

    'Private _chart As Chart
    'Public Property chart() As Chart
    '    Get
    '        Return _chart
    '    End Get
    '    Set(ByVal value As Chart)
    '        _chart = value
    '    End Set
    'End Property

    Private _valueCols() As String = Nothing
    Public Property valueCols() As String()
        Get
            Return _valueCols
        End Get
        Set(ByVal value As String())
            _valueCols = value
        End Set
    End Property

    Public Property valueColsStr() As String
        Get
			Dim out As String = ""
			If valueCols Is Nothing Then valueCols = {}
			For Each str As String In valueCols
                out &= str & ","
            Next
            Return out.Trim(",")
        End Get
        Set(ByVal value As String)
            valueCols = value.Trim(",").Split(",")
        End Set
    End Property

    Private _displayedTextCol() As String = Nothing
    Public Property displayedTextCol() As String()
        Get
            Return _displayedTextCol
        End Get
        Set(ByVal value As String())
            _displayedTextCol = value
        End Set
    End Property

    Public Property displayedTextColStr() As String
        Get
            Dim out As String = ""
            If displayedTextCol Is Nothing Then Return out
            For Each str As String In displayedTextCol
                out &= str & ","
            Next
            Return out.Trim(",")
        End Get
        Set(ByVal value As String)
            If value.Trim = "" Then
                displayedTextCol = valueCols
            Else
                displayedTextCol = value.Trim(",").Split(",")
            End If
        End Set
    End Property

    Private _numberOfSerise As Integer = 2
    Public Property numberOfSeries() As Integer
        Get
            If _valueCols Is Nothing OrElse _valueCols.Length = 0 Then Return _numberOfSerise
            Return _valueCols.Length
        End Get
        Set(ByVal value As Integer)
            _numberOfSerise = value
        End Set
    End Property

    Private _addtionalXML As String = ""
    Public Property addtionalXML() As String
        Get
            Return _addtionalXML
        End Get
        Set(ByVal value As String)
            _addtionalXML = value
        End Set
    End Property

#End Region

    Protected WithEvents hidfield As New HtmlControls.HtmlInputHidden
    Protected WithEvents hidbtn As New HtmlControls.HtmlInputSubmit
    Private Sub Chart_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        hidbtn.Style.Item("Display") = "None"
        'hidbtn.ID = Me.ClientID & "_hbtn"
        hidbtn.Value = "true"
        'hidfield.ID = Me.ClientID & "_hfld"
        Me.Controls.Add(hidfield)
        Me.Controls.Add(hidbtn)
    End Sub

    Private Sub Chart_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Page.IsPostBack Then
            If Me.Page.Request.Params(hidbtn.UniqueID) IsNot Nothing Then
                Dim vals() As String = Me.Page.Request.Params(hidfield.UniqueID).Split(",")

                If data Is Nothing OrElse data.Rows.Count < vals(0) Then
                    RaiseEvent DrillDownEvent(Me, Nothing, vals(1), vals(2))
                Else
                    RaiseEvent DrillDownEvent(Me, Me.data.Rows(vals(0)), vals(1), vals(2))
                End If
            End If
        End If


    End Sub

    Public Overrides Function outputXML() As String
        Try
            If data Is Nothing Then
                Return testXml()
            End If
            If labelCol Is Nothing OrElse labelCol = "" Then
                labelCol = dt.Columns(1).ColumnName
            End If
            If _valueCols Is Nothing OrElse _valueCols.Length = 0 Then
                Dim serLen As Integer = numberOfSeries
                Dim colList As String = ""
                For j As Integer = 1 To serLen
                    Dim i As Integer = j
                    If i = 1 Then i = 0
                    If dt.Columns.Count > i Then
                        colList &= dt.Columns(i).ColumnName & ","
                    End If
                Next
                valueColsStr = colList
            End If
            If displayedTextCol IsNot Nothing AndAlso displayedTextCol.Length < valueCols.Length Then
                Dim cols As String() = valueCols.Clone
                Dim i As Integer = 0
                For Each displayName As String In displayedTextCol
                    If Not displayName.Trim = "" Then _
                        cols(i) = displayName
                    i += 1
                Next
                displayedTextCol = cols
            End If
            If displayedTextCol Is Nothing OrElse displayedTextCol.Length < valueCols.Length Then
                displayedTextCol = valueCols
            End If
			Dim out As String = "<graph " & props.xmlpropString & " " & addtionalChartProperties & " >"
			If numberOfSeries > 1 And Not singleset Then
                out &= getCatagoriesString(labelCol)
                For i As Integer = 0 To valueCols.Length - 1
                    out &= series(i).getMultiSerieseString(valueCols(i), displayedTextCol(i))
                Next
            Else
                out &= series(0).getSingleSerieseString(labelCol, valueCols(0))
            End If
			out &= addtionalXML & "</graph>"
			Return out
        Catch ex As Exception

        End Try
		Return "<graph " & props.xmlpropString & " ></graph>"
	End Function

    Public Function getValueStr(ByVal row As DataRow, ByVal colname As String) As String
        Dim out As String = " value='" & row(colname) & "'"
        Dim X As String = "|,|"
        If drillable Then
            out &= " link=""JavaScript:doDrill('" & hidbtn.ClientID & X & hidfield.ClientID & X & dt.Rows.IndexOf(row) & X & colname & X & row(colname) & "')"" "
        End If
        Return out
    End Function


    Private Function getCatagoriesString(ByVal colname As String) As String
        Dim out As String = "<categories>"
        For Each row As DataRow In dt.Rows
			out &= "<category name='" & row(colname) & "' />"
		Next
        out &= "</categories>"
        Return out
    End Function

    Private Function testXml() As String
		Return "<graph caption='Monthly Sales Summary' subcaption='For the year 2006' xAxisName='Month' yAxisName='Sales' numberPrefix='$'><set label='January' value='17400' /><set label='February' value='19800' /><set label='March' value='21800' /><set label='April' value='23800' /><set label='May' value='29600' /><set label='June' value='27600' /><set label='July' value='31800' /><set label='August' value='39700' /><set label='September' value='37800' /><set label='October' value='21900' /><set label='November' value='32900' /><set label='December' value='39800' /></graph>"
	End Function

    Public Class SeriesXML
        Private chart As Chart

#Region "Properties"

        Private _color As System.Drawing.Color = Nothing
        Public Property color() As System.Drawing.Color
            Get
                Return _color
            End Get
            Set(ByVal value As System.Drawing.Color)
                _color = value
            End Set
        End Property

        Private _name As String = Nothing
        Public Property name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Private _ShowValues As Boolean
        Private isShowValuesSet = False
        Public Property ShowValues() As Boolean
            Get
                Return _ShowValues
            End Get
            Set(ByVal value As Boolean)
                isShowValuesSet = True
                _ShowValues = value
            End Set
        End Property

        Private _alpha As Integer = -1
        Public Property alpha() As Integer
            Get
                Return _alpha
            End Get
            Set(ByVal value As Integer)
                _alpha = value
            End Set
        End Property

        'Private _font As String = Nothing
        'Public Property font() As String
        '    Get
        '        Return _font
        '    End Get
        '    Set(ByVal value As String)
        '        _font = value
        '    End Set
        'End Property

        'Private _fontcolor As System.Drawing.Color = Nothing
        'Public Property fontcolor() As System.Drawing.Color
        '    Get
        '        Return _fontcolor
        '    End Get
        '    Set(ByVal value As System.Drawing.Color)
        '        _fontcolor = value
        '    End Set
        'End Property

        'Private _fontsize As Double = -1
        'Public Property fontsize() As Double
        '    Get
        '        Return _fontsize
        '    End Get
        '    Set(ByVal value As Double)
        '        _fontsize = value
        '    End Set
        'End Property

#End Region

        Private Function getAdditional() As String
            Dim additonal As String = ""
            If Not color = Nothing Then
                additonal &= "color='" & String.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B) & "' "
            End If
            If alpha > 0 Then additonal &= "alpha='" & alpha & "' "
            If isShowValuesSet Then
                If ShowValues Then additonal &= "showValues='1' " Else additonal &= "showValues='0' "
            End If

            Return additonal
        End Function

        Public Function getMultiSerieseString(ByVal colname As String, Optional ByRef newName As String = Nothing) As String
            If Not newName Is Nothing Then name = newName
            Dim additonal As String = getAdditional()
            If name Is Nothing Then name = colname
            additonal &= "seriesName='" & name & "' "
            Dim out As String = "<dataset " & additonal & ">"
            For Each row As DataRow In chart.dt.Rows
                out &= "<set " & chart.getValueStr(row, colname) & " />"
            Next
            out &= "</dataset>"
            Return out
        End Function

        Public Function getSingleSerieseString(ByVal labelCol As String, ByVal valueCol As String) As String
            Dim out As String = ""
            Dim additonal As String = getAdditional()
            For Each row As DataRow In chart.dt.Rows
				out &= "<set " & additonal & " name='" & row(labelCol).ToString.Replace("&", "&amp;").Replace("'", "&#39;") & "' " & chart.getValueStr(row, valueCol) & " />"
			Next
            Return out
        End Function

        Public Sub New(ByVal mychart As Chart)
            Me.chart = mychart
        End Sub
    End Class

    'End Class


    Public Sub New()
        animation = True
        showValues = True
        slantLabels = False
        showGradient = True
        showPlotBorder = True
        showBorder = True
        showShadow = True
        chartType = chartTypeEnum.Column2D
    End Sub

    'Private Sub Chart_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
    '    If SeriesNames IsNot Nothing AndAlso Not SeriesNames.Trim = "" Then
    '        Dim i As Integer = 0
    '        'displayedTextCol = SeriesNames.Split(",")
    '        For Each name As String In
    '            displayedTextColStr = name
    '        Next

    '    End If
    'End Sub
End Class


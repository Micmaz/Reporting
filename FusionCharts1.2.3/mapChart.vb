Imports System.Web.UI.WebControls
Imports System.Web.UI

Public Class MapChart
    Inherits ChartBase

    Public Event DrillDownEvent(ByVal row As DataRow, ByVal columnName As String, ByVal columnValue As String)


    Private Sub hidbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles hidbtn.Click
        Dim vals() As String = Me.hidfield.Value.Split(",")
        RaiseEvent DrillDownEvent(Me.data.Rows(vals(0)), vals(1), vals(2))
    End Sub

    Private Function getXml() As String
        Return outputXML()
    End Function

    Public Overrides Function getSwfFile() As String
        Return SwfFileNames(chartType)
    End Function

    Private Function getContent() As String
        Dim chartfile As String = getSwfFile()
        Dim swf As String = Page.ClientScript.GetWebResourceUrl(Me.GetType(), "FusionCharts." & chartfile)
        Dim swfid As String = Me.ClientID & "swf"
        Dim xml As String
        Dim i As Integer = New Random().Next()
        Dim var As String = "dynchart" & i
        If dataUrl Is Nothing Then
            xml = var & ".setDataXML(""" & JavaScriptEncode(getXml()) & """);"
        Else
            xml = var & ".setDataURL(""" & Me.dataUrl & """);"
        End If
        Return "   <script type=""text/javascript""> " & vbCrLf & _
"     var " & var & " = new FusionCharts(""" & swf & """, """ & swfid & """, """ & Me.Width.Value & """, """ & Me.Height.Value & """, ""0"", ""1"");" & vbCrLf & _
xml & vbCrLf & _
var & ".addParam(""wmode"", ""opaque"");" & vbCrLf & _
var & ".render(""" & innerpnl.ClientID & """);" & vbCrLf & _
"   </script> " & vbCrLf
    End Function

#Region "Filenames and enum"
    Private SwfFileNames() As String = New String() {"FCMap_USA.swf"}

    Public Enum mapTypeEnum
        USA_States = 0
    End Enum

    Private _chartType As Integer = 0
    Public Property chartType() As mapTypeEnum
        Get
            Return _chartType
        End Get
        Set(ByVal value As mapTypeEnum)
            _chartType = value
        End Set
    End Property

#End Region

#Region "Layout properties"

    Public Property chartTitle() As String
        Get
            Return xmlprop("caption")
        End Get
        Set(ByVal value As String)
            xmlprop("caption") = value
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

    Public Property slantLabels() As Boolean
        Get
            Return xmlBoolProp("slantLabels")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("slantLabels") = value
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

    Private _chart As Chart
    Public Property chart() As Chart
        Get
            Return _chart
        End Get
        Set(ByVal value As Chart)
            _chart = value
        End Set
    End Property

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
            For Each str As String In displayedTextCol
                out &= str & ","
            Next
            Return out.Trim(",")
        End Get
        Set(ByVal value As String)
            displayedTextCol = value.Trim(",").Split(",")
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


    Protected WithEvents hidfield As New HiddenField
    Protected WithEvents hidbtn As New Button
    Friend hiddenbuttonId As String
    Friend hiddenfieldID As String
    Private Sub Chart_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        hidbtn.Style.Item("Display") = "None"
        hidbtn.ID = Me.ID & "hbtn"
        hidfield.ID = Me.ID & "hfld"
        Me.Controls.Add(hidfield)
        Me.Controls.Add(hidbtn)
        hiddenbuttonId = hidbtn.ClientID
        hiddenfieldID = hidfield.ClientID
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
            If displayedTextCol Is Nothing OrElse displayedTextCol.Length < valueCols.Length Then
                displayedTextCol = valueCols
            End If
            Dim out As String = "<chart " & props.xmlpropString & " >"
            If numberOfSeries > 1 And Not singleset Then
                out &= getCatagoriesString(labelCol)
                For i As Integer = 0 To valueCols.Length - 1
                    out &= series(i).getMultiSerieseString(valueCols(i), displayedTextCol(i))
                Next
            Else
                out &= series(0).getSingleSerieseString(labelCol, valueCols(0))
            End If
            out &= addtionalXML & "</chart>"
            Return out
        Catch ex As Exception

        End Try
        Return "<chart " & props.xmlpropString & " ></chart>"
    End Function


    Public Function getValueStr(ByVal row As DataRow, ByVal colname As String) As String
        Dim out As String = " value='" & row(colname) & "'"
        Dim X As String = "|,|"
        If drillable Then
            out &= " link=""JavaScript:doDrill('" & hiddenbuttonId & X & hiddenfieldID & X & dt.Rows.IndexOf(row) & X & colname & X & row(colname) & "')"" "
        End If
        Return out
    End Function


    Private Function getCatagoriesString(ByVal colname As String) As String
        Dim out As String = "<categories>"
        For Each row As DataRow In dt.Rows
            out &= "<category label='" & row(colname) & "' />"
        Next
        out &= "</categories>"
        Return out
    End Function

    Private Function testXml() As String
        Return "<chart caption='Monthly Sales Summary' subcaption='For the year 2006' xAxisName='Month' yAxisName='Sales' numberPrefix='$'><set label='January' value='17400' /><set label='February' value='19800' /><set label='March' value='21800' /><set label='April' value='23800' /><set label='May' value='29600' /><set label='June' value='27600' /><set label='July' value='31800' /><set label='August' value='39700' /><set label='September' value='37800' /><set label='October' value='21900' /><set label='November' value='32900' /><set label='December' value='39800' /></chart>"
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
                out &= "<set " & additonal & " label='" & row(labelCol) & "' " & chart.getValueStr(row, valueCol) & " />"
            Next
            Return out
        End Function

        Public Sub New(ByVal mychart As Chart)
            Me.chart = mychart
        End Sub
    End Class

    'End Class


End Class


Imports System.Web.UI.WebControls
Imports System.Web.UI

Public Class DragChart
    Inherits ChartBase
    Friend WithEvents hidden As New HiddenField

#Region "XMLProperties"

    Public Property viewMode() As Boolean
        Get
            Return xmlBoolProp("viewMode")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("viewMode") = value
        End Set
    End Property

    Public Property enableLink() As Boolean
        Get
            Return xmlBoolProp("enableLink")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("enableLink") = value
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

    Public Property palette() As palettEnum
        Get
            Return xmlprop("palette")
        End Get
        Set(ByVal value As palettEnum)
            If value = palettEnum.Not_Set Then props.remove("palette")
            xmlprop("palette") = value
        End Set
    End Property

    Public Property paletteColorsList() As String
        Get
            Return xmlprop("paletteColors")
        End Get
        Set(ByVal value As String)
            xmlprop("paletteColors") = value
        End Set
    End Property

    Public Property showLabels() As Boolean
        Get
            Return xmlBoolProp("showLabels")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("showLabels") = value
        End Set
    End Property

    Public Property rotateLabels() As Boolean
        Get
            Return xmlBoolProp("rotateLabels")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("rotateLabels") = value
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

    Public Property showValues() As Boolean
        Get
            Return xmlBoolProp("showValues")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("showValues") = value
        End Set
    End Property

    Public Property showFormBtn() As Boolean
        Get
            Return xmlBoolProp("showFormBtn")
        End Get
        Set(ByVal value As Boolean)
            xmlBoolProp("showFormBtn") = value
        End Set
    End Property


#End Region

    'Public Overrides Function getSwfFile() As String
    '    Return "DragNode.swf"
    'End Function

    Public Overrides Function outputXML() As String
        Try
            Dim out As String = "<chart viewMode='1'  showBorder='0' xAxisMinValue='0' showformbtn='0' xAxisMaxValue='100' yAxisMinValue='0' yAxisMaxValue='100' is3D='1' " & props.xmlpropString & " >"
            out &= "<dataset showPlotBorder='0' plotBorderAlpha='0' allowDrag='0' showformbtn='0'>"
            Dim i As Integer = 0
            For Each itm As DragChartItem In dragItms
                If itm.Id = -1 Then itm.Id = i
                i = itm.Id
                out &= itm.props.outputsingleTagXML
                i += 1
            Next
            out &= "</dataset>"
            out &= "<connectors>"
            For Each itm As DragChartConnection In dragConnections
                out &= itm.props.outputsingleTagXML
            Next
            out &= "</connectors></chart>"
            Return out
        Catch ex As Exception

        End Try
        Return "<chart " & props.xmlpropString & " ></chart>"
    End Function

#Region "dataclass"

    Public Class DragChartItem
        Inherits xmlDataObject

        Public Property Id() As Integer
            Get
                Return xmlprop("Id")
            End Get
            Set(ByVal value As Integer)
                xmlprop("Id") = value
            End Set
        End Property

        Public Property allowDrag() As Boolean
            Get
                Return xmlBoolProp("allowDrag")
            End Get
            Set(ByVal value As Boolean)
                xmlBoolProp("allowDrag") = value
            End Set
        End Property

        Public Property width() As Integer
            Get
                Return xmlprop("width")
            End Get
            Set(ByVal value As Integer)
                xmlprop("width") = value
            End Set
        End Property

        Public Property height() As Integer
            Get
                Return xmlprop("height")
            End Get
            Set(ByVal value As Integer)
                xmlprop("height") = value
            End Set
        End Property

        Public Property radius() As Integer
            Get
                Return xmlprop("radius")
            End Get
            Set(ByVal value As Integer)
                xmlprop("radius") = value
            End Set
        End Property

        Public Property numSides() As Integer
            Get
                Return xmlprop("numSides")
            End Get
            Set(ByVal value As Integer)
                xmlprop("numSides") = value
            End Set
        End Property

        Public Property imageURL() As String
            Get
                Return xmlprop("imageURL")
            End Get
            Set(ByVal value As String)
                If Not value Is Nothing Then
                    xmlBoolProp("imageNode") = True
                    alpha = 0
                    'xmlprop("imageWidth") = width
                    'xmlprop("imageHeight") = height
                Else
                    xmlBoolProp("imageNode") = False
                End If
                xmlprop("imageURL") = value
            End Set
        End Property

        Public Property x() As Integer
            Get
                Return xmlprop("x")
            End Get
            Set(ByVal value As Integer)
                If value > 100 Then value = 100
                If value < 0 Then value = 0
                xmlprop("x") = value
            End Set
        End Property

        Public Property y() As Integer
            Get
                Return xmlprop("y")
            End Get
            Set(ByVal value As Integer)
                If value > 100 Then value = 100
                If value < 0 Then value = 0
                xmlprop("y") = value
            End Set
        End Property

        Public Property color() As System.Drawing.Color
            Get
                Return ChartBase.parseColor(xmlprop("color"))
            End Get
            Set(ByVal value As System.Drawing.Color)
                xmlprop("color") = ChartBase.getColorHex(value)
            End Set
        End Property

        Public Property alpha() As Integer
            Get
                Return xmlprop("alpha")
            End Get
            Set(ByVal value As Integer)
                If value > 100 Then value = 100
                If value < 0 Then value = 0
                xmlprop("alpha") = value
            End Set
        End Property

        Public Property toolText() As String
            Get
                Return xmlprop("toolText")
            End Get
            Set(ByVal value As String)
                xmlprop("toolText") = value
            End Set
        End Property

        Public Property link() As String
            Get
                Return xmlprop("link")
            End Get
            Set(ByVal value As String)
                xmlprop("link") = value
            End Set
        End Property

        Public Property name() As String
            Get
                Return xmlprop("name")
            End Get
            Set(ByVal value As String)
                xmlprop("name") = value
            End Set
        End Property

        Public Sub New(ByVal name As String, ByVal x As Integer, ByVal y As Integer, Optional ByVal height As Integer = 100, Optional ByVal width As Integer = 100, Optional ByVal allowdrag As Boolean = True, Optional ByVal imgurl As String = Nothing, Optional ByVal id As Integer = -1)
            Me.tag = "set"
            Me.name = name
            Me.x = x
            Me.y = y
            Me.height = height
            Me.width = width
            Me.allowDrag = allowdrag
            Me.imageURL = imgurl
            Me.Id = id
        End Sub

    End Class

    Public Class DragChartConnection
        Inherits xmlDataObject

        Public Property alpha() As Integer
            Get
                Return xmlprop("alpha")
            End Get
            Set(ByVal value As Integer)
                If value > 100 Then value = 100
                If value < 0 Then value = 0
                xmlprop("alpha") = value
            End Set
        End Property

        Public Property weight() As Double
            Get
                Return xmlprop("strength")
            End Get
            Set(ByVal value As Double)
                If value > 2 Then value = 2
                If value < 0 Then value = 0
                xmlprop("strength") = value
            End Set
        End Property

        Public Property label() As String
            Get
                Return xmlprop("label")
            End Get
            Set(ByVal value As String)
                xmlprop("label") = value
            End Set
        End Property

        Public Property toId() As Integer
            Get
                Return xmlprop("to")
            End Get
            Set(ByVal value As Integer)
                xmlprop("to") = value
            End Set
        End Property

        Public Property fromId() As Integer
            Get
                Return xmlprop("from")
            End Get
            Set(ByVal value As Integer)
                xmlprop("from") = value
            End Set
        End Property

        Public Property arrowAtStart() As Boolean
            Get
                Return xmlBoolProp("arrowAtStart")
            End Get
            Set(ByVal value As Boolean)
                xmlBoolProp("arrowAtStart") = value
            End Set
        End Property

        Public Property arrowAtEnd() As Boolean
            Get
                Return xmlBoolProp("arrowAtEnd")
            End Get
            Set(ByVal value As Boolean)
                xmlBoolProp("arrowAtEnd") = value
            End Set
        End Property

        Public Property dashed() As Boolean
            Get
                Return xmlBoolProp("dashed")
            End Get
            Set(ByVal value As Boolean)
                xmlBoolProp("dashed") = value
            End Set
        End Property

        Public Property color() As System.Drawing.Color
            Get
                Return ChartBase.parseColor(xmlprop("color"))
            End Get
            Set(ByVal value As System.Drawing.Color)
                xmlprop("color") = ChartBase.getColorHex(value)
            End Set
        End Property

        Public Sub New(ByVal fromId As Integer, ByVal toID As Integer)
            Me.New(fromId, toID, Drawing.Color.Gray)
        End Sub

        Public Sub New(ByVal fromId As Integer, ByVal toID As Integer, ByVal color As System.Drawing.Color, Optional ByVal label As String = Nothing, Optional ByVal weight As Double = 1, Optional ByVal arrowsAtStart As Boolean = False, Optional ByVal arrowsatEnd As Boolean = False, Optional ByVal dashed As Boolean = False)
            Me.tag = "connector"
            Me.fromId = fromId
            Me.toId = toID
            Me.label = label
            Me.color = color
            Me.weight = weight
            Me.arrowAtStart = arrowsAtStart
            Me.arrowAtEnd = arrowsatEnd
            Me.dashed = dashed
        End Sub

    End Class

    Private _dragConnections As New List(Of DragChartConnection)
    Public Property dragConnections() As List(Of DragChartConnection)
        Get
            Return _dragConnections
        End Get
        Set(ByVal value As List(Of DragChartConnection))
            _dragConnections = value
        End Set
    End Property

    Private _dragItms As New List(Of DragChartItem)
    Public Property dragItms() As List(Of DragChartItem)
        Get
            Return _dragItms
        End Get
        Set(ByVal value As List(Of DragChartItem))
            _dragItms = value
        End Set
    End Property



#End Region

End Class

Imports System.Web.UI.WebControls
Imports System.Web.UI

Public MustInherit Class ChartBase
    Inherits Panel

    Protected innerpnl As New Panel
    Protected content As New LiteralControl
	'Public MustOverride Function getSwfFile() As String
	Public MustOverride Function outputXML() As String


    Private _chartType As Integer = 0
    Public Property chartType() As Chart.chartTypeEnum
        Get
            Return _chartType
        End Get
        Set(ByVal value As Chart.chartTypeEnum)
            _chartType = value
        End Set
    End Property

    Private _dataurl As String = Nothing
    Public Property dataUrl() As String
        Get
            Return _dataurl
        End Get
        Set(ByVal value As String)
            _dataurl = value
        End Set
    End Property

    Private _debugmode As Boolean = False
    Public Property debugMode() As Boolean
        Get
            Return _debugmode
        End Get
        Set(ByVal value As Boolean)
            _debugmode = value
        End Set
    End Property


    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        innerpnl.Width = Me.Width
        innerpnl.Height = Me.Height
        innerpnl.ID = "innerpnl" & Me.ID
        content.Text = getContent()
        MyBase.Render(writer)
    End Sub

    Private Function getContent() As String

		Dim swf As String = BaseClasses.Scripts.ScriptsURL(True) & "/FusionCharts/FCF_" & chartType.ToString() & ".swf"
		swf = swf.Trim("/")
		Dim swfid As String = Me.ClientID & "swf"
        Dim xml As String
        Dim i As Integer = New Random().Next()
        Dim var As String = "dynchart" & i
        If dataUrl Is Nothing Then
			xml = var & ".setDataXML(""" & JavaScriptEncode(outputXML()) & """);"
		Else
            xml = var & ".setDataURL(""" & Me.dataUrl & """);"
        End If
		Return "   <script type=""text/javascript""> " & vbCrLf &
"     var " & var & " = new FusionCharts(""" & swf & """, """ & swfid & """, """ & Me.Width.Value & """, """ & Me.Height.Value & """, ""0"", ""1"");" & vbCrLf &
xml & vbCrLf &
var & ".addParam(""wmode"", ""opaque"");" & vbCrLf &
var & ".render(""" & innerpnl.ClientID & """);" & vbCrLf &
"   </script> " & vbCrLf
		'Return "   <script type=""text/javascript""> var " & var & ";" & vbCrLf &
		'		"     $(function(){ " & var & " = new FusionCharts(""" & chartType.ToString() & """, """ & swfid & """, """ & Me.Width.Value & """, """ & Me.Height.Value & """, ""0"", ""1"");" & vbCrLf &
		'		xml & vbCrLf &
		'		var & ".render(""" & innerpnl.ClientID & """);" & vbCrLf &
		'		"  }); </script> " & vbCrLf

	End Function

    Public Shared Function JavaScriptEncode(ByVal Str As String) As String

        Str = Replace(Str, "\", "\\")
        Str = Replace(Str, "'", "\'")
        Str = Replace(Str, """", "\""")
        Str = Replace(Str, Chr(8), "\b")
        Str = Replace(Str, Chr(9), "\t")
        Str = Replace(Str, Chr(10), "\r")
        Str = Replace(Str, Chr(12), "\f")
        Str = Replace(Str, Chr(13), "\n")

        Return Str

    End Function

    Private Sub Chart_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Dim scriptLocation As String = Page.ClientScript.GetWebResourceUrl(Me.GetType(), "FusionCharts.FusionCharts.js")
        'scriptLocation = scriptLocation.Trim("/")
        'Page.ClientScript.RegisterClientScriptInclude("FusionCharts.FusionCharts.js", scriptLocation)

        registerControl(Me.Page)

        innerpnl.Controls.Add(content)
        Me.Controls.Add(innerpnl)

    End Sub

    ''' <summary>
    ''' Registers all necessary javascript and css files for this control to function on the page.
    ''' </summary>
    ''' <param name="page"></param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Registers all necessary javascript and css files for this control to function on the page.")> _
    Public Shared Sub registerControl(ByVal page As Page)
        If Not page Is Nothing Then
            jQueryLibrary.jQueryInclude.addScriptFile(page, "/FusionCharts/FusionCharts.js")
			'jQueryLibrary.jQueryInclude.addScriptFile(page, "/FusionCharts/themes.js")
		End If
    End Sub

    Public Function removeValue(valueName As String) As Boolean
        If props.containsKey(valueName) Then
            props.remove(valueName)
            Return True
        End If
        Return False
    End Function

    Protected props As New PropertyHash()
    Public Property xmlprop(ByVal propname As String) As String
        Get
            Return props.xmlprop(propname)
        End Get
        Set(ByVal value As String)
            props.xmlprop(propname) = value
        End Set
    End Property

    Public Property xmlBoolProp(ByVal propname As String) As Boolean
        Get
            Return props.xmlBoolProp(propname)
        End Get
        Set(ByVal value As Boolean)
            props.xmlBoolProp(propname) = value
        End Set
    End Property

    Public Enum palettEnum
        Not_Set = 0
        Green = 1
        Grey = 2
        Brown = 3
        Blue = 4
        Red = 5
    End Enum

    Public Enum labelDisplayEnum
        Not_Set = 0
        WRAP
        STAGGER
        ROTATE
        NONE
        SLANT
    End Enum

    Public Enum ChartTheme
        None
        Flint
        Fire
        Carbon
        Ocean
        Zune
    End Enum

    Public Enum BooleanDefault
        [No]
        [Yes]
        [Default]
    End Enum

    Public Shared Function parseColor(ByVal hexstring As String) As System.Drawing.Color
        Try
            Dim r As Integer = Integer.Parse(hexstring.Substring(0, 2), Globalization.NumberStyles.HexNumber)
            Dim g As Integer = Integer.Parse(hexstring.Substring(2, 2), Globalization.NumberStyles.HexNumber)
            Dim b As Integer = Integer.Parse(hexstring.Substring(4, 2), Globalization.NumberStyles.HexNumber)
            Return System.Drawing.Color.FromArgb(r, g, b)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function getColorHex(ByVal color As System.Drawing.Color) As String
        If color = Nothing Then Return Nothing
        Try
            Return String.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class


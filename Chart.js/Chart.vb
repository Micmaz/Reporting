Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Drawing
Imports Reporting.Report

Public Class Chart
	Inherits Panel

	Public Event DrillDownEvent(ByVal sender As Chart, ByVal row As DataRow, ByVal columnName As String, ByVal columnValue As String)
	Protected content As New LiteralControl

	Public Enum colorSchemeType
		Monochromatic
		Contrast
		Triade
		Tetrade
		Analogic
		Analogic_Compliment
	End Enum

	Public Enum colorSchemeVariationType
		[Default]
		Pastel
		Soft
		Light
		Hard
		Pale
	End Enum



#Region "chart Prop Hash"

	Public Property addtionalChartProperties() As String = ""
	Public Property addtionalItemLevelProperties() As String = ""

	Public Property chartTitle As String = ""

	Public Property beginAtZero As Boolean = True
	Public Property fillLineChart As Boolean = False


	Public Property colorScheme As colorSchemeType = colorSchemeType.Analogic

	Public Property colorSchemeVariation As colorSchemeVariationType = Chart.colorSchemeVariationType.Default

	Public Property colorSchemeBaseColor As Color = Nothing

	Public Property colorSchemeRandomizeColorOrder As Boolean = False

	Public Property colorSchemedistance As Double = 0.5

	Public Function getProp(name As String) As String
		If props.ContainsKey(name) Then
			Return name & ":" & getPropVal(name) & ","
		End If
		Return ""
	End Function

	Public Function getPropVal(name As String) As String
		If props.ContainsKey(name) Then
			Dim val = props(name)
			Return getjsValue(val)
		End If
		Return "''"
	End Function

	'Public Function getjsValue(val As Object) As String
	'	If val Is Nothing Then
	'		val = ""
	'	End If
	'	Return "'" & val.ToString().Replace("'", "/'") & "'"
	'End Function

	Private Sub removeValue(v As String)
		props.Remove(v)
	End Sub

	Private props As New Hashtable()
	Public Property javascriptProp(v As String) As Object
		Get
			Return props(v)
		End Get
		Set(val As Object)
			props(v) = val
		End Set
	End Property

	Public Property chartType As chartTypeEnum = chartTypeEnum.bar

	Public Property cornerRadius As Integer = 0
	Public Property dropShadow As Boolean = True
	Public Property dropShadowColor As Color
	Public Property dropShadowBlur As Integer = 15
	Public Property dropShadowOffsetX As Integer = 5
	Public Property dropShadowOffsetY As Integer = 5
	Public Property gradient As Boolean = True
	Public Property gradientStart As Double = 1.5
	Public Property gradientEnd As Double = 0.5
	Public Property gradientStartShade As Double = 0
    Public Property gradientEndShade As Double = 0.8
    Public Property chartBorderColor As Color = Color.Black
    Public Property chartBorderWidth As Integer = 0

    Public ReadOnly Property chartTypeName As String
		Get
			Return [Enum].GetName(GetType(chartTypeEnum), chartType)
		End Get
	End Property

#End Region

#Region "Filenames and enum"

	Public Enum chartTypeEnum
		line
		bar
		horizontalBar
		radar
		polarArea
		pie
		doughnut
		bubble
		scatter
	End Enum

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

	Public Property drillable() As Boolean = True


	Public Property labelCol As String = Nothing


	Private Shared Function getValString(vals As String())
		Dim out As String = ""
		If vals Is Nothing Then vals = {}
		For Each str As String In vals
			out &= str & ","
		Next
		Return out.Trim(",")
	End Function
	Private Shared Function setValString(value As String) As String()
		Return value.Trim(",").Split(",")
	End Function

	Public Property defaultFontColor As Color

	Public Property defaultFontSize As Integer = 12
	Public Property defaultFontFamily As String = Nothing
	Public Property defaultFontStyle As String = Nothing

	Public Property valueCols() As String() = Nothing
	Public Property valueColsStr() As String
		Get
			Return getValString(valueCols)
		End Get
		Set(ByVal value As String)
			valueCols = setValString(value)
		End Set
	End Property

	Public Property datsetLabels() As String() = {}
	Public Property datsetLabelsStr() As String
		Get
			Return getValString(datsetLabels)
		End Get
		Set(ByVal value As String)
			datsetLabels = setValString(value)
		End Set
	End Property

	Private Property palette As String = ""

	Private _numberOfSerise As Integer = 1
	Public Property numberOfSeries() As Integer
		Get
			If _valueCols Is Nothing OrElse _valueCols.Length = 0 Then Return _numberOfSerise
			Return _valueCols.Length
		End Get
		Set(ByVal value As Integer)
			_numberOfSerise = value
		End Set
	End Property


#End Region

	Protected WithEvents hidfield As New HtmlControls.HtmlInputHidden
	Protected WithEvents hidbtn As New HtmlControls.HtmlInputSubmit
	Private Sub Chart_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
		jQueryLibrary.jQueryInclude.RegisterJQuery(Me.Page)
		jQueryLibrary.jQueryInclude.addScriptFile(Page, "/Chart.js/Chart.bundle.min.js")
		Me.Controls.Add(New LiteralControl("<canvas id='" & Me.ClientID & "cnv'></canvas>"))

		hidbtn.Style.Item("Display") = "None"
		'hidbtn.ID = Me.ClientID & "_hbtn"
		hidbtn.Value = "true"
		'hidfield.ID = Me.ClientID & "_hfld"
		Me.Controls.Add(hidfield)
		Me.Controls.Add(hidbtn)
		Me.Controls.Add(content)
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


	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
		content.Text = outputJavascript()
		MyBase.Render(writer)
	End Sub

	Public Function getGlobalString(val As Object, globalProp As String) As String
		val = getJSValue(val)
		If String.IsNullOrEmpty(val) Then
			Return ""
		End If
		'Dim retVal As String = ""
		'If TypeOf val Is Color Then
		'	retVal = getColorString(val)
		'End If
		Return "Chart.defaults.global." & globalProp & " = " & val & ";" & vbCrLf
	End Function

	Public Function outputJavascript() As String
		If data Is Nothing Then
			Return testData()
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
        'If displayedTextCol IsNot Nothing AndAlso displayedTextCol.Length < valueCols.Length Then
        '	Dim cols As String() = valueCols.Clone
        '	Dim i As Integer = 0
        '	For Each displayName As String In displayedTextCol
        '		If Not displayName.Trim = "" Then _
        '			cols(i) = displayName
        '		i += 1
        '	Next
        '	displayedTextCol = cols
        'End If
        'If displayedTextCol Is Nothing OrElse displayedTextCol.Length < valueCols.Length Then
        '		displayedTextCol = valueCols
        '	End If
        Dim out As String = "<script>
$(function(){
" &
getGlobalString(defaultFontColor, "defaultFontColor") &
getGlobalString(defaultFontFamily, "defaultFontFamily") &
getGlobalString(defaultFontSize, "defaultFontSize") &
getGlobalString(defaultFontStyle, "defaultFontStyle") &
"
var ctx = document.getElementById('" & ClientID & "cnv').getContext('2d');

ctx.cornerRadius = " & cornerRadius & ";"

        If dropShadow Then
            out &= "
ctx._dropShadow = " & getBoolString(dropShadow) & ";
ctx._shadowColor = '" & getColorString(dropShadowColor, "#000000") & "';
ctx._shadowBlur = " & dropShadowBlur & ";
ctx._shadowOffsetX = " & dropShadowOffsetX & ";
ctx._shadowOffsetY = " & dropShadowOffsetY & ";"
        End If
        out &= "
ctx.gradient = " & getBoolString(gradient) & ";
ctx.gradientStart = " & gradientStart & ";
ctx.gradientEnd = " & gradientEnd & ";
ctx.gradientStartShade = " & gradientStartShade & ";
ctx.gradientEndShade = " & gradientEndShade & ";

var " & ID & " = new Chart(ctx, {
    type: '" & chartTypeName & "',
    data: {
        labels: " & getValueArry(labelCol) & ",
		datasets: " & getDataSets()

        'If numberOfSeries > 1 Then
        '		out &= getCatagoriesString(labelCol)
        '		For i As Integer = 0 To valueCols.Length - 1
        '			out &= series(i).getMultiSerieseString(valueCols(i), displayedTextCol(i))
        '		Next
        '	Else
        '		out &= series(0).getSingleSerieseString(labelCol, valueCols(0))
        '	End If
        out &= "
	},
	options: {
		maintainAspectRatio: false"
		If Not (chartType = chartTypeEnum.pie OrElse chartType = chartTypeEnum.doughnut) Then
			out &= "
		,scales: {
            yAxes: [{
                ticks: {
                    beginAtZero:" & getBoolString(beginAtZero) & "
                }
            }]
        }"
		End If
		out &= "
        ,onClick:function(c,i){
			doDrill('" & hidbtn.ClientID & "','" & hidfield.ClientID & "',c,i,this);
		}
	}
});

});</script>"
		Return out
	End Function

	Private Function getDataSets() As String
		Dim out = "["
		For i As Integer = 0 To numberOfSeries - 1
			out &= getDataSet(i) & ","
		Next
		Return out.Trim(",") & "]"
	End Function

	Private Function getDataSet(i As Integer) As String
		Dim out = "{"
		Dim label = chartTitle
		If i > 0 Or String.IsNullOrEmpty(label) Then
			label = Reporting.Report.UppercaseWords(valueCols(i))
		End If
		Dim rowct As Integer = data.Rows.Count
		If datsetLabels.Length > i Then label = datsetLabels(i)
		Dim defColorStr As String = "false"
		If Not colorSchemeBaseColor = Color.Empty Then
			defColorStr = getJSValue(colorSchemeBaseColor).Replace("#", "")
		End If
		defColorStr = "," & defColorStr
        out &= "
 		    label: " & getJSValue(label, True) & ",
		    data: " & getValueArry(valueCols(i)) & "
      		,backgroundColor: getColors(" & dt.Rows.Count & defColorStr & ", '" & colorScheme.ToString().ToLower() & "', '" & colorSchemeVariation.ToString().ToLower() & "'," & colorSchemedistance & "," & getBoolString(Not colorSchemeRandomizeColorOrder) & ")
			,fill: " & getJSValue(fillLineChart)
        If Not chartBorderColor = Color.Empty AndAlso chartBorderWidth > 0 Then
            out &= "
        ,borderWidth: " & getJSValue(chartBorderWidth) & "
        ,borderColor: '" & getColorString(chartBorderColor, "#000000") & "'"
        End If
        out &= "
"
        Return out & "      	}"
	End Function


	Private Function testData() As String
		Return ""
	End Function

	'Public Function getValueStr(ByVal row As DataRow, ByVal colname As String) As String
	'	Dim out As String = getJSValue(row(colname))
	'	If String.IsNullOrEmpty(out) Then Return "''"
	'	Return out
	'End Function

	Public Function getValueArry(ByVal colname As String, Optional dt As DataTable = Nothing) As String
		Dim out As String = "["
		If dt Is Nothing Then dt = Me.data
		For Each r As DataRow In dt.Rows
			out &= getJSValue(r(colname), True) & ","
		Next
		Return out.Trim(",") & "]"
	End Function

	Public Class SeriesXML
		Private chart As Chart

#Region "Properties"


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


#End Region


		Public Sub New(ByVal mychart As Chart)
			Me.chart = mychart
		End Sub
	End Class

	'End Class

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


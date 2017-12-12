Imports System
Imports System.Reflection
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Web.UI
Imports System.Web
Imports System.Web.Caching
Imports System.Web.UI.WebControls
Imports System.Text.RegularExpressions
Imports System.Web.UI.Design

''' <remarks>
''' PropertyTableRow is an extension to the TableRow class that includes references
''' to key controls in the row: the Label in column 1, the BaseProperty subclass, and the error label.
''' In this sense, it is specialized to work with other classes here.
''' </remarks>
<System.ComponentModel.Description("")> _
Public Class PropertyTableRow
    Inherits TableRow
    Public fLabelControl As LiteralControl = Nothing
    ' field associated with the label;
    Public fPropertyControl As BaseProperty = Nothing
    ' field associated with a BaseProperty subclass
    Public fErrorControl As Label = Nothing
    ' field associated with the error label
    Public fThirdColControl As LiteralControl = Nothing
    ' field associated with the third column
    Public fpropControl As BaseProperty = Nothing

    Public fRemovedB As Boolean = False
    Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
        If Me.fPropertyControl Is Nothing Then
            Me.Visible = False
        Else

        End If
        MyBase.OnPreRender(e)
    End Sub

    Public Sub New(ByVal propControl As BaseProperty, ByVal typename As String)

        Me.Visible = propControl.Visible
        '                  vTableRow.EnableViewState = vIsObjectPropertyB; // only ObjectProperty uses ViewState. Handled automatically in TableRow

        ' fill in the tooltip with the property's DescriptionAttribute (assigned to BaseProperty.fHint)
        If propControl.fHint <> "" Then
            Me.ToolTip = propControl.fHint
        End If

        Dim cell1 As New TableCell()
        Dim padding As String = ""
        For i As Integer = 1 To propControl.fDepth
            padding &= "&nbsp;&nbsp;&nbsp;&nbsp;"
        Next
        cell1.Controls.Add(New LiteralControl(padding))
        cell1.Controls.AddAt(1, New LiteralControl("&nbsp;"))
        Me.fLabelControl = New LiteralControl(propControl.propertyPath)
        cell1.Controls.Add(Me.fLabelControl)
        'cell1.EnableViewState = False
        Me.Controls.Add(cell1)

        ' fill in the second column with vRowControl and optionally an error message label
        Dim cell2 As New TableCell()
        Me.fPropertyControl = propControl
        If TypeOf propControl Is BaseObjectProperty Then
            cell1.Controls.AddAt(1, propControl)
        Else
            cell1.Controls.AddAt(1, New LiteralControl("&nbsp;&nbsp;"))
            cell2.Controls.Add(propControl)
        End If

        ' data validation errors are shown in a label below (using a <BR> as part of the error text).
        ' The field initially is hidden. If an error msg is assigned, show it (adding the <BR>).
        ' Hiding avoids adding <SPAN> tags without any data to show inside them
        Me.fErrorControl = New Label()
        Me.fErrorControl.ForeColor = Color.Red
        Me.fErrorControl.Visible = False
        ' will be set to true when filled in later
        Me.fErrorControl.EnableViewState = False
        'cell2.Controls.Add(Me.fErrorControl)

        Me.Controls.Add(cell2)

        ' third column, identify the class type
        Dim cell3 As New TableCell()
        Me.fThirdColControl = New LiteralControl(typename)
        'Me.fThirdColControl = New LiteralControl(propControl.fPropertyInfo.PropertyType.Name)
        cell3.Controls.Add(Me.fThirdColControl)
        'cell3.EnableViewState = False
        Me.Controls.Add(cell3)
    End Sub


    '' when true, the row was added but should NEVER be made visible
    '' we have to keep a strict limit on use of ViewState. We enable ViewState on this
    '' control only when it contains an ObjectProperty which uses its own view state.
    '' Otherwise, each TableRow contributes a minimum of 64 bytes (in this implementation.)
    '' To make this work, we'll override LoadViewState and SaveViewState to avoid their ancestors work.
    'Public Overloads Overrides Property EnableViewState() As Boolean
    '    Get
    '        If fPropertyControl IsNot Nothing Then
    '            Return fPropertyControl.EnableViewState
    '        Else
    '            Return MyBase.EnableViewState
    '        End If
    '    End Get
    '    Set(ByVal value As Boolean)
    '        MyBase.EnableViewState = value
    '    End Set
    'End Property
    '' EnableViewState
    ''-----------------------------------------------------------------------------------------------
    'Protected Overloads Overrides Function SaveViewState() As Object
    '    Return Nothing
    'End Function
    '' SaveViewState()
    ''-----------------------------------------------------------------------------------------------
    'Protected Overloads Overrides Sub LoadViewState(ByVal savedState As Object)
    'End Sub
    '' LoadViewState()

End Class

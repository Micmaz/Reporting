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
Imports DTIMiniControls

Public Class ObjectProperty
    Inherits BaseObjectProperty

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer, ByVal pParentObjectProperty As BaseObjectProperty)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth, pParentObjectProperty)
    End Sub

    Public Overrides Function getProposedValue() As Object
        Return Nothing
    End Function

End Class

Public MustInherit Class BaseTextBoxProperty
    Inherits BaseProperty
    Protected fTextControl As TextBoxEncoded = Nothing
    Protected fOriginalValueNullB As Boolean = False

    Public Overrides Function getProposedValue() As Object
        if fTextControl is nothing then return nothing
        Return fTextControl.Text
    End Function

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        ' most controls simply use the value.ToString() to get the data.
        ' Override this method only when that is not the case.
        Dim vValue As Object = fPropertyInfo.GetValue(pInstance, Nothing)
        If vValue IsNot Nothing Then
            fTextControl.Text = vValue.ToString()
        Else
            fOriginalValueNullB = True
        End If
    End Sub

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        ' subclasses must handle
    End Sub

    Protected Overloads Overrides Sub CreateChildControls()
        fTextControl = New TextBoxEncoded()
        fTextControl.EnableViewState = False
        ToolTipToStatus(fTextControl)
        fTextControl.ID = "propEd_tb_" & propertyPath
        Controls.Add(fTextControl)
        ApplyTextBoxAttributes()

    End Sub

    Protected Overridable Sub ApplyTextBoxAttributes()
    End Sub

End Class

Public Class StringProperty
    Inherits BaseTextBoxProperty
    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
		' don't update when the field is empty if the original value was null.
		If (fTextControl.Text.Trim() <> "") OrElse Not fOriginalValueNullB Then
			Try
				If fPropertyInfo.GetValue(pInstance, Nothing) <> fTextControl.Text Then _
					fPropertyInfo.SetValue(pInstance, fTextControl.Text, Nothing)
			Catch ex As Exception

			End Try

		End If
	End Sub

    Protected Overloads Overrides Sub ApplyTextBoxAttributes()
        MyBase.ApplyTextBoxAttributes()
        fTextControl.Width = New Unit("100%")
    End Sub

End Class

Public Class NumericProperty
    Inherits BaseTextBoxProperty
    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        If fTextControl.Text.Trim() <> "" Then
            ' numbers must have a value assigned first
            Dim vValue As Object = Nothing
            Select Case fPropertyInfo.PropertyType.Name
                Case "Char"
                    vValue = DirectCast(Convert.ToChar(fTextControl.Text), Object)
                    Exit Select
                Case "Byte"
                    vValue = DirectCast(Convert.ToByte(fTextControl.Text), Object)
                    Exit Select
                Case "SByte"
                    vValue = DirectCast(Convert.ToSByte(fTextControl.Text), Object)
                    Exit Select
                Case "Int16"
                    vValue = DirectCast(Convert.ToInt16(fTextControl.Text), Object)
                    Exit Select
                Case "UInt16"
                    vValue = DirectCast(Convert.ToUInt16(fTextControl.Text), Object)
                    Exit Select
                Case "Int32"
                    vValue = DirectCast(Convert.ToInt32(fTextControl.Text), Object)
                    Exit Select
                Case "UInt32"
                    vValue = DirectCast(Convert.ToUInt32(fTextControl.Text), Object)
                    Exit Select
                Case "Int64"
                    vValue = DirectCast(Convert.ToInt64(fTextControl.Text), Object)
                    Exit Select
                Case "UInt64"
                    vValue = DirectCast(Convert.ToUInt64(fTextControl.Text), Object)
                    Exit Select
                Case "Decimal"
                    vValue = DirectCast(Convert.ToDecimal(fTextControl.Text), Object)
                    Exit Select
                Case "Double"
                    vValue = DirectCast(Convert.ToDouble(fTextControl.Text), Object)
                    Exit Select
                Case "Single"
                    vValue = DirectCast(Convert.ToSingle(fTextControl.Text), Object)
                    Exit Select
                Case Else
                    Throw New ArgumentOutOfRangeException("Unknown numeric type")
            End Select
            ' switch
            fPropertyInfo.SetValue(pInstance, vValue, Nothing)
            ' if 
        End If
    End Sub
    ' UpdateValueInternal()
    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyTextBoxAttributes()
        MyBase.ApplyTextBoxAttributes()
        fTextControl.Width = New Unit("60px")
        Select Case fPropertyInfo.PropertyType.Name
            Case "Char"
                fTextControl.MaxLength = 1
                fTextControl.Width = New Unit("20px")
                Exit Select
            Case "SByte", "Byte"
                fTextControl.MaxLength = 3
                Exit Select
            Case "UInt16", "Int16"
                fTextControl.MaxLength = 6
                ' supports the negative
                Exit Select
            Case "UInt32", "Int32"
                fTextControl.MaxLength = 11
                ' supports the negative
                Exit Select
            Case Else
                fTextControl.Width = New Unit("120px")
                ' all of the rest will have no limits
                Exit Select
                ' switch
        End Select
    End Sub
    ' ApplyTextBoxAttributes()
End Class

Public Class DateProperty
    Inherits BaseTextBoxProperty

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        If fTextControl.Text.Trim() <> "" Then
            ' DateTime cannot be "empty"
            fPropertyInfo.SetValue(pInstance, Convert.ToDateTime(fTextControl.Text), Nothing)
        End If
    End Sub
    ' UpdateValueInternal()
    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyTextBoxAttributes()
        MyBase.ApplyTextBoxAttributes()
        fTextControl.Width = New Unit("60pt")
    End Sub
    ' ApplyTextBoxAttributes()
End Class

Public Class BoolProperty
    Inherits BaseProperty
    Private fDDLControl As DropDownList = Nothing

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    Public Overrides Function getProposedValue() As Object
        If fDDLControl Is Nothing Then Return Nothing
        Return fDDLControl.SelectedValue = 1
    End Function

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        Dim vObj As Object = fPropertyInfo.GetValue(pInstance, Nothing)
        If CBool(vObj) Then
            fDDLControl.SelectedIndex = 0
        Else
            fDDLControl.SelectedIndex = 1
        End If
    End Sub
    ' ApplyValueInternal()
    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
		Dim vBoolB As Boolean = fDDLControl.SelectedIndex = 0
		Try
			If fPropertyInfo.GetValue(pInstance, Nothing) <> vBoolB Then _
				fPropertyInfo.SetValue(pInstance, vBoolB, Nothing)
		Catch ex As Exception

		End Try

	End Sub
    ' UpdateValueInternal()
    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub CreateChildControls()
        fDDLControl = New DropDownList()
        fDDLControl.Width = New Unit("100px")
        fDDLControl.Items.Add(New ListItem("Yes"))
        fDDLControl.Items.Add(New ListItem("No"))
        fDDLControl.SelectedIndex = 0
        ' default
        fDDLControl.EnableViewState = False
        ToolTipToStatus(fDDLControl)
        fDDLControl.ID = "proped_Bdd_" & propertyPath
        Controls.Add(fDDLControl)
    End Sub
    ' CreateChildControls
End Class

Public Class EnumProperty
    Inherits BaseProperty
    Private fEnumControl As DropDownList

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        Dim vEnum As [Enum] = DirectCast(fPropertyInfo.GetValue(pInstance, Nothing), [Enum])
        BaseProperty.SetEnumValue(fEnumControl, fPropertyInfo.PropertyType, vEnum)
    End Sub

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        Dim vEnum As [Enum] = BaseProperty.GetEnumValue(fEnumControl, fPropertyInfo.PropertyType)

        fPropertyInfo.SetValue(pInstance, vEnum, Nothing)
    End Sub

    Public Overrides Function getProposedValue() As Object
        If fEnumControl Is Nothing Then Return Nothing
        Return BaseProperty.GetEnumValue(fEnumControl, fPropertyInfo.PropertyType)
    End Function

    Protected Overloads Overrides Sub CreateChildControls()
        fEnumControl = CreateEnumDropList(fPropertyInfo.PropertyType)
        ToolTipToStatus(fEnumControl)
        fEnumControl.ID = "Proped_dd_" & propertyPath
        Controls.Add(fEnumControl)
    End Sub
End Class

Public Class ColorProperty
    Inherits BaseProperty
    Private fColorControl As JqueryUIControls.ColorPicker
    Private fOriginalColor As Color = Color.Empty

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        fOriginalColor = DirectCast(fPropertyInfo.GetValue(pInstance, Nothing), Color)
        If fOriginalColor.IsEmpty Then
            fColorControl.hidVal.Value = ""
        End If
        'Dim vItem As ListItem = fColorControl.Items.FindByValue(fOriginalColor.Name)
        'If vItem IsNot Nothing Then
        '    fColorControl.SelectedIndex = fColorControl.Items.IndexOf(vItem)
        'ElseIf fOriginalColor.IsEmpty Then
        '    fColorControl.SelectedIndex = fColorControl.Items.Count - 2
        'Else
        '    fColorControl.SelectedIndex = fColorControl.Items.Count - 1
        'End If
    End Sub

    Public Overrides Function getProposedValue() As Object
        Try
            Return fColorControl.color
        Catch ex As Exception

        End Try


        'If (fColorControl.SelectedIndex <> -1) AndAlso (fColorControl.SelectedItem.Value <> "-1") Then
        '    Dim vColor As Color
        '    If fColorControl.SelectedItem.Value = "0" Then
        '        vColor = Color.Empty
        '    Else
        '        vColor = Color.FromName(fColorControl.SelectedItem.Value)
        '    End If
        '    Return vColor
        'End If
        Return Nothing
    End Function

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        ' if the selected item's value is -1, we'll leave the original color

        Dim vColor As Color = getProposedValue()
        fPropertyInfo.SetValue(pInstance, vColor, Nothing)

    End Sub

    Protected Overloads Overrides Sub CreateChildControls()
        Dim vEnumValues As Array = [Enum].GetValues(GetType(KnownColor))
        'fColorControl = New DropDownList()
        fColorControl = New JqueryUIControls.ColorPicker
        fColorControl.EnableViewState = False
        ToolTipToStatus(fColorControl)

        'For vI As Integer = 0 To vEnumValues.Length - 1
        '    Dim vValue As Integer = Convert.ToInt32(vEnumValues.GetValue(vI))
        '    Dim vValueName As String = [Enum].GetName(GetType(KnownColor), vValue)
        '    If Not Color.FromName(vValueName).IsSystemColor Then
        '        ' only non System colors
        '        'fColorControl.Items.Add(New ListItem(vValueName, vValueName))
        '    End If
        'Next
        ' for
        ' add an extra to hold two possible cases based on data being applied: Empty and unknown colors
        ' When this row is selected, UpdateValueInternal should not attempt to apply it back
        '        fColorControl.Items.Add(New ListItem("Empty", "0"))
        '        fColorControl.Items.Add(New ListItem("Other", "-1"))
        ' -1 will not conflict with any true color
        '        fColorControl.SelectedIndex = 0
        ' first position. We don't ever want it at -1
        fColorControl.ID = "proped_color_" & propertyPath
        fColorControl.Style.Add("display", "inline-block")
		fColorControl.miniPicker = True
		Try
			fColorControl.color = fPropertyInfo.GetValue(instance, Nothing)
		Catch ex As Exception

		End Try
		Controls.Add(fColorControl)
		Controls.Add(New LiteralControl("<a href=""javascript:void(0);"" style='margin:4px;' onclick=""$(this).parent().find('.colorSelector').first().find('div').css('background-color',''); $(this).parent().find('input[type=hidden]').val('');return false;"">Use Default</a>"))
    End Sub
End Class

Public Class UnitProperty
    Inherits BaseProperty

    Private fTextControl As TextBox = Nothing
    Private fEnumControl As DropDownList = Nothing

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        Dim vUnit As Unit = DirectCast(fPropertyInfo.GetValue(pInstance, Nothing), Unit)
        If Not vUnit.IsEmpty Then
            fTextControl.Text = vUnit.Value.ToString()

            BaseProperty.SetEnumValue(fEnumControl, GetType(UnitType), vUnit.Type)
        End If
    End Sub

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        Dim vUnit As Unit
        If fTextControl.Text.Trim() = "" Then
            ' empty edit box means empty unit struct
            vUnit = Unit.Empty
        Else
            vUnit = New Unit(Convert.ToDouble(fTextControl.Text), DirectCast(BaseProperty.GetEnumValue(fEnumControl, GetType(UnitType)), UnitType))
        End If

        fPropertyInfo.SetValue(pInstance, vUnit, Nothing)
    End Sub

    Protected Overloads Overrides Sub CreateChildControls()
        fTextControl = New TextBox()
        fTextControl.Width = New Unit("60px")
        fTextControl.EnableViewState = False
        ToolTipToStatus(fTextControl)
        fTextControl.ID = "Proped_textctrl_" & propertyPath
        Controls.Add(fTextControl)

        fEnumControl = CreateEnumDropList(GetType(UnitType))
        ToolTipToStatus(fEnumControl)
        fEnumControl.ID = "Prop_edEnumdd_" & propertyPath
        Controls.Add(fEnumControl)
    End Sub
    ' CreateChildControls()

    Public Overrides Function getProposedValue() As Object
        If fEnumControl Is Nothing Then Return Nothing
        Dim vUnit As Unit
        If fTextControl.Text.Trim() = "" Then
            ' empty edit box means empty unit struct
            vUnit = Unit.Empty
        Else
            vUnit = New Unit(Convert.ToDouble(fTextControl.Text), DirectCast(BaseProperty.GetEnumValue(fEnumControl, GetType(UnitType)), UnitType))
        End If
        Return vUnit
    End Function
End Class

Public Class FontUnitProperty
    Inherits BaseProperty

    Private fTextControl As TextBox = Nothing
    Private fFontSizeControl As DropDownList = Nothing
    Private fUnitTypeControl As DropDownList = Nothing

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        Dim vFontUnit As FontUnit = DirectCast(fPropertyInfo.GetValue(pInstance, Nothing), FontUnit)
        If Not vFontUnit.IsEmpty Then
            BaseProperty.SetEnumValue(fFontSizeControl, GetType(FontSize), vFontUnit.Type)

            fTextControl.Text = vFontUnit.Unit.Value.ToString()


            BaseProperty.SetEnumValue(fUnitTypeControl, GetType(UnitType), vFontUnit.Unit.Type)
        End If
    End Sub
    ' ApplyValueInternal()
    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        fPropertyInfo.SetValue(pInstance, getProposedValue, Nothing)
    End Sub
    ' UpdateValueInternal()
    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub CreateChildControls()
        fFontSizeControl = CreateEnumDropList(GetType(FontSize))
        ToolTipToStatus(fFontSizeControl)
        fFontSizeControl.ID = "proped_fontsz_" & propertyPath
        Controls.Add(fFontSizeControl)

        fTextControl = New TextBox()
        fTextControl.Width = New Unit("60px")
        fTextControl.EnableViewState = False
        ToolTipToStatus(fTextControl)
        fTextControl.ID = "proped_textctl_" & propertyPath
        Controls.Add(fTextControl)

        fUnitTypeControl = CreateEnumDropList(GetType(UnitType))
        fUnitTypeControl.ID = "proped_unittp_" & propertyPath
        Controls.Add(fUnitTypeControl)
        ToolTipToStatus(fUnitTypeControl)
        MyBase.CreateChildControls()
    End Sub

    Public Overrides Function getProposedValue() As Object
        Dim vFontUnit As FontUnit
        Dim vFontSize As FontSize = DirectCast(BaseProperty.GetEnumValue(fFontSizeControl, GetType(FontSize)), FontSize)
        If (fTextControl.Text.Trim() = "") AndAlso (vFontSize = FontSize.AsUnit) Then
            ' empty edit box means empty unit struct
            vFontUnit = FontUnit.Empty
        ElseIf vFontSize = FontSize.AsUnit Then
            ' use the Textbox
            vFontUnit = New FontUnit(New Unit(Convert.ToDouble(fTextControl.Text), DirectCast(BaseProperty.GetEnumValue(fUnitTypeControl, GetType(UnitType)), UnitType)))
        Else
            vFontUnit = New FontUnit(vFontSize)
        End If
        Return vFontUnit
    End Function
End Class


Public Class TemplatePropertyBuilder
    Implements ITemplate

    Public fUserText As String = ""
    ' holds the text that the user intends to put into a LiteralControl
    Public Sub InstantiateIn(ByVal pContainer As Control) Implements System.Web.UI.ITemplate.InstantiateIn
        If fUserText <> "" Then
            pContainer.Controls.Add(New LiteralControl(fUserText))
        End If
    End Sub

End Class

Public Class TemplateProperty
    Inherits BaseTextBoxProperty
    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        ' since the text isn't actually a property of the control, we start out with a blank text item
    End Sub
    ' ApplyValueInternal()
    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        If fTextControl.Text.Trim() <> "" Then
            Dim vTemplate As New TemplatePropertyBuilder()
            vTemplate.fUserText = fTextControl.Text
            ' will assign or replace the ITemplate property
            fPropertyInfo.SetValue(pInstance, vTemplate, Nothing)
        End If
    End Sub

    Protected Overloads Overrides Sub ApplyTextBoxAttributes()
        MyBase.ApplyTextBoxAttributes()

        fTextControl.Width = New Unit("100%")
        fTextControl.TextMode = TextBoxMode.MultiLine

        fTextControl.Height = New Unit(36, UnitType.Point)
    End Sub
    ' ApplyTextBoxAttributes()
End Class

Class ReadOnlyProperty
    Inherits BaseProperty

    Private fLabelControl As Label = Nothing


    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        ' since the text isn't actually a property of the control, we start out with a blank text item
        fLabelControl.Text = "&nbsp;"
        ' anticipate no text at all. If so, add this to let the TableCell draw nicely
        Try
            Dim vValue As Object = fPropertyInfo.GetValue(pInstance, Nothing)
            If vValue IsNot Nothing Then
                Dim vStr As String = vValue.ToString().Trim()

                If vStr <> "" Then
                    ' add something so the TableCell will draw like all others
                    fLabelControl.Text = vStr
                End If
            End If
        Catch generatedExceptionName As Exception
            ' catch
        End Try
    End Sub

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        ' do nothing. Cannot write to a Read Only field
    End Sub

    Protected Overloads Overrides Sub CreateChildControls()
        fLabelControl = New Label()
        fLabelControl.Font.Italic = True
        ' to distinguish it
        fLabelControl.EnableViewState = False

        Controls.Add(fLabelControl)
    End Sub

    Public Overrides Function getProposedValue() As Object
        Return Nothing
    End Function
End Class

Class NothingProperty
    Inherits BaseProperty

    Private fLabelControl As Label = Nothing

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
    End Sub

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        ' since the text isn't actually a property of the control, we start out with a blank text item
        fLabelControl.Text = "&nbsp;Nothing"
    End Sub

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        ' do nothing. Cannot write to a Read Only field
    End Sub

    Protected Overloads Overrides Sub CreateChildControls()
        fLabelControl = New Label()
        fLabelControl.Font.Italic = True
        ' to distinguish it
        fLabelControl.EnableViewState = False

        Controls.Add(fLabelControl)
    End Sub

    Public Overrides Function getProposedValue() As Object
        Return Nothing
    End Function
End Class
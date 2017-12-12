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

<ParseChildren(ChildrenAsProperties:=True)>
Public MustInherit Class BaseProperty
	Inherits System.Web.UI.WebControls.WebControl
	Implements INamingContainer
	Public fPropertyInfo As PropertyInfo
	Public propertyPath As String
	' the info for the property. Some subclasses don't use this and its null.
	Public fOwnerControl As WebControl = Nothing
	Public fHint As String = ""
	' if the PropertyInfo offers a DescriptionAttribute, this is its text.
	Public fDepth As Integer = 0
	Public mainID As Integer = 0
	' how deeply nested this object is from the initial object.
	Public ReadOnly Property objectKey() As String
		Get
			If fOwnerControl.GetType Is GetType(PropertiesGrid) Then
				Return CType(fOwnerControl, PropertiesGrid).objectKey
			End If
			Return fOwnerControl.ID
		End Get
	End Property


	Public ReadOnly Property instance() As Object
		Get
			If fOwnerControl.GetType Is GetType(PropertiesGrid) Then
				Return CType(fOwnerControl, PropertiesGrid).xInstance
			End If
			Return fOwnerControl.ID
		End Get
	End Property

	Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer)
		fPropertyInfo = pPropertyInfo
		fOwnerControl = pOwnerControl
		fDepth = pDepth

		' get the hint (tooltip) from the DescriptionAttribute of the property
		If fPropertyInfo IsNot Nothing Then
			Dim vAttribA As Object() = fPropertyInfo.GetCustomAttributes(GetType(DescriptionAttribute), True)
			If vAttribA.Length > 0 Then
				fHint = DirectCast(vAttribA(0), DescriptionAttribute).Description
			End If
		End If
	End Sub

	Public ReadOnly Property changes() As ComparatorDS.DTIPropDifferencesDataTable
		Get
			If Page.Session("Changes_" & objectKey) Is Nothing Then
				Page.Session("Changes_" & objectKey) = New ComparatorDS.DTIPropDifferencesDataTable
			End If
			Return Page.Session("Changes_" & objectKey)
		End Get
	End Property

	Protected Sub addChange(ByVal value As Object)
		If propertyPath Is Nothing Then Exit Sub
		propertyPath = propertyPath.Trim(New Char() {"."})
		If value Is Nothing Then
			changes.AddDTIPropDifferencesRow(propertyPath, Nothing, Nothing, objectKey, mainID)
		Else
			Dim xml As String = Comparator.searializeToBase64String(value)
			If findDiffRow(changes, mainID, propertyPath, objectKey) Is Nothing Then
				If Not xml Is Nothing Then
					changes.AddDTIPropDifferencesRow(propertyPath, xml, value.GetType.AssemblyQualifiedName, objectKey, mainID)
				End If
			Else
				findDiffRow(changes, mainID, propertyPath, objectKey).PropertyValue = xml
			End If
		End If
	End Sub

	Public Shared Function findDiffRow(ByVal dt As ComparatorDS.DTIPropDifferencesDataTable, ByVal mainid As Integer, ByVal propertypath As String, ByVal objectKey As String) As ComparatorDS.DTIPropDifferencesRow
		Dim dv As New DataView(dt, "propertyPath = '" & propertypath & "' and mainID = " & mainid & "  and objectKey = '" & objectKey & "'", "", DataViewRowState.CurrentRows)
		If dv.Count > 0 Then Return dv(0).Row
		Return Nothing
	End Function

	Public Sub ApplyValueToControl(ByVal pInstance As Object)
		If pInstance IsNot Nothing Then
			EnsureChildControls()
			ApplyValueInternal(pInstance)
		End If
	End Sub

	''' <summary>
	''' ApplyValueInternal is an abstract method to transfer the value from pInstance into the control
	''' </summary>
	<System.ComponentModel.Description("ApplyValueInternal is an abstract method to transfer the value from pInstance into the control")>
	Protected MustOverride Sub ApplyValueInternal(ByVal pInstance As Object)

	Public Sub UpdateValueFromControl(ByVal instance As Object)
		If instance IsNot Nothing Then
			EnsureChildControls()
			Dim orig As Object = Me.fPropertyInfo.GetValue(instance, Nothing)
			UpdateValueInternal(instance)
			Dim newval As Object = Me.fPropertyInfo.GetValue(instance, Nothing)
			Try
				If Not orig = newval Then
					addChange(newval)
				End If
			Catch ex As Exception
			End Try

		End If
	End Sub

	Protected MustOverride Sub UpdateValueInternal(ByVal pInstance As Object)

	Public Sub ToolTipToStatus(ByVal pControl As WebControl)
		If (pControl IsNot Nothing) AndAlso (fHint <> "") Then
			pControl.Attributes("OnFocus") = "javascript:window.status = '" & fHint & "';"
			pControl.Attributes("OnBlur") = "javascript:window.status = '';"
		End If
	End Sub

	Public Function getValue() As Object
		Dim newInstance As Object = instance
		For Each prop As String In Me.propertyPath.Split(".")
			Dim tmppinfo As PropertyInfo = Nothing
			If prop.Contains("(") Then
				prop = prop.Trim(")")
				Dim idx As Integer = prop.Substring(prop.IndexOf("(") + 1)
				prop = prop.Substring(0, prop.IndexOf("("))
				newInstance = newInstance(idx)
			Else
				Try
					tmppinfo = newInstance.GetType().GetProperty(prop)
				Catch ex As Exception
					For Each tmppinfo2 As PropertyInfo In newInstance.GetType().GetProperties()
						If tmppinfo2.Name = prop Then tmppinfo = tmppinfo2
					Next
				End Try
				'If tmppinfo Is fPropertyInfo Then Exit For
				If tmppinfo.GetIndexParameters().Length = 1 Then
					newInstance = tmppinfo.GetValue(newInstance, New Object() {0})
				Else
					newInstance = tmppinfo.GetValue(newInstance, Nothing)
				End If
			End If
		Next
		Return newInstance
	End Function

	Public Function CreateEnumDropList(ByVal pEnumType As Type) As DropDownList
		Dim vEnumControl As New DropDownList()
		FillInEnumDropList(vEnumControl, pEnumType)
		vEnumControl.ID = "proped_dd_" & Me.propertyPath
		Return vEnumControl
	End Function

	Public Shared Sub FillInEnumDropList(ByVal pEnumControl As DropDownList, ByVal pEnumType As Type)
		Dim vEnumValues As Array = [Enum].GetValues(pEnumType)
		' .NET's tool for giving us all of an enum type's values
		For vI As Integer = 0 To vEnumValues.Length - 1
			Dim vValue As Integer = Convert.ToInt32(vEnumValues.GetValue(vI))
			pEnumControl.Items.Add(New ListItem([Enum].GetName(pEnumType, vValue), vValue.ToString()))
		Next
		' for
		pEnumControl.SelectedIndex = 0
		' first position. We don't ever want it at -1
		'pEnumControl.EnableViewState = False
	End Sub

	Public Shared Sub SetEnumValue(ByVal pEnumControl As DropDownList, ByVal pEnumType As Type, ByVal pEnumValue As [Enum])
		Dim vItem As ListItem = pEnumControl.Items.FindByText([Enum].GetName(pEnumType, pEnumValue))
		If vItem Is Nothing Then
			Throw New ArgumentNullException("Unknown Name in type: " & pEnumType.FullName)
		Else
			pEnumControl.SelectedIndex = pEnumControl.Items.IndexOf(vItem)

		End If
	End Sub

	Public Shared Function GetEnumValue(ByVal pEnumControl As DropDownList, ByVal pEnumType As Type) As [Enum]
		If pEnumControl.SelectedIndex = -1 Then
			Throw New Exception("Bug: Cannot allow unassigned droplist")
		Else
			Dim vValue As Integer = Convert.ToInt32(pEnumControl.SelectedItem.Value)
			Dim vEnum As [Enum] = DirectCast(([Enum].ToObject(pEnumType, vValue)), [Enum])

			Return vEnum
		End If
	End Function

	Public MustOverride Function getProposedValue() As Object


End Class

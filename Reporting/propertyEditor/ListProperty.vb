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



Public Class ListProperty
    Inherits BaseObjectProperty

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer, ByVal pParentObjectProperty As BaseObjectProperty)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth, pParentObjectProperty)
    End Sub
    ' constructor
    Protected Overloads Overrides ReadOnly Property ButtonNoun() As String
        Get
            Return "Items"
        End Get
    End Property

    Public Property xItemCount() As Integer
        Get
            If ViewState("ItemCount") IsNot Nothing Then
                Return CInt(ViewState("ItemCount"))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("ItemCount") = value
        End Set
    End Property

    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        ' no data to work with
        fButtonControl.Enabled = (pInstance IsNot Nothing) AndAlso (xItemCount > 0)
    End Sub

    Protected Overloads Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()
        fButtonControl.Enabled = xItemCount > 0
    End Sub

    Public Overrides Function getProposedValue() As Object
        Return Nothing
    End Function
End Class


Public Class ListItemProperty
    Inherits BaseObjectProperty
    Public index As Integer

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal itemPropertyInfo As PropertyInfo, ByVal pDepth As Integer, ByVal pParentObjectProperty As BaseObjectProperty, ByVal propertypath As String, ByVal idx As Integer)
        MyBase.New(pOwnerControl, itemPropertyInfo, pDepth, pParentObjectProperty)
        Me.index = idx
        Me.propertyPath = propertypath.Trim(".") & ".item(" & idx & ")"
    End Sub
    ' constructor
    Protected Overloads Overrides ReadOnly Property ButtonNoun() As String
        Get
            Return "Item Details"
        End Get
    End Property

    ' xInstanceType returns the class type based on the textual name and assembly
    Public ReadOnly Property xInstanceType() As Type
        Get
            Dim vAssemblyName As String = ViewState("Assembly").ToString()
            Dim vAssembly As Assembly = Assembly.Load(vAssemblyName)
            Return vAssembly.[GetType](ViewState("Class").ToString(), True)
        End Get
    End Property

    '-----------------------------------------------------------------------------------------------
    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        MyBase.ApplyValueInternal(pInstance)
        If pInstance IsNot Nothing Then
            ' we keep information about the Class Type in View State.
            ' To restore to the real class, we need to know the Assembly name too.
            ViewState("Class") = pInstance.[GetType]().FullName
            ViewState("Assembly") = pInstance.[GetType]().Assembly.FullName
        End If
    End Sub

    Public Overrides Function getProposedValue() As Object
        Return Nothing
    End Function

End Class
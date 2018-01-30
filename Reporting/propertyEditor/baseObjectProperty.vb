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


Public MustInherit Class BaseObjectProperty
    Inherits BaseProperty
    Public fButtonControl As ImageButton = Nothing
    Public fChildTableRows As New Collections.Generic.List(Of PropertyTableRow)
    Public fParentObjectProperty As BaseObjectProperty = Nothing
    ' refers to the parent in the ObjectProperty hierarchy

    Public Sub New(ByVal pOwnerControl As WebControl, ByVal pPropertyInfo As PropertyInfo, ByVal pDepth As Integer, ByVal pParentObjectProperty As BaseObjectProperty)
        MyBase.New(pOwnerControl, pPropertyInfo, pDepth)
        'EnableViewState = True
        fParentObjectProperty = pParentObjectProperty
    End Sub
    ' constructor
    Protected Overridable ReadOnly Property ButtonNoun() As String
        Get
            Return ""
        End Get
    End Property

    Public Property xOpen() As Boolean
        Get
            ' just be assigned to be open
            Return Page.Session("PropEditor_open_" & propertyPath & "Open") IsNot Nothing
        End Get
        Set(ByVal value As Boolean)
            If value Then
                Page.Session("PropEditor_open_" & propertyPath & "Open") = "1"
            Else
                Page.Session.Remove("PropEditor_open_" & propertyPath & "Open")
            End If
        End Set
    End Property

    Public Sub AddChildTableRow(ByVal pControl As PropertyTableRow)
        fChildTableRows.Add(pControl)
    End Sub

    Protected Overloads Overrides Sub ApplyValueInternal(ByVal pInstance As Object)
        ' no data to work with
        fButtonControl.Visible = (pInstance IsNot Nothing)
    End Sub

    Protected Overloads Overrides Sub UpdateValueInternal(ByVal pInstance As Object)
        ' no data to work with
    End Sub

    Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
        Dim i As Integer = 0
        For Each ctrl As Control In Me.Controls
            ctrl.EnableViewState = False
        Next
        MyBase.OnPreRender(e)
    End Sub

    Protected Overloads Overrides Sub CreateChildControls()
        fButtonControl = New ImageButton()
        'fButtonControl.ID = "pedit_" & Me.propertyPath
        fButtonControl.EnableViewState = False
        If xOpen Then
			fButtonControl.ImageUrl = BaseClasses.Scripts.ScriptsURL(False) & "Reporting/close.png"
			'fButtonControl.ImageUrl = Page.ClientScript.GetWebResourceUrl(Me.GetType, "PropertiesEditorvb.close.png")
			'fButtonControl.Text = "- " & ButtonNoun
		Else
			fButtonControl.ImageUrl = BaseClasses.Scripts.ScriptsURL(False) & "Reporting/open.png"
			'fButtonControl.ImageUrl = Page.ClientScript.GetWebResourceUrl(Me.GetType, "PropertiesEditorvb.open.png")
			'fButtonControl.Text = "+ " & ButtonNoun
		End If
        propertyPath = propertyPath.Trim(".")
        fButtonControl.CausesValidation = False
        fButtonControl.ID = "Propedbtn_" & propertyPath.Replace("(", "_").Replace(")", "_")

        AddHandler fButtonControl.Click, AddressOf Button_Click
        Controls.Add(fButtonControl)

    End Sub

    Public ReadOnly Property propGrid() As PropertiesGrid
        Get
            Dim ctrl As Control = fOwnerControl
            While ctrl IsNot Nothing
                If ctrl.GetType Is GetType(PropertiesGrid) Then
                    Return ctrl
                End If
                ctrl = ctrl.Parent
            End While
            Return Nothing
        End Get
    End Property

    '    Protected Sub Button_Click(ByVal sender As Object, ByVal e As System.EventArgs)
    Protected Sub Button_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        xOpen = Not xOpen
        propGrid.expandedObject = True

        propGrid.onDoExpandSubProp(Me.propertyPath, xOpen)
        setButton()

    End Sub

    Public Function setButton() As Integer
        SetTRVisibility(xOpen)
        ' correct the button
        If xOpen Then
            If fButtonControl Is Nothing Then CreateChildControls()
			fButtonControl.ImageUrl = BaseClasses.Scripts.ScriptsURL(False) & "/res/Reporting/close.png"
			'fButtonControl.ImageUrl = Page.ClientScript.GetWebResourceUrl(Me.GetType, "PropertiesEditorvb.close.png")
			'CType(fOwnerControl, PropertiesEditor).DataBind()
			If fOwnerControl.GetType Is GetType(PropertiesGrid) Then
                Dim newInstance As Object = getValue()
                'CType(fOwnerControl, PropertiesEditor).CreateObjectRowChildren1(newInstance, fPropertyInfo.PropertyType, propertyPath, xOpen, Me.propertyPath, Me, Me.Parent.Parent, Me.Parent.Parent.Parent.Controls.IndexOf(Me.Parent.Parent))
                Return 1
                'If newInstance IsNot Nothing Then
                '    Return CType(fOwnerControl, PropertiesEditor).CreateTableRows(newInstance.GetType, newInstance, "." & propertyPath, Me.propertyPath, Me.xOpen, Me, Me.Parent.Parent.Parent.Controls.IndexOf(Me.Parent.Parent))
                'Else
                '    Return CType(fOwnerControl, PropertiesEditor).CreateTableRows(fPropertyInfo.PropertyType, newInstance, "." & propertyPath, Me.propertyPath, Me.xOpen, Me, Me.Parent.Parent.Parent.Controls.IndexOf(Me.Parent.Parent))
                'End If

            End If
        Else
            If Not fButtonControl Is Nothing Then _
			fButtonControl.ImageUrl = BaseClasses.Scripts.ScriptsURL(False) & "/res/Reporting/open.png"
			'fButtonControl.ImageUrl = Page.ClientScript.GetWebResourceUrl(Me.GetType, "PropertiesEditorvb.open.png")
			'fButtonControl.Text = "+"
			Return 0
        End If
    End Function

    Protected Sub SetTRVisibility(ByVal pVisibleB As Boolean)
        For Each vControl As PropertyTableRow In fChildTableRows
            If Not vControl.fRemovedB Then
                vControl.Visible = pVisibleB
                If TypeOf vControl.fPropertyControl Is BaseObjectProperty Then
                    ' change children's visibility
                    DirectCast(vControl.fPropertyControl, BaseObjectProperty).SetTRVisibility(pVisibleB AndAlso DirectCast(vControl.fPropertyControl, BaseObjectProperty).xOpen)
                    ' if
                End If
            End If
            ' if / foreach
        Next
    End Sub
End Class

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

Friend Class BaseDesignerData
    <Browsable(True)> _
    Public Property BaseReadWrite() As Boolean
        Get
            Return fBaseReadWrite
        End Get
        Set(ByVal value As Boolean)
            fBaseReadWrite = value
        End Set
    End Property
    Private fBaseReadWrite As Boolean = False
    ' storage for above
    <Browsable(True)> _
    Public ReadOnly Property BaseReadOnly() As Integer
        Get
            Return fBaseReadOnly
        End Get
    End Property
    Private fBaseReadOnly As Integer = 10
    ' storage for above
    <Browsable(False)> _
    Public Property BaseNonBrowsable() As String
        Get
            Return fBaseNonBrowsable
        End Get
        Set(ByVal value As String)
            fBaseNonBrowsable = value
        End Set
    End Property
    Private fBaseNonBrowsable As String = "Non browsable text"
    ' storage for above
End Class
' class BaseDesignerData
Friend Class ChildDesignerData
    Inherits BaseDesignerData
    <Browsable(True)> _
    Public Property ChildReadWrite() As Color
        Get
            Return fChildReadWrite
        End Get
        Set(ByVal value As Color)
            fChildReadWrite = value
        End Set
    End Property
    Private fChildReadWrite As Color = Color.Blue
    ' storage for above
    <Browsable(True)> _
    Protected Property ChildNonPublic() As Unit
        Get
            Return fChildNonPublic
        End Get
        Set(ByVal value As Unit)
            fChildNonPublic = value
        End Set
    End Property
    Private fChildNonPublic As New Unit("100%")
    ' storage for above
End Class
' class ChildDesignerData
'----- CLASS PropertiesEditorDesigner --------------------------------------------

''' <remarks>
''' PropertiesEditorDesigner is the ControlDesigner for PropertiesEditor.
''' It assigns xInstance to an instance of ChildDesignerData. That class
''' was designed to show a variety of properties that will show or hide 
''' based on the properties of PropertiesEditor.
''' </remarks>
<System.ComponentModel.Description("")> _
Public Class PropertiesEditorDesigner
    Inherits ControlDesigner

    Public Overloads Overrides Function GetDesignTimeHtml() As String

        ' Get the instance this designer applies to
        '
        Dim vControl As PropertiesGrid = DirectCast(Component, PropertiesGrid)

        ' Render the control at design time
        '
        Try
            ' establish the data using a ChildDesignerData instance
            Dim vData As New ChildDesignerData()
            vControl.xInstance = vData
            vControl.RenderAtDesignTime()

            Dim vHTML As String = MyBase.GetDesignTimeHtml()
            vControl.Controls.Clear()
            Return vHTML
        Catch e As Exception
            Return e.Message

        End Try
    End Function
    ' GetDesignTimeHtml()
End Class
' class PropertiesEditorDesigner


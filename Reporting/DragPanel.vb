Public Class DragPanel
    Inherits Panel

    Public Property ContentString() As String
    Public Property LayoutString() As String

    Public Property editable As Boolean = True

    Private Sub DragPanel_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        jQueryLibrary.jQueryInclude.RegisterJQueryUI(Me.Page)
        jQueryLibrary.jQueryInclude.addScriptFile(Page, "Reporting/DragPanel.js", , True)
        jQueryLibrary.jQueryInclude.addScriptFile(Page, "Reporting/DragPanel.css", , True)
    End Sub

    Private Sub DragPanel_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not editable Then jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, "DragEditOn = false")
        jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, "setupDragable('" & Me.ClientID & "',""" & LayoutString & """,""" & ContentString & """);")
    End Sub

End Class

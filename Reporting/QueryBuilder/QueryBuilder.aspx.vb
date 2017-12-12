Imports System.Text.RegularExpressions
Public Class QueryBuilder
    Inherits ReportEditorBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        jQueryLibrary.ThemeAdder.AddTheme(Me, jQueryLibrary.ThemeAdder.themes.smoothness, True, False)

        '     Dim noteregex As Regex = New Regex( _
        '  "\""note\""\s*:\s*\""(?<note>[\x01-\x7E]*)\""\s*\,\s*\""updated_at\""", _
        'RegexOptions.CultureInvariant _
        'Or RegexOptions.Compiled _
        ')

        '     Dim s As String = noteregex.Match("asdasd").Groups("note").ToString()
    End Sub

End Class
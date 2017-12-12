Imports System
Imports System.ComponentModel
Imports System.Security.Permissions
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Drawing
Imports System.Web.UI.HtmlTextWriterTag
Imports Reporting.dsReports
Imports System.Enum
Imports BaseClasses

Public Class ParmDisplay
    Inherits Panel

    Friend WithEvents Submit As New Button

    Public Enum parmType
        Textbox = 0
        Datepicker = 1
        NumberPicker = 2
        DropDown = 3
        Autocomplete = 4
        NameValueDropDown = 5
        NameValueAutocomplete = 6
    End Enum

    Private ReadOnly Property sqlhelper() As BaseClasses.BaseHelper
        Get
            Return parentGraph.sqlhelper
        End Get
    End Property

    Public ReadOnly Property parentGraph() As Graph
        Get
            Return Me.Parent
        End Get
    End Property

    Public Function addParmControl(ByVal name As String, Optional ByVal displayname As String = Nothing, Optional ByVal type As parmType = parmType.Textbox, Optional ByVal controlOptions As String = Nothing) As Control
        If displayname Is Nothing OrElse displayname = "" Then
            displayname = name
        End If
        Dim c As Control = Nothing
        If type = parmType.Textbox Then
            c = New TextBox
        ElseIf type = parmType.Datepicker Then
            c = New JqueryUIControls.DatePicker
        ElseIf type = parmType.NumberPicker Then
            c = New JqueryUIControls.maskedTextbox
            If controlOptions Is Nothing Then
                controlOptions = "99999999"
            End If
            CType(c, JqueryUIControls.maskedTextbox).mask = controlOptions
        ElseIf type = parmType.DropDown Then
            c = New DropDownList
            For Each Str As String In controlOptions.Split(vbCrLf)
                Str = Str.Trim
                If Str.Contains("#") Then
                    Dim display As String = Str.Split("#")(0).Trim("#")
                    Dim value As String = Str.Split("#")(1).Trim("#")
                    CType(c, DropDownList).Items.Add(New ListItem(display, value))
                Else
                    CType(c, DropDownList).Items.Add(Str.Trim)
                End If

            Next
        ElseIf type = parmType.Autocomplete OrElse parmType.NameValueAutocomplete OrElse parmType.NameValueDropDown Then
            c = New JqueryUIControls.Autocomplete
            Dim ops() As String = controlOptions.Split(vbCrLf)
            If ops.Length > 1 Then
                Dim numReturned As Integer = 20
                Dim searchPatern As String = "%{0}%"
                If ops.Length > 2 Then
                    Integer.TryParse(ops(2).Trim, numReturned)
                End If
                If ops.Length > 3 Then
                    If ops(3).IndexOf("%") > -1 Then
                        searchPatern = ops(3).Trim
                    End If
                End If
                If type = parmType.NameValueAutocomplete OrElse type = parmType.NameValueDropDown Then
                    Dim sql As String = ops(0).Trim
                    Dim valcol As String = ops(1).Trim
                    Dim displaycol As String = ""
                    If valcol.Contains(",") Then
                        displaycol = valcol.Split(",")(1)
                        valcol = valcol.Split(",")(0)
                    End If
                    If displaycol = "" Then displaycol = valcol
                    If type = parmType.NameValueDropDown Then
                        c = New DropDownList
                        If Not sql.ToLower.Contains("select ") Then
                            sql = String.Format("select {0},{1} from {2} order by {0}", displaycol, valcol, sql)
                        End If
                        Dim dt As DataTable = DataBase.createHelper(Me.parentGraph.parentReport.ReportDataConnection).FillDataTable(sql)
                        For Each row As DataRow In dt.Rows
                            CType(c, DropDownList).Items.Add(New ListItem(row(displaycol), row(valcol)))
                        Next
                    Else
                        If Not sql.ToLower.Contains("select ") Then
                            sql = String.Format("select top {3} {0},{1} from {2} where {0} like @parm order by {0}", displaycol, valcol, sql, numReturned)
                        End If
                        CType(c, JqueryUIControls.Autocomplete).setSelectAutocomplete(sql, displaycol, valcol, searchPatern, Me.parentGraph.parentReport.ReportDataConnection)
                    End If
                Else
                    CType(c, JqueryUIControls.Autocomplete).setDistinctAutocomplete(ops(0).Trim, ops(1).Trim, numReturned, searchPatern, Me.parentGraph.parentReport.ReportDataConnection)
                End If
            End If
        End If
        If Not c Is Nothing Then
            'c.ID = "parm_" & Me.parentGraph.GraphID & "_" & name
            Me.Controls.Add(New LiteralControl(displayname & ": "))
            Me.Controls.Add(c)
            'If type = parmType.Autocomplete Then
            '    If controlOptions.Split(vbCrLf).Length >= 2 Then
            '        CType(c, JqueryUIControls.Autocomplete).setDistinctAutocomplete(controlOptions.Split(vbCrLf)(0), controlOptions.Split(vbCrLf)(1), , , Me.parentGraph.parentReport.ReportDataConnection)
            '    End If
            'End If

            Me.Controls.Add(New LiteralControl("<br />"))
        End If
        Return c
    End Function

    Public Function parentSavedParms() As Dictionary(Of String, dsReports.DTIGraphParmsRow)
        Dim dv As New DataView(parentGraph.parentReport.graphParmsDT, "graph_id = " & parentGraph.GraphRowId, "", DataViewRowState.CurrentRows)
        Dim ht As New Dictionary(Of String, dsReports.DTIGraphParmsRow)
        For Each rv As DataRowView In dv
            ht.Add(rv("Name").ToString.ToLower, rv.Row)
        Next
        Return ht
    End Function

    Private Sub submitClicked()
        For Each key As String In controlParmHash.Keys
            Dim c As Control = controlParmHash(key)
            If Not c Is Nothing Then
                If TypeOf c Is JqueryUIControls.DatePicker Then
                    Dim dte As Date = Date.Today
                    If Not Date.TryParse(Me.Page.Request.Params(c.UniqueID), dte) Then
                        dte = Date.Today
                    End If
                    parentGraph.clickedvals.Add(key, dte)
                ElseIf TypeOf c Is DropDownList Then
                    parentGraph.clickedvals.Add(key, Me.Page.Request.Params(c.UniqueID))
                ElseIf TypeOf c Is JqueryUIControls.Autocomplete Then
                    parentGraph.clickedvals.Add(key, CType(c, JqueryUIControls.Autocomplete).Value)
                Else
                    parentGraph.clickedvals.Add(key, Me.Page.Request.Params(c.UniqueID))
                End If
            End If
        Next
        Me.Page.Response.Redirect(Me.Page.Request.Url.PathAndQuery)
    End Sub

    Public Shared Function getParmsFromSql(ByVal sqlString As String) As String()
        Dim a As New List(Of String)
        Dim regex1 As Regex = New Regex("\x40\w+", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant _
            Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)
        For Each singleMatch As Match In regex1.Matches(sqlString)
            Dim parmName As String = singleMatch.ToString
            parmName = parmName.Trim("@").ToLower
            a.Add(parmName)
        Next
        Return a.ToArray
    End Function

    Public controlParmHash As New Hashtable
    'Public hasparms As Boolean = False

    Public ReadOnly Property hasparms() As Boolean
        Get
            For Each parmName As String In getParmsFromSql(parentGraph.SQLStmt)
                If Not parentGraph.clickedvals.ContainsKey(parmName) Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    Private Sub ReportParmDisplay_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Submit.Text = "Submit"
        Dim savedparms As Dictionary(Of String, dsReports.DTIGraphParmsRow) = parentSavedParms()
        For Each parmName As String In savedparms.Keys
            parmName = parmName.ToLower
            If Not parentGraph.clickedvals.ContainsKey(parmName) Then
                Dim r As dsReports.DTIGraphParmsRow = savedparms(parmName)
                Dim c As Control = addParmControl(parmName, r.DisplayName, r.Parm_Type, r.ParmProperties)
                controlParmHash.Add(parmName, c)
            End If
        Next
        For Each parmName As String In getParmsFromSql(parentGraph.SQLStmt)
            If Not parentGraph.clickedvals.ContainsKey(parmName) AndAlso Not controlParmHash.ContainsKey(parmName) Then
                If savedparms.ContainsKey(parmName.ToLower) Then
                    Dim r As dsReports.DTIGraphParmsRow = savedparms(parmName)
                    Dim c As Control = addParmControl(parmName, r.DisplayName, r.Parm_Type, r.ParmProperties)
                    controlParmHash.Add(parmName, c)
                Else
                    controlParmHash.Add(parmName, addParmControl(parmName))
                End If
            End If
        Next
        Me.Controls.Add(Submit)

        If hasparms Then
            Me.Controls.AddAt(0, New LiteralControl("<fieldset class='reportField'><legend>" & Me.parentGraph.GraphName & "</legend>"))
            Me.Controls.Add(New LiteralControl("</fieldset>"))
        End If
        Submit.Visible = hasparms

        If Me.Page.Request.Params(Submit.UniqueID) = Submit.Text Then
            submitClicked()
            Me.Visible = False
            'hasparms = False
        End If


    End Sub

End Class

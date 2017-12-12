Public Class SummaryTables
    Inherits System.Web.UI.WebControls.Panel

#Region "Properties"

    Public Property DataSource() As DataTable

    Private _dv As DataView
    Public ReadOnly Property dv As DataView
        Get
            If _dv Is Nothing Then
                _dv = New DataView(DataSource, "", groupedCol, DataViewRowState.CurrentRows)
            End If
            Return _dv
        End Get
    End Property

    Public Property groupedCol() As String = ""

    Public Property TotalFormat() As String = "Total for {0} ({1})"

    Public Property GroupFormat() As String = "{0} : {1}"

    Public Property VisibleCols() As String = ""

    Public Property ColumnTitles() As String = ""

    Public Property nullDisplay() As String = ""

    Public Property headingColor As System.Drawing.Color = System.Drawing.Color.DarkBlue

    Public Property headingText As System.Drawing.Color = System.Drawing.Color.White

#End Region

    Public Function getVisibleColList() As List(Of String)
        Dim l As New List(Of String)
        If VisibleCols.Trim = "" Then
            For Each c As DataColumn In DataSource.Columns
                l.Add(c.ColumnName.ToLower())
            Next
        Else
            For Each itm As String In VisibleCols.Split(",")
                l.Add(itm.ToLower())
            Next
        End If
        Return l
    End Function

    Public Function getVisibleColListNoGroups() As List(Of String)
        Dim l As List(Of String) = getVisibleColList()
        Dim grps As List(Of String) = getGroupCols()
        For Each i As String In grps
            l.Remove(i)
        Next
        Return l
    End Function

    Public Function getGroupCols() As List(Of String)
        Dim l As New List(Of String)
        For Each itm As String In groupedCol.ToLower().Replace(" asc", "").Replace(" desc", "").Split(",")
            If Not itm.Trim = "" Then _
                l.Add(itm)
        Next
        Return l
    End Function

    Public Function getColumnTotal(colname As String) As Double
        Return 0
    End Function

    Public Function getColumnName(colname As String) As String
        Try
            Dim colindex As Integer = dv.Table.Columns.IndexOf("colname")
            If colindex = -1 Then Return colname
            Dim cols As String() = ColumnTitles.Split(",")
            If cols.Length > colindex Then Return cols(colindex)
        Catch ex As Exception

        End Try
        Return colname
    End Function

    Public Function makeFooter(lastrow As DataRow, nextRow As DataRow) As String
        Dim s As String = "</table> <div class='totals'>"
        For Each c As String In groupsNotEqual(lastrow, nextRow)
            s &= String.Format("<div class='totalLine'>" & TotalFormat & "</div>", getColumnName(c), getColumnTotal(c))
        Next
        s &= "</div>"
        Return s
    End Function

    Public Function makeHeader(lastrow As DataRow, nextRow As DataRow) As String
        Dim s As String = ""
        If Not lastrow Is Nothing Then
            s &= makeFooter(lastrow, nextRow)
        End If
        s &= String.Format("<div class='groupHeader' style='background-color:{0};color:{1};' >", System.Drawing.ColorTranslator.ToHtml(headingColor), System.Drawing.ColorTranslator.ToHtml(headingText))
        Dim groupCols As List(Of String) = getGroupCols()
        For Each colname As String In groupCols
            Try
                s &= "<div class='groupline'>" & String.Format(GroupFormat, getColumnName(colname), nextRow(colname)) & "</div>"
            Catch ex As Exception

            End Try
        Next
        s &= "</div>" & vbCrLf & "  <table width='100%' class='reportTable'> <tr class='headingRow'>"
        For Each col As String In getVisibleColListNoGroups()
            s &= "<td>" & getColumnName(col) & "</td>"
        Next
        s &= "</tr>"
        Return s
    End Function

    Public Function groupsNotEqual(r1 As DataRow, r2 As DataRow) As List(Of String)
        Dim l As New List(Of String)
        If r1 Is Nothing OrElse r2 Is Nothing Then Return getGroupCols()
        For Each col As String In getGroupCols()
            If Not r1(col) = r2(col) Then
                l.Add(col)
            End If
        Next
        Return l
    End Function

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)
        'Dim str As String = ""
        Me.RenderBeginTag(writer)
        Dim lastRow As DataRow = Nothing
        Dim cols As List(Of String) = getVisibleColListNoGroups()
        For Each rv As DataRowView In dv
            If lastRow Is Nothing Then
                writer.Write(makeHeader(lastRow, rv.Row))
            Else
                If groupsNotEqual(rv.Row, lastRow).Count > 0 Then _
                    writer.Write(makeHeader(lastRow, rv.Row))
            End If

            writer.Write("<tr class='reportrow'>")
            For Each col As String In cols
                writer.Write(String.Format("<td>{0}</td>", rv(col)))
            Next
            writer.Write("</tr>" & vbCrLf)
            lastRow = rv.Row
        Next
        writer.Write(makeFooter(lastRow, Nothing))
        Me.RenderEndTag(writer)
    End Sub

End Class


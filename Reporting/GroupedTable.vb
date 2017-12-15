Public Class GroupedTable
    Inherits Table

#Region "Properties"

    Private DataSourceValue As DataTable
    Public Property DataSource() As DataTable
        Get
            Return DataSourceValue
        End Get
        Set(ByVal value As DataTable)
            DataSourceValue = value
        End Set
    End Property

    Private spaceAfterTotalValue As Boolean = False
    Public Property rowAfterTotal() As Boolean
        Get
            Return spaceAfterTotalValue
        End Get
        Set(ByVal value As Boolean)
            spaceAfterTotalValue = value
        End Set
    End Property

    'Private groupedColsValue As String
    'Public Property groupedCols() As String
    '    Get
    '        Return groupedColsValue
    '    End Get
    '    Set(ByVal value As String)
    '        groupedColsValue = value
    '    End Set
    'End Property

    Private groupedColValue As String
    Public Property groupedCol() As String
        Get
            Return groupedColValue
        End Get
        Set(ByVal value As String)
            groupedColValue = value
        End Set
    End Property

    Private TotalFormatValue As String = "Total for {0} ({1})"
    Public Property TotalFormat() As String
        Get
            Return TotalFormatValue
        End Get
        Set(ByVal value As String)
            TotalFormatValue = value
        End Set
    End Property

    Private TotalColumnFormatValue As String = ""
    Public Property TotalColumnFormat() As String
        Get
            Return TotalColumnFormatValue
        End Get
        Set(ByVal value As String)
            TotalColumnFormatValue = value
        End Set
    End Property

    Private visableColsValue As String
    Public Property VisibleCols() As String
        Get
            Return visableColsValue
        End Get
        Set(ByVal value As String)
            visableColsValue = value
        End Set
    End Property

    Private ColumnTitlesValue As String
    Public Property ColumnTitles() As String
        Get
            Return ColumnTitlesValue
        End Get
        Set(ByVal value As String)
            ColumnTitlesValue = value
        End Set
    End Property

    Private removeRepeatsValue As Boolean = True
    Public Property removeRepeats() As Boolean
        Get
            Return removeRepeatsValue
        End Get
        Set(ByVal value As Boolean)
            removeRepeatsValue = value
        End Set
    End Property

    Private nullDisplayValue As String = ""
    Public Property nullDisplay() As String
        Get
            Return nullDisplayValue
        End Get
        Set(ByVal value As String)
            nullDisplayValue = value
        End Set
    End Property

	Public Property tableStyle As TableStyles = TableStyles.Rounded

#End Region

	Enum TableStyles
        NotSet
        Bars
        Blueborder
        Blueborderhover
        Bluegradient
        Bluezebra
        Bluehover
        DCoLogo
        Indigohover
        Plain
        Plainhover
        Purplehover
        Zebrahover
        Lightbluehover
        Rounded
    End Enum

    Public Event hadError(ex As Exception)

    Private Function cols() As List(Of String)
        Dim ret As New List(Of String)
        If VisibleCols IsNot Nothing AndAlso VisibleCols.Length > 0 Then
            For Each str As String In VisibleCols.Split(",")
                ret.Add(str)
            Next
        Else
            For Each col As DataColumn In DataSource.Columns
                ret.Add(col.ColumnName)
            Next
        End If
        Return ret
    End Function

    Private Function makeCell(ByVal value As Object, Optional ByVal cssClass As String = "valueCell") As TableCell
        Dim c As New TableCell
        setCell(c, value)
        If Not cssClass Is Nothing Then
            c.CssClass = cssClass
        End If
        Return c
    End Function

    Private Function makeHeaderCell(ByVal value As Object, Optional ByVal cssClass As String = "valueCell") As TableHeaderCell
        Dim c As New TableHeaderCell
        setCell(c, value)
        If Not cssClass Is Nothing Then
            c.CssClass = cssClass
        End If
        Return c
    End Function

    Private Sub setCell(ByVal c As TableCell, ByVal value As Object)
        If value Is DBNull.Value Then value = nullDisplay
        c.Controls.Clear()
        c.Controls.Add(New LiteralControl(value))
    End Sub

    Private Function getCell(ByVal c As TableCell) As String
        If c.Controls.Count > 0 Then
            Return CType(c.Controls(0), LiteralControl).Text
        End If
        Return ""
    End Function

    Private Function getColIndex(ByVal colname As String) As Integer
        Dim i As Integer = 0
        For Each Str As String In cols()
            If Str.ToLower = colname.ToLower Then Return i
            i += 1
        Next
        Return -1
    End Function

    Private Class hashformat

        Public eval As evalType = evalType.count
        Public columnName As String
        Public columnLabel As String

        Enum evalType
            count = 0
            avg = 1
            sum = 2
            min = 3
            max = 4
        End Enum

    End Class

    Private Function getColumnformats() As Hashtable
        Dim ht As New Hashtable

    End Function

    Private Sub insertTotalRow(ByVal rowList As List(Of TableRow), ByVal groupcol As String)
        If rowList Is Nothing Then Return
        If rowList.Count = 0 Then Return

        Dim r As New TableRow
        r.CssClass = "totalRow"
        For Each col As String In cols()
            Dim index As Integer = getColIndex(col)
            If col = groupcol Then
                Dim totalVal As String = String.Format(TotalFormat, getCell(rowList(rowList.Count - 1).Cells(index)), rowList.Count)
                r.Cells.Add(makeCell(totalVal, "totalCell"))
                'setCell(rowList(0).Cells(index), getCell(rowList(0).Cells(index)) & "#" & rowList.Count)
            Else

                Try
                    Dim tot As Double = 0
                    For Each rw As TableRow In rowList
                        Dim strval As String = getCell(rw.Cells(index))
                        If Not strval = nullDisplay Then
                            tot += strval
                        End If
                    Next
                    r.Cells.Add(makeCell(tot, "totalCell"))
                Catch ex As Exception
                    r.Cells.Add(makeCell("", "totalCell"))
                End Try
            End If
            'r.Cells.Add(makeCell(row(col)))
        Next
        Rows.AddAt(Rows.GetRowIndex(rowList(0)) + 1, r)
        If rowAfterTotal Then
            r = New TableRow
            For Each col As String In cols()
                r.Cells.Add(makeCell("&nbsp;", "spacerCell"))
            Next
            Rows.AddAt(Rows.GetRowIndex(rowList(0)) + 2, r)
        End If

    End Sub

    Private hasbound As Boolean = False
    Private Sub GroupedTable_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DataBinding
        If DataSource IsNot Nothing Then
            Dim colLst As List(Of String) = cols()
            Dim r As TableRow

            'Make the header
            r = New TableHeaderRow
            For Each col As String In colLst
                col = col.Trim()
                r.Cells.Add(makeHeaderCell(col, "headerCell"))
            Next
            Rows.Add(r)

            For Each row As DataRow In DataSource.Rows
                r = New TableRow
                For Each col As String In colLst
                    Try
                        col = col.Trim()
                        r.Cells.Add(makeCell(row(col)))
                    Catch ex As Exception
                        RaiseEvent hadError(ex)
                    End Try
                Next
                Rows.Add(r)
            Next
            Dim c As New Collection

            Dim l As New List(Of TableRow)
            If groupedCol IsNot Nothing AndAlso groupedCol.Length > 0 Then
                For rownum As Integer = Rows.Count - 1 To 1 Step -1
                    Try
                        Dim colnum As Integer = getColIndex(groupedCol)
                        l.Add(Rows(rownum))
                        If getCell(Rows(rownum).Cells(colnum)) = getCell(Rows(rownum - 1).Cells(colnum)) Then
                            If removeRepeatsValue Then
                                setCell(Rows(rownum).Cells(colnum), "")
                            End If
                        Else
                            c.Add(l)
                            'insertTotalRow(l, groupedCol)
                            l = New List(Of TableRow)
                        End If
                    Catch ex As Exception
                        RaiseEvent hadError(ex)
                    End Try
                Next
                c.Add(l)
                For Each lst As List(Of TableRow) In c
                    insertTotalRow(lst, groupedCol)
                Next
                'insertTotalRow(l, groupedCol)
            End If

            If ColumnTitles IsNot Nothing Then
                Dim ndx As Integer = 0
                For Each Str As String In ColumnTitles.Split(",")
                    If (Rows(0).Cells.Count > ndx) Then
                        setCell(Rows(0).Cells(ndx), Str)
                    End If
                    ndx += 1
                Next
            End If
            hasbound = True
        End If
    End Sub

    Private Sub GroupedTable_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not hasbound Then
            Me.DataBind()
        End If
        If Not tableStyle = TableStyles.NotSet Then
            If Me.Rows.Count > 0 Then
                If Me.Rows(0).Cells.Count > 0 Then
                    Me.Rows(0).Cells(0).Controls.Add(New LiteralControl("<link rel='stylesheet' href='~/res/BaseClasses/Scripts.aspx?f=Reporting/" & [Enum].GetName(tableStyle.GetType(), tableStyle) & ".css' />"))
                End If
            End If
        End If
    End Sub



End Class


Public Partial Class ExcellExport
    Inherits BaseClasses.BaseSecurityPage

    Public Property excellHash() As Hashtable
        Get
            If Session("Listoftablesavailableforexcellexport") Is Nothing Then
                Session("Listoftablesavailableforexcellexport") = New Hashtable
            End If
            Return Session("Listoftablesavailableforexcellexport")
        End Get
        Set(ByVal value As Hashtable)
            Session("Listoftablesavailableforexcellexport") = value
        End Set
    End Property



    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '    'If IsPostBack Then
    '    Dim id As String = Request.QueryString("iasdf")

    '    If excellHash.ContainsKey(id) Then

    '        'Dim dg As New DataGrid()
    '        'Me.Controls.Add(dg)
    '        'dg.DataSource = excellHash.Item(id)
    '        'dg.DataBind()

    '        'Me.Page.Response.Clear()
    '        'Me.Page.Response.ContentType = "application/vnd.ms-excel"
    '        'Me.Page.Response.AddHeader("Content-Disposition", "attachment; filename=report.xls")
    '        'Me.Page.Response.Charset = ""
    '        'Me.Page.EnableViewState = False

    '        'Dim oStringWriter As System.IO.StringWriter = New System.IO.StringWriter()
    '        'Dim oHtmlTextWriter As System.Web.UI.HtmlTextWriter = New System.Web.UI.HtmlTextWriter(oStringWriter)
    '        'dg.RenderControl(oHtmlTextWriter)
    '        'Me.Page.Response.Write(oStringWriter.ToString())
    '        'Me.Page.Response.End()
    '        Me.writeexcel(excellHash.Item(id))

    '        Me.Page.Response.Clear()
    '        Me.Page.Response.ContentType = "application/vnd.ms-excel"
    '        Me.Page.Response.AddHeader("Content-Disposition", "inline;filename=report.xls")

    '        Dim dt As DataTable = excellHash.Item(id)
    '        Dim dr As DataRow, ary() As Object, i As Integer
    '        Dim iCol As Integer

    '        'Output Column Headers
    '        For iCol = 0 To dt.Columns.Count - 1
    '            Me.Page.Response.Write(dt.Columns(iCol).ToString & vbTab)
    '        Next
    '        Me.Page.Response.Write(vbCrLf)

    '        'Output Data
    '        For Each dr In dt.Rows
    '            ary = dr.ItemArray
    '            For i = 0 To UBound(ary)
    '                Me.Page.Response.Write(ary(i).ToString & vbTab)
    '            Next
    '            Me.Page.Response.Write(vbCrLf)
    '        Next
    '        Page.Response.End()
    '    End If
    '    'End If
    'End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim id As String = Request.QueryString("iasdf")
        If excellHash.ContainsKey(id) Then
            Dim filename As String = Request.QueryString("filename") & ".xls"
            If filename Is Nothing OrElse filename = "" Then filename = "Document.xls"
            Me.writeexcel(excellHash.Item(id), , True, filename)
            'excellHash.Remove(id)
        End If
    End Sub
End Class
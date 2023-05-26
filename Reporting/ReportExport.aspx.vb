Imports System

Partial Public Class ReportExport
    Inherits ReportEditorBase


    Public dsReportList As New Reporting.dsReports()
    Overloads Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        jQueryLibrary.ThemeAdder.AddThemeToIframe(Me)

        sqlhelper.FillDataTable("Select * from DTIReports", dsReportList.DTIReports)
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        dsReportList.Clear()
        Dim idStr As String = "( -1"
        For Each id As String In tbReportList.Text.Split(",")
            Dim i As Integer = -1
            Integer.TryParse(id, i)
            idStr &= "," & i
        Next
        idStr &= ")"
        sqlHelper.FillDataTable("Select * from DTIReports where id in " & idStr, dsReportList.DTIReports)
        sqlHelper.FillDataTable("Select * from DTIGraphs where report_id in " & idStr, dsReportList.DTIGraphs)
        sqlHelper.FillDataTable("Select * from DTIGraphParms where graph_id in (select id from DTIGraphs where id in " & idStr & " )", dsReportList.DTIGraphParms)
        sqlhelper.FillDataTable("Select * from DTIGraphTypes", dsReportList.DTIGraphTypes)
        sqlhelper.FillDataTable("
 SELECT *
  FROM DTIPropDifferences
  where ObjectKey in (select 'Graph_' + Cast(id as varchar) from DTIGraphs)", dsReportList.DTIPropDifferences)


        Dim filename = "Reports-" & DateTime.Now.ToString("yyyy-dd-M") & ".xml"
        Response.Clear()
        Response.AddHeader("content-disposition", "attachment; filename=" & filename)
        dsReportList.WriteXml(Response.OutputStream)
        Response.End()
    End Sub

End Class
Imports System

Partial Public Class ReportImport
    Inherits ReportEditorBase


    Public ReadOnly Property dsReportList As Reporting.dsReports
        Get
            If Session("ReportingImportDS") Is Nothing Then
                Session("ReportingImportDS") = New Reporting.dsReports
            End If
            Return Session("ReportingImportDS")
        End Get
    End Property
    Public dtExistingReports As New Reporting.dsReports.DTIReportsDataTable


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        sqlhelper.FillDataTable("Select * from DTIReports", dtExistingReports)
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        dsReportList.Clear()
        dsReportList.EnforceConstraints = False

        dsReportList.ReadXml(fuReportsFile.PostedFile.InputStream)
        pnlSelectReports.Visible = True
        pnlUpload.Visible = False
    End Sub

    Protected Sub btnImport_Click(sender As Object, e As EventArgs)

        Dim dsNew As New Reporting.dsReports
        Dim importIDlist As String = tbReportList.Text
        Reporting.Report.loadDSToDatabase(sqlhelper)

        sqlhelper.FillDataTable("Select * from DTIReports", dsNew.DTIReports)
        sqlhelper.FillDataTable("Select * from DTIGraphs", dsNew.DTIGraphs)
        sqlhelper.FillDataTable("Select * from DTIGraphParms", dsNew.DTIGraphParms)
        sqlhelper.FillDataTable("Select * from DTIGraphTypes", dsNew.DTIGraphTypes)

        sqlhelper.FillDataTable("Select * from DTIPropDifferences", dsNew.DTIPropDifferences)
        Dim graphTypesHash As New Hashtable()
        For Each gt As dsReports.DTIGraphTypesRow In dsReportList.DTIGraphTypes
            Dim typeRow As Reporting.dsReports.DTIGraphTypesRow
            If dsNew.DTIGraphTypes.Select("name like '" & EscapeFilter(gt.Name) & "'").Length = 0 Then
                typeRow = dsNew.DTIGraphTypes.AddDTIGraphTypesRow(gt.Control_Name, gt.Name)
                sqlhelper.Update(dsNew.DTIGraphTypes)
            Else
                typeRow = dsNew.DTIGraphTypes.Select("name like '" & EscapeFilter(gt.Name) & "'")(0)
            End If
            graphTypesHash.Add(gt.Id, typeRow.Id)
        Next
        For Each r As dsReports.DTIReportsRow In dsReportList.DTIReports
            If importIDlist.Contains("," & r.Id & ",") Then
                Dim renameRows As DataRow() = dsNew.DTIReports.Select("name like '" & EscapeFilter(r.Name) & "'")
                If renameRows.Length > 0 Then
                    Dim newname As String = r.Name
                    Dim i As Integer = 0

                    While dsNew.DTIReports.Select("name like '" & EscapeFilter(newname) & "'").Length > 0
                        newname = r.Name + " - old"
                        If i > 0 Then newname &= i.ToString()
                        i += 1
                    End While
                    renameRows(0)("name") = newname
                    renameRows(0)("Published") = False
                End If
                Dim newRep = dsNew.DTIReports.AddDTIReportsRow(r.Name, r.Height, r.Width, r.Scrollable, r.showHistory, r.Published)
                sqlhelper.Update(dsNew.DTIReports)
                For Each g As dsReports.DTIGraphsRow In dsReportList.DTIGraphs
                    If g.Report_Id = r.Id Then
                        Dim newg As dsReports.DTIGraphsRow = dsNew.DTIGraphs.AddDTIGraphsRow(newRep.Id, g.Name, g.SelectStmt, g.Order, graphTypesHash(g.Graph_Type), g.Drillable, g.Export)
                        sqlhelper.Update(dsNew.DTIGraphs)
                        For Each p As dsReports.DTIGraphParmsRow In dsReportList.DTIGraphParms
                            If p.Graph_Id = g.Id Then
                                dsNew.DTIGraphParms.AddDTIGraphParmsRow(p.Name, p.DisplayName, p.Parm_Type, newg.Id, p.ParmProperties)
                            End If
                        Next
                        For Each pr As DataRow In dsReportList.DTIPropDifferences
                            If pr("ObjectKey") = "Graph_" & g.Id Then
                                dsNew.DTIPropDifferences.AddDTIPropDifferencesRow(pr("PropertyPath"), pr("PropertyValue"), pr("PropertyType"), "Graph_" & newg.Id, pr("mainID"))
                            End If
                        Next
                    End If
                Next
            End If
        Next
        sqlhelper.Update(dsNew.DTIGraphParms)
        sqlhelper.Update(dsNew.DTIPropDifferences)
        Response.Clear()
        Response.Write("Import Complete.")
        Response.End()
    End Sub

    Public Shared Function EscapeFilter(ByVal value As String) As String
        Dim sb As StringBuilder = New StringBuilder(value.Length)

        For i As Integer = 0 To value.Length - 1
            Dim c As Char = value(i)

            Select Case c
                Case "]"c, "["c, "%"c, "*"c
                    sb.Append("[").Append(c).Append("]")
                Case "'"c
                    sb.Append("''")
                Case Else
                    sb.Append(c)
            End Select
        Next

        Return sb.ToString()
    End Function


End Class
Imports System.Xml
'Imports System.Xml.Linq



Public Class tableData
    Inherits ReportEditorBase

#Region "Properties"

    Private _sql As String = Nothing
    Public ReadOnly Property sql As String
        Get
            If _sql = Nothing Then
                _sql = Server.UrlDecode(Request.Params("sql"))
            End If
            Return _sql
        End Get
    End Property

    Public ReadOnly Property schema As Boolean
        Get
            Return Request.QueryString("schema") = "Y"
        End Get
    End Property

    Public ReadOnly Property relations As Boolean
        Get
            Return Request.QueryString("relations") = "Y"
        End Get
    End Property

    Public ReadOnly Property views As Boolean
        Get
            Return Request.QueryString("views") = "Y"
        End Get
    End Property

    Public ReadOnly Property otherDatabases As Boolean
        Get
            Return Request.QueryString("otherDB") = "Y"
        End Get
    End Property

    Public ReadOnly Property otherDatabaseName As String
        Get
            Return Request.QueryString("otherDBname")
        End Get
    End Property

    Public ReadOnly Property helpername As String
        Get
            Return helper.GetType.Name.ToLower()
        End Get
    End Property

    Private _helper As BaseClasses.BaseHelper
    Public ReadOnly Property helper() As BaseClasses.BaseHelper
        Get
            If Not Session("ReportDataConnection") Is Nothing Then
                If _helper Is Nothing Then
                    _helper = BaseClasses.DataBase.createHelper(Session("ReportDataConnection"))
                End If
                Return _helper
            Else
                Return sqlHelper
            End If
        End Get
    End Property

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Clear()
		'Try

		If sql IsNot Nothing Then
            Dim sqltop As String = sql.Substring(6)
            sqltop = "select top 10 " & sqltop
            If helpername.StartsWith("sqlhelper") Then
                sqltop = sqltop & " OPTION (MAXDOP 1) "
            End If
            Try
                Response.Write(getJsonTableData(helper.FillDataTable(sqltop)))
            Catch ex As Exception
                Response.End()
            End Try

        End If
        If schema Then
            Response.Write(getJsonSchema(tableList()))
        End If
        If views Then
            Response.Write(getJsonSchema(viewList()))
        End If
        If otherDatabases Then
            If otherDatabaseName Is Nothing Then
                Response.Write(getJsonTableData(databaseList()))
            Else
                Response.Write(getJsonSchema(tableList()))
            End If
        End If

        Response.End()
    End Sub

    Public ds As New DataSet
    Public Function getJsonSchema(tableList As DataTable) As String
        Dim schema As String = "{"
        Dim hasSchemaName As Boolean = tableList.Columns.Contains("sch")
        For Each tableNameRow As DataRow In tableList.Rows
            Dim tablename As String = tableNameRow(0)
            Dim schemaName As String = "dbo"
            If hasSchemaName Then schemaName = tableNameRow("sch")
            If Not tablename.StartsWith("DTI") AndAlso Not tablename.ToLower() = "sysdiagrams" Then
                If otherDatabaseName IsNot Nothing Then
                    tablename = otherDatabaseName & "." & schemaName & "." & tablename
                Else
                    If schemaName <> "dbo" Then
                        tablename = schemaName & "." & tablename
                    End If
                End If

                Dim dt As DataTable = ds.Tables.Add()
                Try
                    helper.FillDataTable("select top 1 * from " & tablename, dt)
                Catch ex As Exception
                    If tablename.Contains(".") Then
                        Dim ttmp = ""
                        For Each sec As String In tablename.Split(".")
                            ttmp &= "[" & sec & "]."
                        Next
                        tablename = ttmp.Trim(".")
                    Else
                        tablename = "[" & tablename & "]"
                    End If
                    Try
                        helper.FillDataTable("select top 1 * from " & tablename, dt)
                    Catch ex3 As Exception

                    End Try

                End Try
                Try
                    helper.Adaptor(dt.TableName).FillSchema(dt, SchemaType.Mapped)
                Catch ex As Exception
                    Try
                        helper.Adaptor(tablename).FillSchema(dt, SchemaType.Mapped)
                    Catch ex2 As Exception

                    End Try
                End Try

                dt.TableName = tablename
                'ds.Tables.Add(dt)
            End If
        Next
        For Each dt As DataTable In ds.Tables
            schema &= vbCrLf & getJsonTableSchema(dt) & ","
        Next
        Return schema.Trim(",") & vbCrLf & "}"
    End Function

#Region "Database Calls"

    Public Function tableList() As DataTable
        If Request.QueryString("tablelist") <> "" Then
            Dim dt As New DataTable
            dt.Columns.Add("name")
            For Each tblname As String In Request.QueryString("tablelist").Split("##")
                If tblname.Trim() <> "" Then
                    dt.Rows.Add(New Object() {tblname})
                End If
            Next
            Return dt
        End If
        If helpername.StartsWith("sqlite") Then
            Return helper.FillDataTable( _
            "SELECT name FROM  " & vbCrLf & _
            "   (SELECT * FROM sqlite_master UNION ALL " & vbCrLf & _
            "    SELECT * FROM sqlite_temp_master) " & vbCrLf & _
            "WHERE type='table' " & vbCrLf & _
            "ORDER BY name " & vbCrLf)
        ElseIf helpername.StartsWith("sqlhelper") Then
            Dim sql As String = "SELECT table_name as name, table_schema as sch FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' order by table_name"
            If otherDatabaseName IsNot Nothing Then sql = "use [" & otherDatabaseName & "] " & sql
            Return helper.FillDataTable(sql)
        ElseIf helpername.StartsWith("mysql") Then
            Return helper.FillDataTable("show tables")
        End If
        Return Nothing
    End Function

    Public Function viewList() As DataTable
        If helpername.StartsWith("sqlite") Then
			Return helper.FillDataTable(
			"SELECT name FROM  " & vbCrLf &
			"   (SELECT * FROM sqlite_master UNION ALL " & vbCrLf &
			"    SELECT * FROM sqlite_temp_master) " & vbCrLf &
			"WHERE type='view' " & vbCrLf &
			"ORDER BY name " & vbCrLf)
		ElseIf helpername.StartsWith("sqlhelper") Then
            Dim sql As String = "SELECT table_name as name, table_schema as sch FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='VIEW' order by table_name"
            If otherDatabaseName IsNot Nothing Then sql = "use [" & otherDatabaseName & "] " & sql
            Return helper.FillDataTable(sql)
        ElseIf helpername.StartsWith("mysql") Then
            Return helper.FillDataTable("show tables")
        End If
        Return Nothing
    End Function

    Public Function databaseList() As DataTable
        If helpername.StartsWith("sqlite") Then
            Return New DataTable
        ElseIf helpername.StartsWith("sqlhelper") Then
            Return helper.FillDataTable("SELECT name FROM master.dbo.sysdatabases order by name")
        ElseIf helpername.StartsWith("mysql") Then
            Return helper.FillDataTable("show databases")
        End If
        Return Nothing
    End Function

    Public Function relationList() As DataTable
        Dim sql As String = ""
        If helpername.StartsWith("sqlite") Then
			Dim tables As DataTable = tableList()
			Dim outTbl As New DataTable
			outTbl.Columns.Add("FK_Table")
			outTbl.Columns.Add("FK_Column")
			outTbl.Columns.Add("PK_Table")
			outTbl.Columns.Add("PK_Column")
			outTbl.Columns.Add("Constraint_Name")
			outTbl.Columns.Add("Constraint_schema")
			outTbl.Columns.Add("Ordinal_position")

			For Each r As DataRow In tables.Rows
				Dim tblName As String = r("name")
				Dim constraints As DataTable = helper.FillDataTable("PRAGMA foreign_key_list( " & tblName & " )")
				Dim c As Integer = 0
				For Each constRow As DataRow In constraints.Rows
					outTbl.Rows.Add({tblName, constRow("to"), constRow("table"), constRow("from"), "fk_" & tblName & "_" & c, "", c})
					c += 1
				Next
			Next
			Return outTbl
		ElseIf helpername.StartsWith("sqlhelper") Then
            sql = "				SELECT " & vbCrLf & _
"				FK_Table = FK.TABLE_NAME, " & vbCrLf & _
"                FK_Column = CU.COLUMN_NAME, " & vbCrLf & _
"                PK_Table = PK.TABLE_NAME, " & vbCrLf & _
"                PK_Column = CUPK.COLUMN_NAME, " & vbCrLf & _
"                Constraint_Name = C.CONSTRAINT_NAME,  " & vbCrLf & _
"                Constraint_schema = c.Constraint_schema, " & vbCrLf & _
"				Ordinal_position = CU.Ordinal_position " & vbCrLf & _
"				FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS c  " & vbCrLf & _
"				INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME " & vbCrLf & _
"				INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME " & vbCrLf & _
"				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME " & vbCrLf & _
"				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CUPK ON PK.CONSTRAINT_NAME = CUPK.CONSTRAINT_NAME AND CUPK.Ordinal_position = CU.Ordinal_position " & vbCrLf & _
"				order by c.CONSTRAINT_NAME,FK_Table, Ordinal_position " & vbCrLf
            If otherDatabaseName IsNot Nothing Then sql = "use [" & otherDatabaseName & "] " & sql
            Dim dt As DataTable = helper.FillDataTable(sql)
            If otherDatabaseName IsNot Nothing Then
                For Each r As DataRow In dt.Rows
                    r("FK_Table") = otherDatabaseName & "." & r("Constraint_schema") & "." & r("FK_Table")
                    r("PK_Table") = otherDatabaseName & "." & r("Constraint_schema") & "." & r("PK_Table")
                Next
            End If
            Return dt
        ElseIf helpername.StartsWith("mysql") Then

        End If
        Return Nothing
    End Function

#End Region

    'This region could have been done with newtonsoft or some other object->json formatter, but I established the json format first and didn't want to re-create it.
#Region "Json formatters"

    Public Function getJsonColumns(dt As DataTable) As String
        Dim colstr As String = "		""columns"":["
        Dim ordlist As New List(Of String)
        For Each col As DataColumn In dt.Columns
            ordlist.Add(col.ColumnName)
        Next
        'ordlist.Sort()
        For Each colname As String In ordlist
            Dim col As DataColumn = dt.Columns(colname)
            colstr &= vbCrLf & "			{""name"":""" & col.ColumnName & """,""type"":""" & getTypeStr(col.DataType) & """},"
        Next
        colstr = colstr.Trim(",")
        colstr &= vbCrLf & "		]"
        Return colstr
    End Function

    Public Function getTypeStr(t As Type) As String
        Dim str As String = t.ToString().ToLower().Replace("system.", "")
        If str.StartsWith("int") Then str = "int"
        If str.StartsWith("date") Then str = "date"
        If str.StartsWith("deci") Then str = "float"
        Return str
    End Function

    Public Function getJsonRows(dt As DataTable) As String
        Dim rows As String = "		""rows"":["
        For Each row As DataRow In dt.Rows
            rows &= "            {"
            For Each col As DataColumn In dt.Columns
                rows &= vbCrLf & """" & col.ColumnName & """:""" & getRowValue(row, col.ColumnName) & ""","
            Next
            rows = rows.Trim(",")
            rows &= "},"
        Next
        rows = rows.Trim(",")
        rows &= vbCrLf & "		]"
        Return rows
    End Function

    Public Function getRowValue(row As DataRow, col As String) As String
        Dim val As String = ""
        Try
            If Not row(col) Is DBNull.Value Then
                val = row(col).ToString()
                val = val.Replace("'", "###")
                val = JavaScriptEncode(val).Replace("###", "'")
            End If
        Catch ex As Exception

        End Try
        Return val
    End Function

    Public Function getJsonPKs(dt As DataTable) As String
        Dim keys As String = "		""keys"":["
        For Each col As DataColumn In dt.PrimaryKey()
            keys &= """" & col.ColumnName & ""","
        Next
        keys = keys.Trim(",")
        keys &= "]"
        Return keys
    End Function

    Public Function getJsonTableData(dt As DataTable) As String
        Return "{" & vbCrLf & getJsonColumns(dt) & "," & vbCrLf & getJsonRows(dt) & vbCrLf & "}"
    End Function


    Private rels As DataTable
    Public Function getJsonRelations(tableName As String) As String
        Dim schema As String = "" '"{"
        If rels Is Nothing Then rels = relationList()
        schema &= "		""relations"":["
        Dim relString = "         {3}""table"":""{0}"",""child_cols"":[{1}],""parent_cols"":[{2}]{4},"
		Dim q = """"
		tableName = tableName.Trim({"["c, "]"c})
		Dim dv As New DataView(rels, "FK_Table = '" & tableName & "' OR FK_Table = '[" & tableName & "]'", "", DataViewRowState.CurrentRows)
        Dim relationName = ""
        Dim FKCols = ""
        Dim PKCols = ""
        Dim pkTable = ""
        For Each rel As DataRowView In dv
            If relationName <> rel("CONSTRAINT_NAME") And relationName <> "" Then
                schema &= vbCrLf & String.Format(relString, pkTable, FKCols.Trim(","), PKCols.Trim(","), "{", "}")
                FKCols = ""
                PKCols = ""
            End If
            pkTable = rel("PK_Table")
            If ds.Tables(pkTable) Is Nothing Then
                pkTable = "[" & pkTable & "]"
            End If
            relationName = rel("CONSTRAINT_NAME")
            FKCols &= q & rel("FK_Column") & q & ","
            PKCols &= q & rel("PK_Column") & q & ","

        Next
        If relationName <> "" Then _
            schema &= vbCrLf & String.Format(relString, pkTable, FKCols.Trim(","), PKCols.Trim(","), "{", "}")
        schema = schema.Trim(",")
        If dv.Count > 0 Then schema &= vbCrLf & "		"
        Return schema & "]"
    End Function

    Public Function getJsonTableSchema(dt As DataTable) As String
        Dim tblString As String = ""
        tblString &= "	""" & dt.TableName & """: {"
        tblString &= vbCrLf & "		""description"": """ & dt.DisplayExpression & ""","
        tblString &= vbCrLf & getJsonColumns(dt) & ","
        tblString &= vbCrLf & getJsonPKs(dt) & ","
        tblString &= vbCrLf & getJsonRelations(dt.TableName)
        tblString = tblString.Trim(",") & vbCrLf & "	}"

        Return tblString
    End Function

#End Region



End Class
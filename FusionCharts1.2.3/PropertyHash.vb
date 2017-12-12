Public Class PropertyHash



    Private _propht As New Hashtable
    Public Property xmlprop(ByVal propname As String) As String
        Get
            Return _propht(propname)
        End Get
        Set(ByVal value As String)
            _propht(propname) = value
        End Set
    End Property

    Public Function containsKey(ByVal keyname As String) As Boolean
        Return _propht.ContainsKey(keyname)
    End Function

    Public Sub remove(ByVal propname As String)
        _propht.Remove(propname)
    End Sub

    Public Property xmlBoolProp(ByVal propname As String) As Boolean
        Get
            If xmlprop(propname) Is Nothing Then Return False
            If xmlprop(propname) = "1" Then Return True
            Return False
        End Get
        Set(ByVal value As Boolean)
            If value Then
                xmlprop(propname) = "1"
            Else
                xmlprop(propname) = "0"
            End If
        End Set
    End Property

    Public Property xmlColorProperty(ByVal propname As String) As Drawing.Color
        Get
            Return getColor(xmlprop(propname))
        End Get
        Set(value As Drawing.Color)
            If value = Nothing Then
                remove(propname)
            Else
                xmlprop(propname) = getColorString(value)
            End If
        End Set
    End Property

    Public Property xmlBoolDefault(ByVal propname As String) As ChartBase.BooleanDefault
        Get
            If xmlprop(propname) Is Nothing Then Return ChartBase.BooleanDefault.Default
            If xmlprop(propname) = "1" Then Return ChartBase.BooleanDefault.Yes
            Return ChartBase.BooleanDefault.No
        End Get
        Set(ByVal value As ChartBase.BooleanDefault)
            If value = ChartBase.BooleanDefault.Yes Then
                xmlprop(propname) = "1"
            ElseIf value = ChartBase.BooleanDefault.No Then
                xmlprop(propname) = "0"
            End If
            If value = ChartBase.BooleanDefault.Default Then
                remove(propname)
            End If
        End Set
    End Property

    Public Property xmlpropString() As String
        Get
            Dim outstr As String = ""
            For Each key As String In _propht.Keys
                outstr &= key & "='" & _propht(key) & "' "
            Next
            Return outstr
        End Get
        Set(ByVal value As String)
            If value Is Nothing OrElse value.Trim = "" Then
            Else
                value = value & " "
                _propht.Clear()
                For Each val As String In value.Split("' ")
                    Dim vals() As String = val.Split("='")
                    If vals.Length > 1 Then
                        _propht.Add(vals(0), vals(1))
                    End If
                Next
            End If
        End Set
    End Property

    Private _tag As String = "dataset"
    Public Property tag() As String
        Get
            Return _tag
        End Get
        Set(ByVal value As String)
            _tag = value
        End Set
    End Property

    Public Function getColor(color As String) As Drawing.Color
        Try
            Return System.Drawing.ColorTranslator.FromHtml(color)
        Catch ex As Exception

        End Try
        Return Nothing
    End Function

    Public Function getColorString(color As Drawing.Color) As String
        If color = Nothing Then Return ""
        Return String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B)
    End Function

    Public Overridable Function outputXML(ByVal innerText As String) As String
        Dim out As String = "<" & tag & " " & xmlpropString & " >" & innerText & "</" & tag & ">"
        Return out
    End Function

    Public Overridable Function outputsingleTagXML() As String
        Dim out As String = "<" & tag & " " & xmlpropString & " />"
        Return out
    End Function

    Public Sub New(Optional ByVal tagname As String = Nothing)
        If Not tagname Is Nothing Then tag = tagname
    End Sub

End Class


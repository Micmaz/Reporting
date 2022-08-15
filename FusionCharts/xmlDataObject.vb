
Public Class xmlDataObject

    Public props As New PropertyHash()

    Public Property tag() As String
        Get
            Return props.tag
        End Get
        Set(ByVal value As String)
            props.tag = value
        End Set
    End Property

    Public Property xmlprop(ByVal propname As String) As String
        Get
            Return props.xmlprop(propname)
        End Get
        Set(ByVal value As String)
            props.xmlprop(propname) = value
        End Set
    End Property

    Public Property xmlBoolProp(ByVal propname As String) As Boolean
        Get
            Return props.xmlBoolProp(propname)
        End Get
        Set(ByVal value As Boolean)
            props.xmlBoolProp(propname) = value
        End Set
    End Property

End Class
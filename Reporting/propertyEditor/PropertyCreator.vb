Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text

Public Class PropertyCreator
    Private Const PropertyNameSeparator As Char = "."
    Private Const ListItemBeginSeperator As Char = "("
    Private Const ListItemEndSeperator As Char = ")"

    Private ReadOnly NoParams As Object() = New Object(-1) {}
    Private ReadOnly NoTypeParams As Type() = New Type(-1) {}

    Private propertyCache As IDictionary(Of Type, PropertyInfoCache) = New Dictionary(Of Type, PropertyInfoCache)()
    Private constructorCache As IDictionary(Of Type, ConstructorInfo) = New Dictionary(Of Type, ConstructorInfo)()

    ''' <summary> 
    ''' Gets the Type of the given property of the given targetType. 
    ''' The targetType and propertyName parameters can't be null. 
    ''' </summary> 
    ''' <param name="targetType">the target type which contains the property</param> 
    ''' <param name="propertyName">the property to get, can be a property on a nested object (eg. "Child.Name")</param>
    <System.ComponentModel.Description("Gets the Type of the given property of the given targetType.    The targetType and propertyName parameters can't be null.")> _
    Public Function GetTargetType(ByVal targetType As Type, ByVal propertyName As String) As Type
        If propertyName.IndexOf(PropertyNameSeparator) > -1 Then
            Dim propertyList As String() = propertyName.Split(PropertyNameSeparator)
            For i As Integer = 0 To propertyList.Length - 1
                Dim currentProperty As String = propertyList(i)
                targetType = GetTypeImpl(targetType, currentProperty)
            Next
            Return targetType
        Else
            Return GetTypeImpl(targetType, propertyName)
        End If
    End Function

    ''' <summary> 
    ''' Gets the value of the given property of the given target. 
    ''' If objects within the property hierarchy are null references, null will be returned. 
    ''' The target and propertyName parameters can't be null. 
    ''' </summary> 
    ''' <param name="target">the target object to get the value from</param> 
    ''' <param name="propertyName">the property to get, can be a property on a nested object (eg. "Child.Name")</param>
    <System.ComponentModel.Description("Gets the value of the given property of the given target.    If objects within the property hierarchy are null references, null will be returned.    The target and propertyName parameters can't be null.")> _
    Public Function GetValue(ByVal target As Object, ByVal propertyName As String) As Object
        If propertyName.IndexOf(PropertyNameSeparator) > -1 Then
            Dim propertyList As String() = propertyName.Split(PropertyNameSeparator)
            For i As Integer = 0 To propertyList.Length - 1
                Dim currentProperty As String = propertyList(i)
                target = GetValueImpl(target, currentProperty)
                If target Is Nothing Then
                    Return Nothing
                End If
            Next
            Return target
        Else
            Return GetValueImpl(target, propertyName)
        End If
    End Function

    ''' <summary> 
    ''' Sets the value of the given property on the given target to the given value. 
    ''' If objects within the property hierarchy are null references, an attempt will be 
    ''' made to construct a new instance through a parameterless constructor. 
    ''' The target and propertyName parameters can't be null. 
    ''' </summary> 
    ''' <param name="target">the target object to set the value on</param> 
    ''' <param name="propertyName">the property to set, can be a property on a nested object (eg. "Child.Name")</param>
    ''' <param name="value">the new value of the property</param>
    <System.ComponentModel.Description("Sets the value of the given property on the given target to the given value.    If objects within the property hierarchy are null references, an attempt will be    made to construct a new instance through a parameterless constructor.    The target and propertyName parameters can't be null.")> _
    Public Sub SetValue(ByVal target As Object, ByVal propertyName As String, ByVal value As Object)
        If propertyName.IndexOf(PropertyNameSeparator) > -1 Then
            Dim originalTarget As Object = target
            Dim propertyList As String() = propertyName.Split(PropertyNameSeparator)
            For i As Integer = 0 To propertyList.Length - 2
                propertyName = propertyList(i)
                target = GetValueImpl(target, propertyName)
                If target Is Nothing Then
                    Dim currentFullPropertyNameString As String = GetPropertyNameString(propertyList, i)
                    target = Construct(GetTargetType(originalTarget.[GetType](), currentFullPropertyNameString))
                    SetValue(originalTarget, currentFullPropertyNameString, target)
                End If
            Next
            propertyName = propertyList(propertyList.Length - 1)
        End If
        SetValueImpl(target, propertyName, value)
    End Sub

    ''' <summary> 
    ''' Returns a string containing the properties in the propertyList up to the given 
    ''' level, separated by dots. 
    ''' For the propertyList { "Zero", "One", "Two" } and level 1, the string 
    ''' "Zero.One" will be returned. 
    ''' </summary> 
    ''' <param name="propertyList">the array containing the properties in the corect order</param> 
    ''' <param name="level">the level up to wich to include the properties in the returned string</param> 
    ''' <returns>a dot-separated string containing the properties up to the given level</returns>
    <System.ComponentModel.Description("Returns a string containing the properties in the propertyList up to the given    level, separated by dots.    For the propertyList { ""Zero"", ""One"", ""Two"" } and level 1, the string    ""Zero.One"" will be returned.")> _
    Private Shared Function GetPropertyNameString(ByVal propertyList As String(), ByVal level As Integer) As String
        Dim currentFullPropertyName As New StringBuilder()
        For j As Integer = 0 To level
            If j > 0 Then
                currentFullPropertyName.Append(PropertyNameSeparator)
            End If
            currentFullPropertyName.Append(propertyList(j))
        Next
        Return currentFullPropertyName.ToString()
    End Function

    ''' <summary> 
    ''' Returns the type of the given property on the target instance. 
    ''' The type and propertyName parameters can't be null. 
    ''' </summary> 
    ''' <param name="targetType">the type of the target instance</param> 
    ''' <param name="propertyName">the property to retrieve the type for</param> 
    ''' <returns>the typr of the given property on the target type</returns>
    <System.ComponentModel.Description("Returns the type of the given property on the target instance.    The type and propertyName parameters can't be null.")> _
    Private Function GetTypeImpl(ByVal targetType As Type, ByVal propertyName As String) As Type
        Return GetPropertyInfo(targetType, propertyName).PropertyType
    End Function

    ''' <summary> 
    ''' Returns the value of the given property on the target instance. 
    ''' The target instance and propertyName parameters can't be null. 
    ''' </summary> 
    ''' <param name="target">the instance on which to get the value</param> 
    ''' <param name="propertyName">the property for which to get the value</param> 
    ''' <returns>the value of the given property on the target instance</returns>
    <System.ComponentModel.Description("Returns the value of the given property on the target instance.    The target instance and propertyName parameters can't be null.")> _
    Private Function GetValueImpl(ByVal target As Object, ByVal propertyName As String) As Object
        If propertyName.Contains(ListItemBeginSeperator) Then
            propertyName = propertyName.Trim(ListItemEndSeperator)
            Dim idx As Integer = propertyName.Substring(propertyName.IndexOf(ListItemBeginSeperator) + 1)
            Return target(idx)
        Else
            Return GetPropertyInfo(target.[GetType](), propertyName).GetValue(target, NoParams)
        End If
    End Function

    ''' <summary> 
    ''' Sets the given property of the target instance to the given value. 
    ''' Type mismatches in the parameters of these methods will result in an exception. 
    ''' Also, the target instance and propertyName parameters can't be null. 
    ''' </summary> 
    ''' <param name="target">the instance to set the value on</param> 
    ''' <param name="propertyName">the property to set the value on</param> 
    ''' <param name="value">the value to set on the target</param>
    <System.ComponentModel.Description("Sets the given property of the target instance to the given value.    Type mismatches in the parameters of these methods will result in an exception.    Also, the target instance and propertyName parameters can't be null.")> _
    Private Sub SetValueImpl(ByVal target As Object, ByVal propertyName As String, ByVal value As Object)
        GetPropertyInfo(target.[GetType](), propertyName).SetValue(target, value, NoParams)
    End Sub

    ''' <summary> 
    ''' Obtains the PropertyInfo for the given propertyName of the given type from the cache. 
    ''' If it is not already in the cache, the PropertyInfo will be looked up and added to 
    ''' the cache. 
    ''' </summary> 
    ''' <param name="type">the type to resolve the property on</param> 
    ''' <param name="propertyName">the name of the property to return the PropertyInfo for</param> 
    ''' <returns></returns>
    <System.ComponentModel.Description("Obtains the PropertyInfo for the given propertyName of the given type from the cache.    If it is not already in the cache, the PropertyInfo will be looked up and added to    the cache.")> _
    Private Function GetPropertyInfo(ByVal type As Type, ByVal propertyName As String) As PropertyInfo
        Dim propertyInfoCache As PropertyInfoCache = GetPropertyInfoCache(type)
        If Not propertyInfoCache.ContainsKey(propertyName) Then
            Dim propertyInfo As PropertyInfo = GetBestMatchingProperty(propertyName, type)
            If propertyInfo Is Nothing Then

                Throw New ArgumentException(String.Format("Unable to find public property named {0} on type {1}", propertyName, type.FullName), propertyName)
            End If
            propertyInfoCache.Add(propertyName, propertyInfo)
        End If
        Return propertyInfoCache(propertyName)
    End Function

    ''' <summary> 
    ''' Gets the best matching property info for the given name on the given type if the same property is defined on 
    ''' multiple levels in the object hierarchy. 
    ''' </summary>
    <System.ComponentModel.Description("Gets the best matching property info for the given name on the given type if the same property is defined on    multiple levels in the object hierarchy.")> _
    Private Shared Function GetBestMatchingProperty(ByVal propertyName As String, ByVal type As Type) As PropertyInfo
        Dim propertyInfos As PropertyInfo() = type.GetProperties(BindingFlags.[Public] Or BindingFlags.Instance Or BindingFlags.FlattenHierarchy)

        Dim bestMatch As PropertyInfo = Nothing
        Dim bestMatchDistance As Integer = Integer.MaxValue
        For i As Integer = 0 To propertyInfos.Length - 1
            Dim info As PropertyInfo = propertyInfos(i)
            If info.Name = propertyName Then
                Dim distance As Integer = CalculateDistance(type, info.DeclaringType)
                If distance = 0 Then
                    ' as close as we're gonna get... 
                    Return info
                End If
                If distance > 0 AndAlso distance < bestMatchDistance Then
                    bestMatch = info
                    bestMatchDistance = distance
                End If
            End If
        Next
        Return bestMatch
    End Function

    ''' <summary> 
    ''' Calculates the hierarchy levels between two classes. 
    ''' If the targetObjectType is the same as the baseType, the returned distance will be 0. 
    ''' If the two types do not belong to the same hierarchy, -1 will be returned. 
    ''' </summary>
    <System.ComponentModel.Description("Calculates the hierarchy levels between two classes.    If the targetObjectType is the same as the baseType, the returned distance will be 0.    If the two types do not belong to the same hierarchy, -1 will be returned.")> _
    Private Shared Function CalculateDistance(ByVal targetObjectType As Type, ByVal baseType As Type) As Integer
        If Not baseType.IsInterface Then
            Dim currType As Type = targetObjectType
            Dim level As Integer = 0
            While currType IsNot Nothing
                If baseType Is currType Then
                    Return level
                End If
                currType = currType.BaseType
                level += 1
            End While
        End If
        Return -1
    End Function

    ''' <summary> 
    ''' Returns the PropertyInfoCache for the given type. 
    ''' If there isn't one available already, a new one will be created. 
    ''' </summary> 
    ''' <param name="type">the type to retrieve the PropertyInfoCache for</param> 
    ''' <returns>the PropertyInfoCache for the given type</returns>
    <System.ComponentModel.Description("Returns the PropertyInfoCache for the given type.    If there isn't one available already, a new one will be created.")> _
    Private Function GetPropertyInfoCache(ByVal type As Type) As PropertyInfoCache
        If Not propertyCache.ContainsKey(type) Then
            SyncLock Me
                If Not propertyCache.ContainsKey(type) Then
                    propertyCache.Add(type, New PropertyInfoCache())
                End If
            End SyncLock
        End If
        Return propertyCache(type)
    End Function

    ''' <summary> 
    ''' Creates a new object of the given type, provided that the type has a default (parameterless) 
    ''' constructor. If it does not have such a constructor, an exception will be thrown. 
    ''' </summary> 
    ''' <param name="type">the type of the object to construct</param> 
    ''' <returns>a new instance of the given type</returns>
    <System.ComponentModel.Description("Creates a new object of the given type, provided that the type has a default (parameterless)    constructor. If it does not have such a constructor, an exception will be thrown.")> _
    Private Function Construct(ByVal type As Type) As Object
        If Not constructorCache.ContainsKey(type) Then
            SyncLock Me
                If Not constructorCache.ContainsKey(type) Then
                    Dim constructorInfo As ConstructorInfo = type.GetConstructor(NoTypeParams)
                    If constructorInfo Is Nothing Then

                        Throw New Exception(String.Format("Unable to construct instance, no parameterless constructor found in type {0}", type.FullName))
                    End If
                    constructorCache.Add(type, constructorInfo)
                End If
            End SyncLock
        End If
        Return constructorCache(type).Invoke(NoParams)
    End Function
End Class

''' <summary> 
''' Keeps a mapping between a string and a PropertyInfo instance. 
''' Simply wraps an IDictionary and exposes the relevant operations. 
''' Putting all this in a separate class makes the calling code more 
''' readable. 
''' </summary> 
Friend Class PropertyInfoCache
    Private propertyInfoCache As IDictionary(Of String, PropertyInfo)

    Public Sub New()
        propertyInfoCache = New Dictionary(Of String, PropertyInfo)()
    End Sub

    Public Function ContainsKey(ByVal key As String) As Boolean
        Return propertyInfoCache.ContainsKey(key)
    End Function

    Public Sub Add(ByVal key As String, ByVal value As PropertyInfo)
        propertyInfoCache.Add(key, value)
    End Sub

    Default Public Property Item(ByVal key As String) As PropertyInfo
        Get
            Return propertyInfoCache(key)
        End Get
        Set(ByVal value As PropertyInfo)
            propertyInfoCache(key) = value
        End Set
    End Property
End Class


Imports System
Imports System.Reflection
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Web.UI
Imports System.Web
Imports System.Web.Caching
Imports System.Web.UI.WebControls
Imports System.Text.RegularExpressions
Imports System.Web.UI.Design
Imports PropertiesEditorBase


<ToolboxData("<{0}:PropertiesEditor runat=server></{0}:PropertiesEditor>")> _
<DefaultProperty("xViewOnlyB")> _
<Designer(GetType(PropertiesEditorDesigner))> _
Public Class PropertiesGrid
    Inherits System.Web.UI.WebControls.Table
    Implements INamingContainer
    Public mainID As Integer = 0
    Public expandedObject As Boolean = False

    Public Event toggleSubProp(ByVal propPath As String, ByVal isexpanded As Boolean)

    Public Sub onDoExpandSubProp(ByVal proppath As String, ByVal isexpanded As Boolean)
        RaiseEvent toggleSubProp(proppath, isexpanded)
    End Sub


#Region "Properties"

    Private fFinalNameFilter As String = ""
    ' The actual name filters used. Takes xNameFilter + any auto filters.       
    Private fFinalTypeFilter As String = ""
    ' The actual type filters used. Takes xTypeFilter + any auto filters. 
    Private fChildTableRows As New Collections.Generic.List(Of PropertyTableRow)

    Private _instancename As String = Nothing
    <Category("Data")> _
    Public Property xInstanceName() As String
        Get
            If _instancename Is Nothing Then _instancename = xControlToView
            Return _instancename
        End Get
        Set(ByVal value As String)
            _instancename = value
        End Set
    End Property

    <Browsable(False)> _
    <Category("Data")> _
    Public Property xInstance() As Object
        Get
            Return fInstance
        End Get
        Set(ByVal value As Object)
            fInstance = value
        End Set
    End Property
    Private fInstance As Object = Nothing
    <Category("Data")> _
    Public Property xControlToView() As String
        Get
            If fControlToView = "" Then Return objectKey
            Return fControlToView
        End Get
        Set(ByVal value As String)
            fControlToView = value
        End Set
    End Property

    Private _objectKey As String = ""
    <Category("Data")> _
    Public Property objectKey() As String
        Get
            If _objectKey Is Nothing Then Return xInstanceName
            Return _objectKey
        End Get
        Set(ByVal value As String)
            _objectKey = value
        End Set
    End Property

    Private fControlToView As String = ""

    ''' <summary>
    ''' xShowNonPublicB determines if non-public properties are shown.
    ''' When true, show public and non public properties.
    ''' When false, show public.
    ''' Default is false.
    ''' </summary>
    <Browsable(True)> _
    <Description("xShowNonPublicB determines if non-public properties are shown.   When true, show public and non public properties.   When false, show public.   Default is false.")> _
    <Category("Behavior")> _
    <DefaultValue(False)> _
    Public Property xShowNonPublicB() As Boolean
        Get
            If ViewState("ShowNonPublic") Is Nothing Then
                Return False
            Else
                Return CBool(ViewState("ShowNonPublic"))
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowNonPublic") = value
        End Set
    End Property

    ''' <summary>
    ''' xCacheMinutes determines how long to cache the property lists associated with each class.
    ''' PropertyEditor caches every property list for speed since they don't change often.
    ''' If you are frequently changing your classes, you may want to turn this off.
    ''' Set to 0 to turn off. It will clear any cached classes when this is 0.
    ''' Otherwise, specify the number of minutes to maintain in a cache.
    ''' </summary>
    <Description("xCacheMinutes determines how long to cache the property lists associated with each class.   PropertyEditor caches every property list for speed since they don't change often.   If you are frequently changing your classes, you may want to turn this off.   Set to 0 to turn off. It will clear any cached classes when this is 0.   Otherwise, specify the number of minutes to maintain in a cache.")> _
    <Browsable(True)> _
    <Category("Data")> _
    <DefaultValue(60)> _
    Public Property xCacheMinutes() As Integer
        Get
            If ViewState("CacheMinutes") Is Nothing Then
                Return 60
            Else
                Return CInt(ViewState("CacheMinutes"))
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("CacheMinutes") = value
        End Set
    End Property

    <Description("Determines if properties can be edited.")> _
    <Browsable(True)> _
    <Category("Appearance")> _
    <DefaultValue(False)> _
    Public Property xViewOnlyB() As Boolean
        Get
            If ViewState("ViewOnly") Is Nothing Then
                Return False
            Else
                Return CBool(ViewState("ViewOnly"))
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("ViewOnly") = value
        End Set
    End Property

    <Description("Show read only properties.")> _
    <Browsable(True)> _
    <Category("Behavior")> _
    <DefaultValue(False)> _
    Public Property xShowReadOnlyB() As Boolean
        Get
            If ViewState("ShowReadOnly") Is Nothing Then
                Return False
            Else
                Return CBool(ViewState("ShowReadOnly"))
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowReadOnly") = value
        End Set
    End Property

    ''' <summary>
    ''' xNameFilter identifies the names of any properties that you want to omit.
    ''' Names do not include the parent names (so no periods in the name list). 
    ''' Names must be delimited with spaces. Matches are case insensitive.
    ''' <example>"BackColor Page AccessKey"</example>
    ''' </summary>
    <Description("xNameFilter identifies the names of any properties that you want to omit.   Names do not include the parent names (so no periods in the name list).    Names must be delimited with spaces. Matches are case insensitive.   <example>""BackColor Page AccessKey""</example>")> _
    <Browsable(True)> _
    <Category("Behavior")> _
    <DefaultValue("")> _
    Public Property xNameFilter() As String
        Get
            If ViewState("NameFilter") Is Nothing Then
                Return ""
            Else
                Return DirectCast(ViewState("NameFilter"), String)
            End If
        End Get
        Set(ByVal value As String)
            ViewState("NameFilter") = value
        End Set
    End Property

    ''' <summary>
    ''' xTypeFilter identifies the types of any properties that you want to omit.
    ''' Do not use the fully qualified names. 
    ''' Types must be delimited with spaces. Matches are case insensitive.
    ''' <example>"Control ISite FontUnit"</example>
    ''' </summary>
    <Description("xTypeFilter identifies the types of any properties that you want to omit.   Do not use the fully qualified names.    Types must be delimited with spaces. Matches are case insensitive.   <example>""Control ISite FontUnit""</example>")> _
    <Browsable(True)> _
    <Category("Behavior")> _
    <DefaultValue("")> _
    Public Property xTypeFilter() As String
        Get
            If ViewState("TypeFilter") Is Nothing Then
                Return ""
            Else
                Return DirectCast(ViewState("TypeFilter"), String)
            End If
        End Get
        Set(ByVal value As String)
            ViewState("TypeFilter") = value
        End Set
    End Property

    ''' <summary>
    ''' xShowTypeColumnB shows a third column with the type name when true.
    ''' Default is true.
    ''' </summary>
    <Description("xShowTypeColumnB shows a third column with the type name when true.   Default is true.")> _
    <Browsable(True)> _
    <Category("Appearance")> _
    <DefaultValue(True)> _
    Public Property xShowTypeColumnB() As Boolean
        Get
            Return fShowTypeColumnB
        End Get
        Set(ByVal value As Boolean)
            fShowTypeColumnB = value
        End Set
    End Property
    Private fShowTypeColumnB As Boolean = True

    ''' <summary>
    ''' xHideAncestorPropsB omits properties defined on ancestors of xInstance's class.
    ''' Use this to focus the viewer on properties introduced in xInstance's class.
    ''' When true, ancestor properties are hidden.
    ''' When false, they are shown.
    ''' Default is false.
    ''' </summary>
    <Description("xHideAncestorPropsB omits properties defined on ancestors of xInstance's class.   Use this to focus the viewer on properties introduced in xInstance's class.   When true, ancestor properties are hidden.   When false, they are shown.   Default is false.")> _
    <Browsable(True)> _
    <Category("Behavior")> _
    <DefaultValue(False)> _
    Public Property xHideAncestorPropsB() As Boolean
        Get
            If ViewState("HideAncestorProps") Is Nothing Then
                Return False
            Else
                Return CBool(ViewState("HideAncestorProps"))
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("HideAncestorProps") = value
        End Set
    End Property

    ''' <summary>
    ''' BrowsableAttributeMode  determines if properties with BrowsableAttribute(false) are shown.
    ''' Here are the settings:
    ''' * VisualStudio - emulate Visual Studio by hiding those that Browsable(false)
    ''' * Templates - BrowsableAttribute(true) plus all ITemplates. ITemplates are a feature supported in
    '''   our interface better than in VS.NET. Thus users who hid templates to keep them out
    '''   of VS.NET can still show them here.
    ''' * Ignore - ignores the attribute
    ''' </summary>
    <Description("BrowsableAttributeMode determines if properties with BrowsableAttribute(false) are shown.   Here are the settings:   * VisualStudio - emulate Visual Studio by hiding those that Browsable(false)   * Templates - BrowsableAttribute(true) plus all ITemplates. ITemplates are a feature supported in    our interface better than in VS.NET. Thus users who hid templates to keep them out    of VS.NET can still show them here.   * Ignore - ignores the attribute")> _
    <Browsable(True)> _
    <Category("Behavior")> _
    <DefaultValue(BrowsableAttributeMode.Templates)> _
    Public Property xBrowsableAttributeMode() As BrowsableAttributeMode
        Get
            If ViewState("Browsable") Is Nothing Then
                Return BrowsableAttributeMode.Ignore
            Else
                Return DirectCast(ViewState("Browsable"), BrowsableAttributeMode)
            End If
        End Get
        Set(ByVal value As BrowsableAttributeMode)
            ViewState("Browsable") = value
        End Set
    End Property

#End Region

#Region "Events and overrides"

    Public Overloads Overrides Sub DataBind()
        MyBase.OnDataBinding(EventArgs.Empty)
        'applyValsToGrid()
    End Sub

    ''' <summary>
    ''' ConvertControlIDToInstance initializes xInstance with the object associated
    ''' with xControlToView.
    ''' </summary>
    <System.ComponentModel.Description("ConvertControlIDToInstance initializes xInstance with the object associated   with xControlToView.")> _
    Protected Overridable Sub ConvertControlIDToInstance()
        ' if we don't have xInstance, try xControlToView to get the control
        If (xInstance Is Nothing) AndAlso (xControlToView <> "") Then
            Dim vControl As Control = Page.FindControl(xControlToView)
            If (vControl Is Nothing) AndAlso (Page IsNot NamingContainer) Then
                ' try using the ID of the current container
                vControl = NamingContainer.FindControl(xControlToView)
            End If
            If vControl IsNot Nothing Then
                xInstance = vControl
            Else
                Throw New Exception("The control whose ID is " & xControlToView & " cannot be found on the page or in this naming container layer.")
            End If

        End If
    End Sub

    Protected Overloads Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)
        'If Page.IsPostBack Then
        Me.Style.Add("font-size", "9pt")
        buildPropList()
        'End If
        ' When posting back, we've already assigned the current values to any visible row
        ' because they were INPUT fields on the form and their contents is sent back through Post Back.
        ' The invisible fields lack values. That becomes a problem if the user has clicked an
        ' Object Property's Show Details button. This action will change the visibility
        ' of table rows on post back, and expose rows that do not have any values from the instance.
        ' This will update all of the invisible rows in anticipation.
        ConvertControlIDToInstance()
        '!bug fix 8/17/2002
        'If Page.IsPostBack Then
        '    ApplyInstance(xInstance, fChildTableRows, ApplyInstanceType.Invisible)

        'End If
    End Sub


    Protected Overloads Overrides Sub OnPreRender(ByVal e As EventArgs)
        MyBase.OnPreRender(e)
        buildPropList()
        'For Each vControl As Control In Controls
        '    If TypeOf vControl Is PropertyTableRow Then
        '        DirectCast(vControl, PropertyTableRow).BackColor = fLevelColors(DirectCast(vControl, PropertyTableRow).fPropertyControl.fDepth)
        '        DirectCast(vControl, PropertyTableRow).Cells(2).Visible = fShowTypeColumnB
        '    End If
        'Next
    End Sub

    Private Sub PropertiesEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Page.IsPostBack Then
            colapseall()
        End If
    End Sub

    Public Sub RenderAtDesignTime()
        DataBind()
        OnPreRender(New EventArgs())
    End Sub
#End Region

#Region "Update Target Values"

    ''' <summary>
    ''' ApplyInstance transfers data from pInstance to the controls.
    ''' It is called on DataBind and OnLoad during Post back.
    ''' Iterates through the properties on pInstance by going through table rows associated
    ''' with pInstance. It goes in sequential order. When it hits a row with an ObjectProperty,
    ''' it calls itself recursively to do the same on that property's instance.
    ''' When it hits a row with an IListProperty, it goes through its item list and calls
    ''' itself recursively for each instance in that list.
    ''' It updates visible or invisible rows depending on pType.
    ''' </summary>
    <System.ComponentModel.Description("ApplyInstance transfers data from pInstance to the controls.   It is called on DataBind and OnLoad during Post back.   Iterates through the properties on pInstance by going through table rows associated   with pInstance. It goes in sequential order. When it hits a row with an ObjectProperty,   it calls itself recursively to do the same on that property's instance.   When it hits a row with an IListProperty, it goes through its item list and calls   itself recursively for each instance in that list.   It updates visible or invisible rows depending on pType.")> _
    Protected Sub ApplyInstance(ByVal pInstance As Object, ByVal pChildTableRows As Collections.Generic.List(Of PropertyTableRow), ByVal pType As ApplyInstanceType)
        For Each vTableRow As PropertyTableRow In pChildTableRows
            Try
                If Not vTableRow.fRemovedB Then
                    ApplyInstanceToRow(vTableRow, pInstance, pChildTableRows, pType)
                End If
            Catch e As Exception
                If vTableRow.fErrorControl IsNot Nothing Then
                    ' report an error below the editing fields. fErrorControl is already using a red font.
                    vTableRow.fErrorControl.Text = "<br>" & e.Message
                    vTableRow.fErrorControl.Visible = True
                    ' if
                End If
                ' catch
            End Try
            ' foreach
        Next
    End Sub

    ''' <summary>
    ''' ApplyInstanceToRow is the heart of ApplyInstance. It takes one PropertyTableRow
    ''' and handles its ApplyValueToControl needs. For BaseObjectProperty subclasses,
    ''' it handles them by calling ApplyInstance on their children.
    ''' </summary>
    <System.ComponentModel.Description("ApplyInstanceToRow is the heart of ApplyInstance. It takes one PropertyTableRow   and handles its ApplyValueToControl needs. For BaseObjectProperty subclasses,   it handles them by calling ApplyInstance on their children.")> _
    Protected Overridable Sub ApplyInstanceToRow(ByVal pTableRow As PropertyTableRow, ByVal pInstance As Object, ByVal pChildTableRows As Collections.Generic.List(Of PropertyTableRow), ByVal pType As ApplyInstanceType)
        ' make the data transfer
        If (pInstance IsNot Nothing) AndAlso ((TypeOf pTableRow.fPropertyControl Is ReadOnlyProperty) OrElse (pType = ApplyInstanceType.All) OrElse ((pType = ApplyInstanceType.Invisible) AndAlso Not pTableRow.Visible) OrElse ((pType = ApplyInstanceType.Visible) AndAlso pTableRow.Visible)) Then
            pTableRow.fPropertyControl.ApplyValueToControl(pInstance)
        End If
        pTableRow.fErrorControl.Visible = False
        ' resets the state in case of a previous error
        ' if we are on an object property, get the associated object
        ' then call ApplyInstance on it
        ' Need to do this even if invisible so we can move the pRowNumber to the next visible row
        If TypeOf pTableRow.fPropertyControl Is ObjectProperty Then
            Dim vNewInstance As Object
            'vNewInstance = pTableRow.fPropertyControl.fPropertyInfo.GetValue(pInstance, Nothing)
            vNewInstance = pTableRow.fPropertyControl.getValue
            ApplyInstance(vNewInstance, DirectCast(pTableRow.fPropertyControl, ObjectProperty).fChildTableRows, pType)
            ' if we are on an IListProperty, we need to update its count and assign each
            ' IListItemProperty to the correct class
        ElseIf TypeOf pTableRow.fPropertyControl Is ListProperty Then
            Dim vEnumerator As IEnumerator = DirectCast(pInstance, IEnumerable).GetEnumerator()
            For Each vIListItemTableRow As PropertyTableRow In DirectCast(pTableRow.fPropertyControl, ListProperty).fChildTableRows
                If vEnumerator.MoveNext() Then
                    Dim vItemInstance As Object = vEnumerator.Current
                    If (pInstance IsNot Nothing) AndAlso ((TypeOf pTableRow.fPropertyControl Is ReadOnlyProperty) OrElse (pType = ApplyInstanceType.All) OrElse ((pType = ApplyInstanceType.Invisible) AndAlso Not pTableRow.Visible) OrElse ((pType = ApplyInstanceType.Visible) AndAlso pTableRow.Visible)) Then
                        vIListItemTableRow.fPropertyControl.ApplyValueToControl(vItemInstance)
                    End If

                    ApplyInstance(vItemInstance, DirectCast(vIListItemTableRow.fPropertyControl, ListItemProperty).fChildTableRows, pType)
                    ' if MoveNext
                End If
                ' foreach
            Next
            ' if (vTableRow is IListProperty)
        End If
    End Sub

    ''' <summary>
    ''' UpdateInstance assigns data from all edit fields to xInstance's properties.
    ''' Call it from your page's Submit event handler. You can connect your OnClick
    ''' event handler to Click_Submit as an alternative.
    ''' It does nothing when xViewOnlyB is true.
    ''' </summary>
    <System.ComponentModel.Description("UpdateInstance assigns data from all edit fields to xInstance's properties.   Call it from your page's Submit event handler. You can connect your OnClick   event handler to Click_Submit as an alternative.   It does nothing when xViewOnlyB is true.")> _
    Public Sub UpdateInstance()
        If xViewOnlyB OrElse (xInstance Is Nothing) Then
            Exit Sub
        End If
        EnsureChildControls()

        UpdateOneInstance(xInstance, fChildTableRows)
    End Sub

    Public ReadOnly Property changes() As ComparatorDS.DTIPropDifferencesDataTable
        Get
            If Page.Session("Changes_" & xControlToView) Is Nothing Then
                Page.Session("Changes_" & xControlToView) = New ComparatorDS.DTIPropDifferencesDataTable
            End If
            Return Page.Session("Changes_" & xControlToView)
        End Get
    End Property

    Public Function changeList() As String
        Dim str As String = ""
        For Each row As ComparatorDS.DTIPropDifferencesRow In changes
            Dim val As Object = Comparator.desearializeFromBase64String(row)
            If val Is Nothing Then val = "{null}"
            str &= row.PropertyPath & " : " & Me.Page.Server.HtmlEncode(val.ToString) & "<br>" & vbCrLf
        Next
        Return str
    End Function

    ''' <summary>
    ''' UpdateOneInstance iterates through the properties on pInstance by going through table rows associated
    ''' with pInstance. It goes in sequential order. When it hits a row with an ObjectProperty,
    ''' it calls itself recursively to do the same on that property's instance.
    ''' When it hits a row with an IListProperty, it goes through its Items list.
    ''' Any invisible rows are have no data transferred because they are not written to the client. 
    ''' </summary>
    <System.ComponentModel.Description("UpdateOneInstance iterates through the properties on pInstance by going through table rows associated   with pInstance. It goes in sequential order. When it hits a row with an ObjectProperty,   it calls itself recursively to do the same on that property's instance.   When it hits a row with an IListProperty, it goes through its Items list.   Any invisible rows are have no data transferred because they are not written to the client.")> _
    Protected Sub UpdateOneInstance(ByVal pInstance As Object, ByRef pChildTableRows As Collections.Generic.List(Of PropertyTableRow))
        For Each vTableRow As PropertyTableRow In pChildTableRows
            Try
                If Not vTableRow.fRemovedB Then
                    UpdateOneTableRow(vTableRow, pInstance, pChildTableRows)
                End If
            Catch e As Exception
                If vTableRow.fErrorControl IsNot Nothing Then
                    ' report an error below the editing fields. fErrorControl is already using a red font.
                    vTableRow.fErrorControl.Text = "<br>" & e.Message
                    vTableRow.fErrorControl.Visible = True
                    ' if
                End If
                ' catch
            End Try
            ' foreach
        Next
    End Sub

    ''' <summary>
    ''' UpdateOneTableRow is the heart of UpdateOneInstance. It transfer data from the pTableRow
    ''' to pInstance. For BaseObjectProperty subclasses, it calls UpdateOneInstance recursively to
    ''' handle their children.
    ''' </summary>
    <System.ComponentModel.Description("UpdateOneTableRow is the heart of UpdateOneInstance. It transfer data from the pTableRow   to pInstance. For BaseObjectProperty subclasses, it calls UpdateOneInstance recursively to   handle their children.")> _
    Protected Overridable Sub UpdateOneTableRow(ByVal pTableRow As PropertyTableRow, ByVal pInstance As Object, ByRef pChildTableRows As Collections.Generic.List(Of PropertyTableRow))

        ' resets the state in case of a previous error
        ' if we are on an object property, get the associated object
        ' then call UpdateOneInstance on it.
        ' Need to do this even if invisible so we can move the pRowNumber to the next visible row
        If TypeOf pTableRow.fPropertyControl Is ObjectProperty Then
            Dim vNewInstance As Object = pTableRow.fPropertyControl.fPropertyInfo.GetValue(pInstance, Nothing)
            UpdateOneInstance(vNewInstance, DirectCast(pTableRow.fPropertyControl, ObjectProperty).fChildTableRows)
            ' if on IListProperty, go through the IListItemProperty items and update
            ' the properties on those instances. We don't support adding or removing instances.
            ' (That's a possibility for the future requiring Clear() then creating all the instances.)
        ElseIf TypeOf pTableRow.fPropertyControl Is ListProperty Then
            Dim vEnumerator As IEnumerator = DirectCast(pInstance, IEnumerable).GetEnumerator()
            For Each vIListItemTableRow As PropertyTableRow In DirectCast(pTableRow.fPropertyControl, ListProperty).fChildTableRows
                If vEnumerator.MoveNext() Then
                    Dim vItemInstance As Object = vEnumerator.Current
                    Try
                        If TypeOf vIListItemTableRow.fPropertyControl Is BaseObjectProperty Then
                            UpdateOneInstance(vItemInstance, DirectCast(vIListItemTableRow.fPropertyControl, ListItemProperty).fChildTableRows)
                        Else
                            vIListItemTableRow.fPropertyControl.UpdateValueFromControl(vItemInstance)
                        End If

                    Catch ex As Exception
                    End Try
                    ' if MoveNext
                End If
                ' foreach
            Next
        End If
        If (pInstance IsNot Nothing) AndAlso (pTableRow.Visible) Then
            pTableRow.fPropertyControl.UpdateValueFromControl(pInstance)
        End If
        pTableRow.fErrorControl.Visible = False
    End Sub

    Private _propertyString As String
    Private _propertyStringList As New Generic.List(Of String)
    Public Property propertyList() As String
        Get
            Return _propertyString
        End Get
        Set(ByVal value As String)
            _propertyString = value
            _propertyStringList.Clear()
            For Each str As String In _propertyString.Split(",")
                _propertyStringList.Add(str.ToLower)
            Next
        End Set
    End Property

    ''' <summary>
    ''' FilterOut determines if the property should be shown or not.
    ''' It returns true when the property should be hidden.
    ''' There are numerous rules:
    ''' 1. Write Only.
    ''' 2. Read Only (various cases)
    ''' 3. Arrays
    ''' 4. Apply xHideAncestorPropsB and xBrowsableAttributeMode properties
    ''' 5. Apply the filters by Name and Type
    ''' </summary>
    <System.ComponentModel.Description("FilterOut determines if the property should be shown or not.   It returns true when the property should be hidden.   There are numerous rules:   1. Write Only.   2. Read Only (various cases)   3. Arrays   4. Apply xHideAncestorPropsB and xBrowsableAttributeMode properties   5. Apply the filters by Name and Type")> _
    Protected Overridable Function FilterOut(ByVal pClassType As Type, ByVal pPropertyInfo As PropertyInfo) As Boolean
        ' filter out anything that is write only
        ' skip read onlys except for read only objects because we can go into the objects for more properties
        If xBrowsableAttributeMode = BrowsableAttributeMode.OnlyListed Then
            If Not _propertyStringList.Contains(pPropertyInfo.Name.ToLower) Then Return True
            Return False
        End If
        If Not pPropertyInfo.CanRead AndAlso pPropertyInfo.CanWrite Then
            ' write only
            Return True
        End If

        ' hide read only unless the user asked for them or they are objects
        If Not Me.xShowReadOnlyB AndAlso pPropertyInfo.CanRead AndAlso Not pPropertyInfo.CanWrite Then
            ' read only
            If pPropertyInfo.PropertyType.IsEnum OrElse pPropertyInfo.PropertyType.IsPrimitive Then
                Return True
            End If

            If pPropertyInfo.PropertyType Is GetType(String) Then
                Return True
            End If

            ' not a class, or if its a class, property is null.
            If Not pPropertyInfo.PropertyType.IsClass Then
                Return True
            End If
        End If

        If pPropertyInfo.PropertyType.IsArray Then
            Return True
        End If
        ' no support of arrays
        ' user can omit properties on ancesters
        If (xHideAncestorPropsB) AndAlso (pPropertyInfo.DeclaringType IsNot pClassType) Then
            Return True
        End If

        ' user can omit properties that have their Browsable Attribute set to false.
        If (xBrowsableAttributeMode = BrowsableAttributeMode.VisualStudio) OrElse ((xBrowsableAttributeMode = BrowsableAttributeMode.Templates) AndAlso (pPropertyInfo.PropertyType IsNot GetType(ITemplate))) Then
            Dim vAttribA As Object() = pPropertyInfo.GetCustomAttributes(GetType(BrowsableAttribute), True)
            If (vAttribA.Length > 0) AndAlso Not DirectCast(vAttribA(0), BrowsableAttribute).Browsable Then
                Return True
            End If
        End If

        ' the user can request property names and types to remove in xNameFilter and xTypeFilter.
        If fFinalNameFilter <> "" Then
            Dim vNamePattern As String = "\b" & Regex.Escape(pPropertyInfo.Name) & "\b"
            If Regex.IsMatch(fFinalNameFilter, vNamePattern, RegexOptions.IgnoreCase) Then
                Return True
            End If
        End If
        If fFinalTypeFilter <> "" Then
            Dim vTypePattern As String = "\b" & Regex.Escape(pPropertyInfo.PropertyType.Name) & "\b"
            If Regex.IsMatch(fFinalTypeFilter, vTypePattern, RegexOptions.IgnoreCase) Then
                Return True
            End If
        End If

        ' no problems. Passed all tests
        Return False
    End Function

#End Region

#Region "BuildControls"

    Private finalbuilt As Boolean = False
    Public Sub buildproplistFinal()
        buildPropList()
        finalbuilt = True
    End Sub

    Private Sub buildPropList()
        If Not finalbuilt Then

            ' Reset the control's state.
            Controls.Clear()
            ClearChildViewState()

            ' if we don't have xInstance, try xControlToView to get the control
            ConvertControlIDToInstance()
            ' Create the control hierarchy using the data source.
            If xInstance Is Nothing Then
                xInstance = Me.Parent
            End If
            If xInstance IsNot Nothing Then
                CreateControlHierarchy(xInstance.[GetType](), xInstance)
            End If
            ChildControlsCreated = True

            TrackViewState()
            If xInstance IsNot Nothing Then
                ' remember xInstance's class for post back.
                ' Requires both the class name and its assembly.
                ViewState("PrimaryClass") = xInstance.[GetType]().FullName
                ViewState("PrimaryAssembly") = xInstance.[GetType]().Assembly.FullName

                ' attach the data from xInstance to the table rows
                ApplyInstance(xInstance, fChildTableRows, ApplyInstanceType.All)

            End If
        End If

    End Sub

    ''' <summary>
    ''' It builds and returns a SortedList of both public and non-public properties for this class type. 
    ''' Each element of the list has a name and value.
    ''' The name is the PropertyInfo.Name and this sorts the list.
    ''' The value is the PropertyInfo.
    ''' There are two sources for this list.
    ''' It looks in Cache["PE."+Type.Name]. If not there, it creates it from data in the Reflection system for pClassType.
    ''' Caching can be disabled by setting xCacheMinutes to 0.
    ''' </summary>
    <System.ComponentModel.Description("It builds and returns a SortedList of both public and non-public properties for this class type.    Each element of the list has a name and value.   The name is the PropertyInfo.Name and this sorts the list.   The value is the PropertyInfo.   There are two sources for this list.   It looks in Cache[""PE.""+Type.Name]. If not there, it creates it from data in the Reflection system for pClassType.   Caching can be disabled by setting xCacheMinutes to 0.")> _
    Protected Overridable Function GetPropertyInfoList(ByVal pClassType As Type) As SortedList
        Dim vSortedProperties As SortedList = Nothing
        Dim vInCacheB As Boolean = False
        Dim vNameInCache As String = "PE." & pClassType.Name
		Dim vBindingFlags As BindingFlags = BindingFlags.[Public] Or BindingFlags.Instance Or BindingFlags.DeclaredOnly

		'If you declare a property list, show any properties from parent classes.
		If propertyList <> "" Then
			vBindingFlags = BindingFlags.[Public] Or BindingFlags.Instance
		End If
		' non-public fields require a different BindingFlags list.
		' We'll use a different cache too because the list is different.
		If xShowNonPublicB Then
            vBindingFlags = BindingFlags.[Public] Or BindingFlags.Instance Or BindingFlags.NonPublic
            vNameInCache = "PE.ALL." & pClassType.Name
        End If

        ' first check for this class in the application Cache
        If (Context IsNot Nothing) AndAlso (Context.Cache(vNameInCache) IsNot Nothing) Then
            ' Design mode lacks the Context
            If xCacheMinutes > 0 Then
                vSortedProperties = DirectCast(Context.Cache(vNameInCache), SortedList)
                vInCacheB = True
            Else
                ' Not only do we ignore the cache but we clear it
                Context.Cache.Remove(vNameInCache)
            End If
        End If

        If Not vInCacheB Then
            Dim vPropertyInfoA As System.Reflection.PropertyInfo() = pClassType.GetProperties(vBindingFlags)
            ' sort it using a SortedList() class.
            vSortedProperties = New SortedList()
            ' uses the default comparer
            For vI As Integer = 0 To vPropertyInfoA.Length - 1
                Try
                    vSortedProperties.Add(vPropertyInfoA(vI).Name, vPropertyInfoA(vI))
                Catch generatedExceptionName As ArgumentException
                    ' don't add it. Its a duplicate. This has been found in some cases. (HttpApplicationState has two Items)
                End Try
            Next

            ' add to the cache for xCacheMinutes. 
            If (xCacheMinutes > 0) AndAlso (Context IsNot Nothing) Then
                ' Design mode lacks the Context
                Context.Cache.Add(vNameInCache, vSortedProperties, Nothing, DateTime.Now.AddMinutes(xCacheMinutes), New TimeSpan(0, 0, 0, 0), CacheItemPriority.BelowNormal, _
                 Nothing)
            End If
        End If

        Return vSortedProperties
    End Function

    ''' <summary>
    ''' CreateChildControls follows the standards for Data Bound templated controls
    ''' by building the control hierarchy only on post back.
    ''' </summary>
    <System.ComponentModel.Description("CreateChildControls follows the standards for Data Bound templated controls   by building the control hierarchy only on post back.")> _
    Protected Overloads Overrides Sub CreateChildControls()
        Controls.Clear()
        If ViewState("PrimaryClass") IsNot Nothing Then
            ' post back. Create controls. If not, DataBind does the work.
            ' conversion of the class type string to a class requires loading the associated
            ' assembly.
            Dim vAssemblyName As String = ViewState("PrimaryAssembly").ToString()
            Dim vAssembly As Assembly = Assembly.Load(vAssemblyName)
            Dim vClassType As Type = vAssembly.[GetType](ViewState("PrimaryClass").ToString(), True)
            CreateControlHierarchy(vClassType, Nothing)
        End If
    End Sub

    Protected Sub CreateControlHierarchy(ByVal pClassType As Type, ByVal pInstance As Object)
        If Context IsNot Nothing Then
            ' avoid design mode issues
            Context.Trace.Write("Begin CreateControlHierarchy")
        End If

        fFinalNameFilter = xNameFilter
        ' limits the work when applying these values frequently
        fFinalTypeFilter = xTypeFilter
        ' when you include non-browsable properties, you bring in the Page, NamingContainer, and BindingContainers.
        ' Aside from being outside the perimeter of properties users can edit,
        ' these generate errors when retrieving properties. The page class itself has a self
        ' referential property called Page.
        ' Remove all of these names.
        fFinalNameFilter += " Page NamingContainer BindingContainer"

        ' Start at the top level
        Dim vCountRows As Integer = CreateTableRows(pInstance, "", "", True, Nothing)

        If Context IsNot Nothing Then
            ' avoid design mode issues
            Context.Trace.Write("End CreateControlHierarchy")
        End If
    End Sub

    Public Function CreateTableRows(ByVal pInstance As Object, ByVal propertyPath As String, ByVal pLeadText As String, ByVal pVisibleB As Boolean, ByVal pParentObjectProperty As BaseObjectProperty, Optional ByVal addAtIndex As Integer = -1) As Integer
        'Dim vRowCount As Integer = 0
        If pInstance Is Nothing Then

            CreateTableRow(CreateNothingPropertyControl(Me, propertyPath & ".nothing"), "Nothing", pParentObjectProperty, addAtIndex)

            Return 0
        End If
        Dim pClassType As Type
        If Not pInstance Is Nothing Then
            pClassType = pInstance.GetType
        Else
            pClassType = Nothing
        End If
        Dim depth As Integer = propertyPath.Split(".").Length - 1
        If propertyPath.EndsWith(")") Then
            '  propertyPath = propertyPath & "."
            depth += 1
        End If

        ' fLevelColors provides a maximum depth
        ' get the properties for this instance in a specified order
        Dim sortedProperties As SortedList = GetPropertyInfoList(pClassType)
        Dim addedct As Integer = 0
        For vI As Integer = 0 To sortedProperties.Count - 1

            Dim propControl As BaseProperty = Nothing
            Dim vPropertyInfo As PropertyInfo = DirectCast(sortedProperties.GetByIndex(vI), PropertyInfo)

            Dim vPropName As String = pLeadText & vPropertyInfo.Name

            ' Filter out any inappropriate or undesirable properties
            If FilterOut(pClassType, vPropertyInfo) Then
                Continue For
            End If

            propControl = CreatePropertyControl1(Me, vPropertyInfo, propertyPath & "." & vPropertyInfo.Name, pParentObjectProperty, xViewOnlyB)

            If propControl IsNot Nothing Then
                addedct += 1
                ' add a new row to the table with a label, propControl and the class name in the third column
                Dim vTableRow As PropertyTableRow = CreateTableRow(propControl, vPropertyInfo.PropertyType.Name, pParentObjectProperty, addAtIndex + addedct)
                ' if we added an ObjectProperty, add the properties on the associated class. 
                If TypeOf propControl Is ObjectProperty Then
                    'Try
                    Dim vNewInstance As Object = Nothing
                    If pInstance IsNot Nothing Then
                        Try
                            vNewInstance = vPropertyInfo.GetValue(pInstance, Nothing)
                        Catch ex As Exception
                        End Try
                    End If
                    If CType(propControl, ObjectProperty).xOpen Then
                        addedct += CreateObjectRowChildren1(vNewInstance, vPropertyInfo.PropertyType, propertyPath, pVisibleB, vPropName, propControl, vTableRow, addAtIndex + addedct)
                    End If

                    'Catch generatedExceptionName As Exception
                    '    vTableRow.Visible = False
                    '    vTableRow.fRemovedB = True
                    'End Try
                ElseIf TypeOf propControl Is ListProperty Then

                    'If CType(propControl, IListProperty).xOpen Then
                    addedct += CreateIListRowChildren(pInstance, vPropertyInfo, propertyPath, pVisibleB, DirectCast(propControl, ListProperty), _
                     vTableRow, addAtIndex + addedct)
                    'End If
                End If
            End If
        Next
        Return addedct
    End Function

    Protected Overridable Function CreateTableRow(ByVal pRowControl As BaseProperty, ByVal typetext As String, ByVal pParentObjectProperty As BaseObjectProperty, Optional ByVal addat As Integer = -1) As PropertyTableRow
        Dim vTableRow As New PropertyTableRow(pRowControl, typetext)
        If addat < 0 Then
            Controls.Add(vTableRow)
        Else
            Controls.AddAt(addat, vTableRow)
        End If

        If pParentObjectProperty IsNot Nothing Then
            pParentObjectProperty.AddChildTableRow(vTableRow)
        Else
            fChildTableRows.Add(vTableRow)
        End If
        Return vTableRow
    End Function


    ''' <summary>
    ''' CreateObjectRowChildren installs the properties of the instance associated with an ObjectProperty.
    ''' pNewInstance will be null on post back. pPropertyType must always reflect the class type.
    ''' </summary>
    <System.ComponentModel.Description("CreateObjectRowChildren installs the properties of the instance associated with an ObjectProperty.   pNewInstance will be null on post back. pPropertyType must always reflect the class type.")> _
    Public Function CreateObjectRowChildren1(ByVal pNewInstance As Object, ByVal pPropertyType As Type, ByVal propertypath As String, ByVal pVisibleB As Boolean, ByVal pLeadText As String, ByVal objPropCtrl As BaseObjectProperty, _
     ByVal pTableRow As PropertyTableRow, Optional ByVal addatIndex As Integer = -1) As Integer
        Dim vNewRowCount As Integer = 0

        ' DisplayThisInstance's pVisibleB flag is determined as follows:
        ' if xOpen = true and pVisibleB is true, use true; otherwise use false
        Dim vNewVisibleB As Boolean = pVisibleB AndAlso DirectCast(objPropCtrl, BaseObjectProperty).xOpen
        Dim lbl As String = propertypath
        If Not objPropCtrl.fPropertyInfo Is Nothing Then lbl = propertypath & "." & objPropCtrl.fPropertyInfo.Name
        vNewRowCount = CreateTableRows(pNewInstance, lbl, pLeadText & ".", vNewVisibleB, DirectCast(objPropCtrl, BaseObjectProperty), addatIndex)

        If vNewRowCount = 0 Then
            ' no children. So hide this row
            pTableRow.Visible = False
            pTableRow.fRemovedB = True
            ' this special flag tells all users of vTableRow never to activate Visible flag
            ' if (vNewRowCount == 0)
        End If
        Return vNewRowCount
    End Function



    ''' <summary>
    ''' CreateIListRowChildren installs the IListItemProperties for the items of an indexer.
    ''' If pInstance != null, create them from the instance. Otherwise, use ViewState information
    ''' to determine how many IListItemProperties and which classes they represent.
    ''' </summary>
    <System.ComponentModel.Description("CreateIListRowChildren installs the IListItemProperties for the items of an indexer.   If pInstance != null, create them from the instance. Otherwise, use ViewState information   to determine how many IListItemProperties and which classes they represent.")> _
    Protected Overridable Function CreateIListRowChildren(ByVal pInstance As Object, ByVal pPropertyInfo As PropertyInfo, ByVal propertyPath As String, ByVal pVisibleB As Boolean, ByVal pIListPropertyControl As ListProperty, _
     ByVal pTableRow As PropertyTableRow, Optional ByVal addAtIndex As Integer = -1) As Integer
        Dim pdepth As Integer = propertyPath.Split(".").Length - 1
        pdepth += 1
        ' IListProperties are always nested one level
        'If pDepth >= fLevelColors.Length Then
        '    Exit Sub
        'End If
        Dim displayText As String = propertyPath.Trim(".") & ".item"
        If pInstance Is Nothing Then
        ElseIf TypeOf pInstance Is IEnumerable Then
            ' Generate from the instance
            Dim itemCount As Integer = 0
            Dim expandedItemCount As Integer = 0
            Dim vEnumerator As IEnumerator = DirectCast(pInstance, IEnumerable).GetEnumerator()
            While vEnumerator.MoveNext()
                Dim vItemInstance As Object = vEnumerator.Current

                Dim itemproppath As String = displayText & "(" & itemCount & ")."
                'Dim vItemRowControl As New ListItemProperty(Me, pPropertyInfo, pdepth, pIListPropertyControl, propertyPath, itemCount)
                Dim itemRowControl As ListItemProperty = CreatePropertyControl1(Me, pPropertyInfo, itemproppath, pIListPropertyControl, False, itemCount)
                ' insert it into the table as one row.
                Dim vTableRow As PropertyTableRow = CreateTableRow(itemRowControl, vItemInstance.GetType().Name, pIListPropertyControl, addAtIndex + itemCount + expandedItemCount)
                'itemRowControl.setButton()
                If itemRowControl.xOpen Then
                    Dim newinstance As Object
                    newinstance = itemRowControl.getValue
                    expandedItemCount += CreateTableRows(newinstance, "." & itemRowControl.propertyPath, itemRowControl.propertyPath, True, itemRowControl, addAtIndex + itemCount + expandedItemCount)
                End If
                ' NOTE: We don't setup instance specific data (xInstanceType) here. That happens in ApplyInstance
                itemCount += 1
            End While
            ' while
            pIListPropertyControl.xItemCount = itemCount

            pTableRow.fThirdColControl.Text = "Count: " & itemCount.ToString()
            Return itemCount + expandedItemCount
        Else
            ' if (pInstance is IEnumerable)
            ' not enumerable. Don't bother adding it or its children
            pTableRow.Visible = False
            pTableRow.fRemovedB = True
            ' this special flag tells all users of vTableRow never to activate Visible flag
            Return 0
        End If

    End Function

    Private Function CreatePropertyControl1(ByVal containerControl As Control, ByVal pInfo As PropertyInfo, ByVal propertypath As String, Optional ByRef pParentObjectProperty As BaseObjectProperty = Nothing, Optional ByVal viewonly As Boolean = False, Optional ByVal listItemIndex As Integer = -1) As BaseProperty
        Dim depth As Integer = propertypath.Split(".").Length - 1
        Dim propertyEditorControl As BaseProperty = Nothing
        If Not pParentObjectProperty Is Nothing AndAlso pParentObjectProperty.fChildTableRows.Count > 0 Then
            For Each ctrl As PropertyTableRow In pParentObjectProperty.fChildTableRows

                If ctrl.fPropertyControl IsNot Nothing AndAlso ctrl.fPropertyControl.propertyPath = propertypath.Trim(".") Then
                    Return ctrl.fPropertyControl
                End If
            Next
        End If
        If listItemIndex > -1 Then
            propertyEditorControl = New ListItemProperty(containerControl, pInfo, depth, pParentObjectProperty, propertypath, listItemIndex) '  (Me, pdepth, DirectCast(pIListPropertyControl, BaseObjectProperty), propertypath, vItemCount)
        ElseIf (viewonly OrElse Not pInfo.CanWrite) AndAlso (Not pInfo.PropertyType.IsClass OrElse (pInfo.PropertyType Is GetType(String))) Then
            propertyEditorControl = New ReadOnlyProperty(containerControl, pInfo, depth)
        Else
            If pInfo.GetIndexParameters().Length = 1 Then
                propertyEditorControl = New ListProperty(containerControl, pInfo, depth, pParentObjectProperty)
            ElseIf pInfo.PropertyType.IsEnum Then
                propertyEditorControl = New EnumProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(System.Drawing.Color) Then
                propertyEditorControl = New ColorProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(System.Web.UI.WebControls.Unit) Then
                propertyEditorControl = New UnitProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(System.Web.UI.WebControls.FontUnit) Then
                propertyEditorControl = New FontUnitProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(String) Then
                propertyEditorControl = New StringProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(Boolean) Then
                propertyEditorControl = New BoolProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(DateTime) Then
                propertyEditorControl = New DateProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(Decimal) Then
                propertyEditorControl = New NumericProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType.IsPrimitive Then
                ' captures anything that is supported by NumericProperty
                propertyEditorControl = New NumericProperty(containerControl, pInfo, depth)
            ElseIf pInfo.PropertyType Is GetType(ITemplate) Then
                propertyEditorControl = New TemplateProperty(containerControl, pInfo, depth)
            Else
                ' its another object. We'll get a row for a header here.
                ' Near the end of this method, we'll add this object's properties to the TableRows list.
                ' That code will detect if this object has nothing to show
                ' and will remove this control.
                propertyEditorControl = New ObjectProperty(containerControl, pInfo, depth, pParentObjectProperty)
            End If
        End If
        propertyEditorControl.propertyPath = propertypath.Trim(".")
        propertyEditorControl.ID = "Proped_ctrl_" & propertyEditorControl.propertyPath.Replace(".", "_")
        propertyEditorControl.mainID = Me.mainID
        Return propertyEditorControl
    End Function


    Private Function CreateNothingPropertyControl(ByVal containerControl As Control, ByVal propertypath As String) As NothingProperty
        Dim depth As Integer = propertypath.Split(".").Length - 1
        Dim propertyEditorControl As New NothingProperty(containerControl, Nothing, depth)

        propertyEditorControl.propertyPath = propertypath.Trim(".")
        propertyEditorControl.ID = "Proped_ctrl_" & propertyEditorControl.propertyPath.Replace(".", "_")
        propertyEditorControl.mainID = Me.mainID
        Return propertyEditorControl
    End Function

#End Region

    Public Sub colapseall()
        Dim remkeys As New Collection
        For Each key As String In Me.Page.Session.Keys
            If key.StartsWith("PropEditor_open_") Then remkeys.Add(key)
        Next
        For Each key As String In remkeys
            Page.Session.Remove(key)
        Next
    End Sub

    Public Shared Sub setPropertiesFromDiffs(ByVal dt As ComparatorDS.DTIPropDifferencesDataTable, ByVal target As Object, ByRef session As System.Web.SessionState.HttpSessionState)
        Comparator.setProperties(dt, target, session)
    End Sub

End Class

''' <remarks>
''' ApplyInstanceType assists the ApplyInstance method as it updates the values assigned to controls.
''' Each row in the table may be visible or invisible. When visible, post back code assigns
''' values. When invisible, it does not.
''' This determines which rows get updated based on visibility.
''' </remarks>
<System.ComponentModel.Description("")> _
Public Enum ApplyInstanceType
    Visible
    Invisible
    ' used on post back
    All
    ' used on databind
End Enum

''' <remarks>
''' BrowsableAttributeMode works with the PropertiesEditor.xBrowsableAttributeMode property. It determines
''' if properties with BrowsableAttribute(false) are shown.
''' * VisualStudio - emulate Visual Studio by hiding those that Browsable(false)
''' * Ignore - ignores the attribute
''' * Templates - BrowsableAttribute(true) plus all ITemplates. ITemplates are a feature supported in
'''   our interface better than in VS.NET. Thus users who hid templates to keep them out
'''   of VS.NET can still show them here.
''' </remarks>
<System.ComponentModel.Description("")> _
Public Enum BrowsableAttributeMode
    VisualStudio
    Templates
    Ignore
    OnlyListed
End Enum

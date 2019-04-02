Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports BaseClasses

<ToolboxData("<{0}:DynamicPropertyEditor runat=server></{0}:DynamicPropertyEditor>")>
<ComponentModel.Designer(GetType(PropertiesEditorDesigner))>
Public Class DynamicPropertyEditor
    Inherits DTIServerControls.DTIServerControl
    Private pnl As Panel
    Private pnlChanges As Panel
    Private pnlSavedChanges As Panel
    Private litSavedChangeList As LiteralControl
    Private litChangeList As LiteralControl
    Private script As LiteralControl
    Public WithEvents btnupdate As New Button
    Public WithEvents btnCancel As Button
    Public WithEvents btnrevert As Button
    Public WithEvents btnsave As Button
    Dim dlgChanges As New JqueryUIControls.Dialog
    'Private WithEvents dd As New DropDownList
    ' Public Shared PropEditorOnDefaultKey As String = "PropEditor.EditOn"
    'Public Shared PropEditorMainID As String = "PropEditor.MainID"
    Public WithEvents _propeditor As PropertiesGrid
    Public Event gridSet()
    Public Event objectPropertiesSet(ByVal objectInstance As Object)
    Private changesFetched
    Friend WithEvents mypage As Page

#Region "Properties"

    'Private _width As Web.UI.WebControls.Unit = Nothing
    'Public Property width() As Web.UI.WebControls.Unit
    '    Get
    '        Return _width
    '    End Get
    '    Set(ByVal value As Web.UI.WebControls.Unit)
    '        _width = value
    '    End Set
    'End Property

    'Private _height As Web.UI.WebControls.Unit = Nothing
    'Public Property Height() As Web.UI.WebControls.Unit
    '    Get
    '        Return _height
    '    End Get
    '    Set(ByVal value As Web.UI.WebControls.Unit)
    '        _height = value
    '    End Set
    'End Property


    Private manuallySetPropertiesValue As Boolean = False
    Public Property manuallySetProperties() As Boolean
        Get
            Return manuallySetPropertiesValue
        End Get
        Set(ByVal value As Boolean)
            manuallySetPropertiesValue = value
        End Set
    End Property

    Public ReadOnly Property propeditor() As PropertiesGrid
        Get
            If _propeditor Is Nothing Then _propeditor = New PropertiesGrid
            Return _propeditor
        End Get
    End Property

    'Private _helper As BaseHelper
    'Protected ReadOnly Property sqlhelper() As BaseClasses.BaseHelper
    '    Get
    '        If _helper Is Nothing Then
    '            If TypeOf Me.Page Is BaseSecurityPage Then
    '                _helper = CType(Me.Page, BaseSecurityPage).sqlHelper
    '            ElseIf TypeOf Me.Page.Master Is MasterBase Then
    '                _helper = CType(Me.Page.Master, MasterBase).sqlHelper
    '            End If
    '            If _helper Is Nothing Then _helper = New SQLHelper
    '        End If
    '        Return _helper
    '    End Get
    'End Property

    Private _startingObj As Control
    Public Property startingControl() As Control
        Get
            If _startingObj Is Nothing Then Return Me.Page
            Return _startingObj
        End Get
        Set(ByVal value As Control)
            If Not _startingObj Is value Then
                _startingObj = value
                If Not manuallySetProperties Then _
                setProperties()
            End If
        End Set
    End Property

    'Private _MainID As Integer = 0
    'Public Property MainID() As Integer
    '    Get
    '        If MasterMainId >= 0 Then
    '            Return MasterMainId
    '        Else
    '            Return _MainID
    '        End If
    '    End Get
    '    Set(ByVal value As Integer)
    '        _MainID = value
    '    End Set
    'End Property

    'Private _masterChanged As Boolean = False
    'Public Property MasterChanged() As Boolean
    '    Get
    '        Return Page.Session("_masterChanged")
    '    End Get
    '    Set(ByVal value As Boolean)
    '        Page.Session("_masterChanged") = value
    '    End Set
    'End Property

    'Private _masterMainId As Integer = -1
    'Public Property MasterMainId() As Integer
    '    Get
    '        Return Page.Session(PropEditorMainID)
    '    End Get
    '    Set(ByVal value As Integer)
    '        Page.Session(PropEditorMainID) = value
    '        MasterChanged = True
    '    End Set
    'End Property

    'Private ReadOnly Property session() As System.Web.SessionState.HttpSessionState
    '    Get
    '        If Page Is Nothing Then Return System.Web.HttpContext.Current.Session Else Return Page.Session
    '    End Get
    'End Property

    Private _adminOn As Boolean = False
    Public Property AdminOn() As Boolean
        Get
            Return _adminOn
            'If session(adminSessionKey) Is Nothing Then session(adminSessionKey) = False
            'Return Session(adminSessionKey)
        End Get
        Set(ByVal value As Boolean)
            _adminOn = value
            'session(adminSessionKey) = value
        End Set
    End Property

    'Private _adminSessionKey As String = PropEditorOnDefaultKey
    'Public Property adminSessionKey() As String
    '    Get
    '        Return _adminSessionKey
    '    End Get
    '    Set(ByVal value As String)
    '        _adminSessionKey = value
    '    End Set
    'End Property

    Private _objectKey As String
    Public Property objectKey() As String
        Get
            If _objectKey Is Nothing Then _objectKey = Page.GetType().Name & "_" & startingControl.ID
            Return _objectKey
        End Get
        Set(ByVal value As String)
            _objectKey = value
            propeditor.objectKey = value
        End Set
    End Property

    Public Property secondaryKey As String = "DefaultSettings"

#End Region

    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)

        If Me.DesignMode = True Then
            writer.Write("[Dynamic Property Editor]")
        End If
        Try
            MyBase.Render(writer)
        Catch e As Exception
        End Try
    End Sub

    Private Sub DynamicPropertyEditor_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        mypage = Me.Page
    End Sub

    Private Sub DynamicPropertyEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If AdminOn Then
            setgrid()
        Else
            Me.Width = Nothing
            Me.Height = Nothing
        End If
    End Sub

    Public Sub setProperties()
        getChanges()

        If Not AdminOn Then
            Comparator.setProperties(changes, startingControl, CType(System.Web.HttpContext.Current.Handler, Page).Session)
            RaiseEvent objectPropertiesSet(startingControl)
        End If
        If AdminOn AndAlso Not pnlChanges Is Nothing Then
            If litChangeList Is Nothing Then litChangeList = New LiteralControl
            litChangeList.Text = displayChangeList(propeditor.changes)
            'pnlChanges.Controls.Add(New LiteralControl(displayChangeList(propeditor.changes)))
            If litSavedChangeList Is Nothing Then litSavedChangeList = New LiteralControl
            litSavedChangeList.Text = displayChangeList(changes)
            'pnlSavedChanges.Controls.Add(New LiteraslControl(displayChangeList(changes)))
            If Not blockPrerenderUpdate Then
                Comparator.setProperties(changes, startingControl, Page.Session)
                If propeditor.expandedObject Then propeditor.UpdateInstance()
            End If
            propeditor.buildproplistFinal()
            'RaiseEvent objectPropertiesSet(startingControl)
        End If
    End Sub


    Private changes As New ComparatorDS.DTIPropDifferencesDataTable
    Private Sub getChanges()
        If Not sqlhelper.defaultConnection Is Nothing Then
            Try
                changes.Clear()
                If secondaryKey = "" Then secondaryKey = objectKey
                sqlhelper.FillDataTable("select * from DTIPropDifferences where mainID = @mainid and  objectKey = @objectKey ", changes, MainID, secondaryKey)
                sqlhelper.FillDataTable("select * from DTIPropDifferences where mainID = @mainid and  objectKey = @objectKey ", changes, MainID, objectKey)
                'Simply used 2selects so that it would fill both keys in order.
            Catch ex As Exception
                If Not sqlhelper.checkDBObjectExists(changes.TableName) Then
                    sqlhelper.checkAndCreateTable(changes)
                End If
            End Try
        End If
    End Sub

    Private Sub createButtons()
        Dim buttons As Generic.List(Of Button) = Session("propbtns_" & MainID & "_" & startingControl.ID)
        If buttons Is Nothing Then
            buttons = New Generic.List(Of Button)
            btnupdate = New Button
            btnsave = New Button
            btnCancel = New Button
            btnrevert = New Button
            btnupdate.ID = "propbtns_" & MainID & "_" & startingControl.ID & "_update"
            btnsave.ID = "propbtns_" & MainID & "_" & startingControl.ID & "_save"
            btnCancel.ID = "propbtns_" & MainID & "_" & startingControl.ID & "_Cancel"
            btnrevert.ID = "propbtns_" & MainID & "_" & startingControl.ID & "_revert"
            buttons.Add(btnupdate)
            buttons.Add(btnsave)
            buttons.Add(btnCancel)
            buttons.Add(btnrevert)
            'Session("propbtns_" & MainID & "_" & startingControl.ID) = buttons
        Else
            btnupdate = buttons(0)
            btnsave = buttons(1)
            btnCancel = buttons(2)
            btnrevert = buttons(3)
        End If
    End Sub

    Private Sub setgrid()
        If dlgChanges Is Nothing Then dlgChanges = New JqueryUIControls.Dialog()
        pnl = dlgChanges

        'pnl.Style("display") = "none"
        'pnl.Style("border") = "2px solid black"
        If Not Me.Height = Nothing Then
            pnl.Height = Me.Height
        Else
            pnl.Height = 500
        End If

        If Not Me.Width = Nothing Then
            pnl.Width = Me.Width
        Else
            pnl.Width = 600
        End If

        createButtons()
        'pnl.Style("overflow") = "auto"
        pnl.ID = "proppnl_" & MainID & "_" & startingControl.ID
        'dd.Items.Add("")
        'For Each ctrl As Control In startingControl.Controls
        '    dd.Items.Add(New ListItem(ctrl.ID & " " & ctrl.GetType().Name, ctrl.ID))
        'Next
        'pnl.Controls.Add(dd)


        btnupdate.Text = "Refresh"
        btnsave.Text = "Save"
        btnCancel.Text = "Cancel Edit"
        btnrevert.Text = "Revert"

        pnl.Controls.Add(New LiteralControl("<br/>"))
        pnl.Controls.Add(btnupdate)
        pnl.Controls.Add(btnsave)
        pnl.Controls.Add(btnCancel)
        pnl.Controls.Add(New LiteralControl("</div>"))
        propeditor.xInstance = startingControl

        Dim pnl1 As New Panel
        'pnl1.Style("border") = "2px solid black"
        'pnl1.Style("overflow") = "auto"
        'If Not Me.Height = Nothing Then pnl1.Height = Me.Height
        'If Not Me.width = Nothing Then pnl1.Width = Me.width
        pnl1.CssClass = "propEditorTable"
        pnl1.Controls.Add(propeditor)
        pnl.Controls.Add(pnl1)

        Me.Controls.Add(pnl)

        pnlChanges = New Panel
        pnlChanges.Style("display") = "none"
        pnlChanges.ID = "propPendingChanges_" & MainID & "_" & startingControl.ID
        If litChangeList Is Nothing Then litChangeList = New LiteralControl
        pnlChanges.Controls.Add(litChangeList)

        pnlSavedChanges = New Panel
        pnlSavedChanges.Style("display") = "none"
        pnlSavedChanges.ID = "propSavedChanges_" & MainID & "_" & startingControl.ID
        pnlSavedChanges.Controls.Add(btnrevert)
        pnlSavedChanges.Controls.Add(New LiteralControl("<br>"))
        If litSavedChangeList Is Nothing Then litSavedChangeList = New LiteralControl
        pnlSavedChanges.Controls.Add(litSavedChangeList)

        pnl.Controls.AddAt(0, pnlSavedChanges)
        pnl.Controls.AddAt(0, pnlChanges)
        pnl.Controls.AddAt(0, New LiteralControl("<a style=""font-size: x-small;"" href=""javascript:void(0);"" onclick=""$('#" & pnlSavedChanges.ClientID & "').toggle('slow');"">Saved Changes</a> "))
        pnl.Controls.AddAt(0, New LiteralControl("<a style=""font-size: x-small;"" href=""javascript:void(0);"" onclick=""$('#" & pnlChanges.ClientID & "').toggle('slow');"">Current Changes</a> "))
        pnl.Controls.AddAt(0, New LiteralControl("<div class='propEditorButtons'>"))
        'dlgChanges.Width = 600

        dlgChanges.OpenerAttributes = "style='font-size: x-small;'"

        dlgChanges.OpenerText = "<i class='fa fa-wrench' aria-hidden='true'></i> Edit Properties"
        Me.Controls.AddAt(0, dlgChanges)
        'Me.Controls.AddAt(0, New LiteralControl(dlgChanges.openerLink("Edit Properties")))
        'Me.Controls.AddAt(0, New LiteralControl("<a style=""font-size: x-small;"" href=""#"" onclick=""toggleProperties('" & pnl.ClientID & "');"">Edit Properties</a> "))
        'Me.Controls.Add(getScript)

        'ajaxSubmitButtonSearchString('.{0}', $('#{1}'), '<!--##{0}##-->')

        jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButtonSearchString('#{0}', $('#{1}'), '<!--'+'##{0}##-->');", startingControl.ClientID, Me.btnupdate.ClientID))
        jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButtonSearchString('#{0}', $('#{1}'), '<!--'+'##{0}##-->');", startingControl.ClientID, Me.btnsave.ClientID))
        jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButtonSearchString('#{0}', $('#{1}'), '<!--'+'##{0}##-->');", startingControl.ClientID, Me.btnrevert.ClientID))
        jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButtonSearchString('#{0}', $('#{1}'), '<!--'+'##{0}##-->');", startingControl.ClientID, Me.btnCancel.ClientID))


        'jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButton('#{0}', $('#{1}'));", startingControl.ClientID, Me.btnupdate.ClientID))
        'jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButton('#{0}', $('#{1}'));", startingControl.ClientID, Me.btnsave.ClientID))
        'jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButton('#{0}', $('#{1}'));", startingControl.ClientID, Me.btnrevert.ClientID))
        'jQueryLibrary.jQueryInclude.addScriptBlockPageLoad(Me.Page, String.Format("ajaxSubmitButton('#{0}', $('#{1}'));", startingControl.ClientID, Me.btnCancel.ClientID))

        ' ajaxSubmitButton('#ctl09_Chart1', $('#ctl09_propbtns_0_Chart1_update'));
        RaiseEvent gridSet()
        Me.Height = Nothing
        Me.Width = Nothing
    End Sub

    Public Shared Sub addHtmlCommentBeforAndAfter(ByVal ctrl As Control, ByVal str As String)
        If Not ctrl.Parent Is Nothing Then
            Dim parentIndex As Integer = ctrl.Parent.Controls.IndexOf(ctrl)
            ctrl.Parent.Controls.AddAt(parentIndex, New LiteralControl("<!--" & str & "-->"))
            parentIndex = ctrl.Parent.Controls.IndexOf(ctrl)
            ctrl.Parent.Controls.AddAt(parentIndex + 1, New LiteralControl("<!--" & str & "-->"))
            BaseClasses.DataBase.setCssClasstoId(ctrl)
        End If
    End Sub

    Public Function displayChangeList(ByVal dt As ComparatorDS.DTIPropDifferencesDataTable) As String
        Dim str As String = ""
        For Each row As ComparatorDS.DTIPropDifferencesRow In dt
            Dim val As Object = Comparator.desearializeFromBase64String(row)
            If val Is Nothing Then val = "{null}"
            str &= row.PropertyPath & " : " & Me.Page.Server.HtmlEncode(val.ToString) & "<br>" & vbCrLf
        Next
        Return str
    End Function

    '    Private Function getScript() As LiteralControl
    '        script = New LiteralControl("<script type=""text/javascript"">  " & vbCrLf & _
    '"var divname; " & vbCrLf & _
    '"function toggleProperties(id){  " & vbCrLf & _
    '"    divname=id;  " & vbCrLf & _
    '"    toggle(id); " & vbCrLf & _
    '"} " & vbCrLf & _
    '"function toggle(id){  " & vbCrLf & _
    '"	var div1 = document.getElementById(id)  " & vbCrLf & _
    '"	if (div1.style.display == 'none') {  " & vbCrLf & _
    '"		div1.style.display = 'block'  " & vbCrLf & _
    '"	} else {  " & vbCrLf & _
    '"		div1.style.display = 'none'  " & vbCrLf & _
    '"	}  " & vbCrLf & _
    '"}  " & vbCrLf & _
    '"Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandle);  " & vbCrLf & _
    '"Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);  " & vbCrLf & _
    '"var divdisplay;  " & vbCrLf & _
    '"function beginRequestHandle(sender, Args)  " & vbCrLf & _
    '"{   if(divname)  " & vbCrLf & _
    '"        divdisplay=document.getElementById(divname).style.display;  " & vbCrLf & _
    '"    return true;  " & vbCrLf & _
    '"}  " & vbCrLf & _
    '"function endRequestHandle(sender, Args)  " & vbCrLf & _
    '"{                    " & vbCrLf & _
    '"    if(divname)  " & vbCrLf & _
    '"        document.getElementById(divname).style.display = divdisplay;  " & vbCrLf & _
    '"    return true;  " & vbCrLf & _
    '"}  " & vbCrLf & _
    '"</script>" & vbCrLf)
    '        Return script
    '    End Function

    Private Sub btnrevert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnrevert.Click
        getChanges()
        Dim dv As New DataView(changes, "mainID = " & MainID & " and ( objectKey = '" & objectKey & "' OR  objectKey = '" & secondaryKey & "' )", "", DataViewRowState.CurrentRows)
        For Each rv As DataRowView In dv
            rv.Delete()
        Next
        Me.sqlhelper.Update(changes)
        Page.Response.Redirect(Page.Request.Url.ToString)
    End Sub

    Private Sub btnsave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsave.Click
        getChanges()
        Dim dv As New DataView(changes, "mainID = " & MainID & " and  objectKey = '" & objectKey & "'", "", DataViewRowState.CurrentRows)
        For Each rv As DataRowView In dv
            rv.Delete()
        Next
        Me.sqlhelper.Update(changes)
        propeditor.changes.Clear()
        propeditor.UpdateInstance()
        getChanges()
        Dim ht As New Hashtable
        For Each row As ComparatorDS.DTIPropDifferencesRow In propeditor.changes
            ht.Add(row.PropertyPath, row.PropertyPath)
            Dim updaterow As ComparatorDS.DTIPropDifferencesRow = BaseProperty.findDiffRow(changes, MainID, row.PropertyPath, objectKey)
            If updaterow Is Nothing Then
                changes.AddDTIPropDifferencesRow(row.PropertyPath, row.PropertyValue, row.PropertyType, Me.objectKey, Me.MainID)
            Else
                updaterow.PropertyValue = row.PropertyValue
                updaterow.PropertyType = row.PropertyType
            End If
        Next
        For Each row As ComparatorDS.DTIPropDifferencesRow In changes
            If Not ht.Contains(row.PropertyPath) Then
                If row.mainID = Me.MainID AndAlso row.ObjectKey = Me.objectKey Then
                    row.Delete()
                End If
            End If
        Next
        Me.sqlhelper.Update(changes)
        Me.Page.Response.Redirect(Me.Page.Request.Url.ToString)
    End Sub

    Private blockPrerenderUpdate As Boolean = False
    Private Sub btnupdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnupdate.Click
        propeditor.UpdateInstance()
        pnl.Style("display") = "block"
        blockPrerenderUpdate = True
    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        propeditor.changes.Clear()
    End Sub

    Private Sub propeditor_toggleSubProp(ByVal propPath As String, ByVal isexpanded As Boolean) Handles _propeditor.toggleSubProp
        pnl.Style("display") = "block"
        If dlgChanges Is Nothing Then dlgChanges = New JqueryUIControls.Dialog()
        dlgChanges.AutoOpen = True
    End Sub

    Private Sub mypage_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles mypage.LoadComplete
        If Not manuallySetProperties Then _
        setProperties()
    End Sub

    Private Sub mypage_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles mypage.PreRenderComplete
        If Not manuallySetProperties Then _
        setProperties()
        If startingControl IsNot mypage Then
            addHtmlCommentBeforAndAfter(startingControl, "##" & startingControl.ClientID & "##")
        End If
    End Sub
End Class
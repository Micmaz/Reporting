<%@ Page Language="vb" ValidateRequest="false" AutoEventWireup="false" CodeBehind="ReportGraphs.aspx.vb" Inherits="Reporting.ReportGraphs" %>
<%@ Register Assembly="DTIControls" Namespace="DTIMiniControls" TagPrefix="DTI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
     <script type="text/javascript">
            function ToggleDisplay(id) {
                var tbid = $('#d' + id).find('.tbSqlStmt').attr('id');
                //eval("editor_" + tbid + ".refresh();");
	            //var elem = document.getElementById('d' + id);
	            $('#d' + id).toggle("slow");// , function () {
	                eval("editor_" + tbid + ".refresh();");
	                eval("editor_" + tbid + ".refresh();"); //This is not a mistake. Refresh must be called twice.
                //});
	            //var tbid = $('#d'+id).find('.tbSqlStmt').attr('id');
	            //setTimeout("editor_"+tbid+".refresh();",1000);
	        } 
function openTester(url){
    url = "~/res/Reporting/GraphTester.aspx?reportname=" + $.urlEncode($('#hidReportID').val()) + "&sql=" + $.urlEncode(url);
    parent.createDialogURL(url,750,850,'sqlTester','SQL Tester')
}
function openParmEditor(graphID) {
    url = "~/res/Reporting/ReportParms.aspx?graphID=" + graphID;
    parent.createDialogURL(url, 400, 550, 'GraphParms', 'Graph Parms')
}
function setEditorVal(graphID, Value) {
    Value = "--Select Y-Axis, X-Axis, OtherAxisOrParm  \r\n" + Value;
    $("#d"+graphID).find(".tbSqlStmt").data("editor").setValue(Value);
}

var addEvent = function () { return document.addEventListener ? function (a, b, c) { if (a && a.nodeName || a === window) a.addEventListener(b, c, !1); else if (a && a.length) for (var d = 0; d < a.length; d++) addEvent(a[d], b, c) } : function (a, b, c) { if (a && a.nodeName || a === window) a.attachEvent("on" + b, function () { return c.call(a, window.event) }); else if (a && a.length) for (var d = 0; d < a.length; d++) addEvent(a[d], b, c) } }();

$(function () {

    addEvent(window, 'storage', function (event) {
        if (event.key.startsWith('Graph_')) {
            var graphid = event.key.replace("Graph_","");
            setEditorVal(graphid, event.newValue);
            alert("Graph Query has been updated.");
        }
    });

    addButtonsFromFrame({ '.btnSave': function () { return true; }, "Cancel": function () { return false; } });
    $(".accordion").accordion({ collapsible: true, active: false, heightStyle: "content", activate:
        function (event, ui) {
            var tbid = ui.newPanel.find('.tbSqlStmt').attr('id');
            if (tbid) {
                eval("editor_" + tbid + ".refresh();");
                eval("editor_" + tbid + ".refresh();"); //This is not a mistake. Refresh must be called twice.
            }
        }
    });
    $(".toolbar a").live("click", function () {
        window.location = $(this).attr("href");
    });
    $(".toolbar input").button();
    $.ui.accordion.prototype._keydown = function (event) { return; };  //This is a fix for pressing space in the accordian textboxes
    $(".btnDelete").click(function (e) { e.stopPropagation(); return true; });
});


        </script>
         <style type="text/css">
.CodeMirror {
    font-size: 15px;
    line-height: 16px;
}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input id="Button1" type="button" onclick="window.location='ReportsEdit.aspx';return false;" value="< Report List" /><br />
        <span style="width:100px">Report Name:</span><asp:HiddenField ID="hidReportID" runat="server" />
        <asp:TextBox ID="tbReportName" Width="200px" runat="server"></asp:TextBox>
        <asp:Button ID="btnAdd" runat="server" Text="Add Graph" /><br />
          <div class="accordion"> 
        <asp:Repeater ID="repeater1" runat="server">
            <HeaderTemplate></HeaderTemplate>
            <ItemTemplate>
            <span>
                Graph Name: <asp:TextBox ID="tbGraphName" Width="200px" runat="server"></asp:TextBox>
<asp:Button ID="btnDelete" style="float:right;" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'
                            CommandName="btnDelete" CssClass="submit" Text="X" />
                                        </span>
                    <div id="d<%# DataBinder.Eval(Container.DataItem, "Id") %>" class="graphList" style="display:none">
                        <table>
                            <tr>
                                <td style="width:100px; vertical-align:top; text-align:right;">Sql Statement:</td>
                                <td><DTI:HighlighedEditor language="plsql" Height="300px" Width="500px" ID="tbSqlStmt" runat="server"> </DTI:HighlighedEditor>
                                
                                <a href="javascript:void(0);" onclick="openTester($($(this).parent().children()[0]).val());"><i class="fa fa-table"></i> Test SQL</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <a href="javascript:void(0);" onclick="openParmEditor(<%# DataBinder.Eval(Container.DataItem, "Id") %>);"><i class="fa fa-terminal"></i> Parm Editor</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <a href="QueryBuilder.aspx?graphID=<%# DataBinder.Eval(Container.DataItem, "Id") %>" target="_blank" ><i class="fa fa-sitemap"></i> Query Builder</a>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">Type:</td>
                                <td><asp:DropDownList ID="ddlType" runat="server"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td align="right">Order:</td>
                                <td><asp:TextBox ID="tbOrder" Width="50px" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">Drillable:</td>
                                <td><asp:CheckBox ID="cbDrillable" runat="server" /></td>
                            </tr>
                             <tr>
                                <td align="right">Export Link:</td>
                                <td><asp:CheckBox ID="cbExport" runat="server" /></td>
                            </tr>                         
                        </table>
                        <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                    <br /><br /><br />
                    </div>
            </ItemTemplate>
            <FooterTemplate></FooterTemplate>
        </asp:Repeater>
          </div> 
        &nbsp;<br />
        &nbsp;<asp:Button ID="btnSave" CssClass="btnSave" runat="server" Text="Save" /></div>
    </form>
</body>
</html>

<%@ Page Language="vb" validateRequest="false" AutoEventWireup="false" CodeBehind="ReportsEdit.aspx.vb" Inherits="Reporting.ReportsEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
 span.toolbar.ui-widget-header.ui-corner-all {
margin: -3px;
}   
    </style>
     <script type="text/javascript">
            function ToggleDisplay(id) 
	        { 
	            //var elem = document.getElementById('d' + id); 
	            $('#d' + id).toggle("slow");
	        }
	        $(function () {
	            addButtonsFromFrame({ '.btnSave': function () { return true; }, "Cancel": function () { return false; } });
	            $(".accordion").accordion({ collapsible: true, active: false, heightStyle: "content" });
	            $(".toolbar a").live("click", function () {
	                window.location = $(this).attr("href");
	            });
	            $.ui.accordion.prototype._keydown = function (event) {return;};  //This is a fix for pressing space in the accordian textboxes
	        });

	        function CopyReport(ReportID) {
	            $("<div><p>").html("Please Enter a name for the new report: <input type='text' id='newReportName'>").dialog({
	                resizable: false,
	                height: 240,
	                modal: true,
	                buttons: {
	                    "OK": function () {
	                        window.location.href = window.location.href.replace("#", "") + "?cpid=" + ReportID + "&NewName=" + $('#newReportName').val();
	                        $(this).dialog("close");
	                        return true;
	                    },
	                    Cancel: function () {
	                        $(this).dialog("close");
	                        return false;
	                    }
	                }
	            });

	        }


        </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnAdd" runat="server" Text="Add Report" /><br />
  <div class="accordion">   
        <asp:Repeater ID="repeater1" runat="server">
            <HeaderTemplate>
             
            </HeaderTemplate>
            <ItemTemplate>

                        <span><asp:TextBox ID="tbReportName" Width="200px" runat="server"></asp:TextBox></span>
                    <div id="d<%# DataBinder.Eval(Container.DataItem, "Id") %>" style="display:none">
                    <asp:TextBox ID="tbWidth" runat="server" Visible="false" />
                    <asp:TextBox ID="tbHeight" runat="server" Visible="false" />
                    <asp:CheckBox ID="cbScrollable" runat="server" Visible="false" />
                            <span style="float:right;padding:5px;" class="toolbar ui-widget-header ui-corner-all">
                                <a href='~/res/Reporting/ReportGraphs.aspx?repid=<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Edit Graphs</a>
                            </span>
                        <table>
<%--                        <tr>>
                                <td style="width:100px; vertical-align:top; text-align:right;">Width:</td>
                                <td><asp:TextBox ID="tbWidth" runat="server" Width="50px" ></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">Height:</td>
                                <td><asp:TextBox ID="tbHeight" runat="server" Width="50px" ></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">Scrollable:</td>
                                <td><asp:CheckBox ID="cbScrollable" runat="server" /></td>
                            </tr>
--%>
                            <tr>
                                <td align="right">Scroll to the last graph clicked:</td>
                                <td><asp:CheckBox ID="cbShowHistory" runat="server" /></td>
                            </tr>
                            <tr>
                                <td align="right">Published:</td>
                                <td><asp:CheckBox ID="cbPublished" runat="server" /></td>
                            </tr>
                            <tr><td>
                                <input type="button" value="Copy" onclick="CopyReport(<%# DataBinder.Eval(Container.DataItem, "Id") %>); return false;" /> 
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'
                            CommandName="btnDelete" CssClass="submit" Text="Delete" />
                                </td>
                            </tr>                                 
                        </table>
                        <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                    <br /><br /><br />
                    </div>
                         
            </ItemTemplate>
            <FooterTemplate>
               
            </FooterTemplate>
        </asp:Repeater>
 </div>    

        &nbsp;<br />
        &nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" /></div>
    </form>
</body>
</html>

<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReportParms.aspx.vb" Inherits="Reporting.ReportParms" %>
<%@ Register Assembly="DTIControls" Namespace="DTIMiniControls" TagPrefix="DTI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    
     <script type="text/javascript">
            function ToggleDisplay(id) 
	        { 
	            //var elem = document.getElementById('d' + id); 
	            $('#d' + id).toggle("slow");
	            //var tbid = $('#d'+id).find('[id$=tbSqlStmt]').attr('id');
	        }

	        $(function() {
	            $("select").change(function() {
	                setOptions($(this));
	            })
	            $("select").each(function () { setOptions($(this)); });
	            $(".accordion").accordion({ collapsible: true, active: false, heightStyle: "content" });
	            $(".toolbar a").live("click", function () {
	                window.location = $(this).attr("href");
	            });
	            $.ui.accordion.prototype._keydown = function (event) { return; };  //This is a fix for pressing space in the accordian textboxes
	            $(".btnDelete").click(function (e) { e.stopPropagation(); return true; })
	            addButtonsFromFrame({ '.btnSave': function () { return true; }, "Cancel": function () { return false; } });
	        });

	        function setOptions(itm) {
	            var row = itm.parent().parent().next();
	            var labelcell = row.children(".label");
	            var tb = row.find("textarea");
	            if (itm.val() < 2) { row.hide(); } else { row.show(); }
	            if (itm.val() == 3) {
	                labelcell.html("<b>DropDown Items:</b><br> Enter seperated<br/> <br/><i>display#value</i> or <i>value</i><br/><br/> ")
	                tb.attr("rows", 5);
	                tb.css("overflow", "visiable");
	            } else if (itm.val() == 2) {
	                labelcell.html("<b>Edit mask </b><i>999</i>:");
	                tb.attr("rows", 1);
	                tb.css("overflow", "hidden");
	            } else if (itm.val() == 4) {
	                labelcell.html("<b>Table*:<br>Column*:<br>Number returned </b><i>20</i>:<br><b>Search Pattern </b><i>%{0}%</i>");
	                tb.attr("rows", 4);
	                tb.css("overflow", "hidden");
	            } else if (itm.val() == 6) {
	                labelcell.html("<b>Table or SQL*:<br>Value Column,Display Col*:<br>Number returned </b><i>20</i>:<br><b>Search Pattern </b><i>%{0}%</i>");
	                tb.attr("rows", 4);
	                tb.css("overflow", "hidden");
	            } else if (itm.val() == 5) {
	                labelcell.html("<b>Table or SQL*:<br>Value Column,Display Col*:<br>");
	                tb.attr("rows", 2);
	                tb.css("overflow", "hidden");
	            }
	        }
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
        Graph<span style="width:100px"> Name:</span><asp:HiddenField ID="hidReportID" runat="server" />
        <asp:Label ID="tbReportName" runat="server"/>
        <asp:Button ID="btnAdd" runat="server" Text="Add Parm" />&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnRegen" runat="server" Text="Regenerate Parms" /><br />
        <div class="accordion"> 
        <asp:Repeater ID="repeater1" runat="server">
            <HeaderTemplate></HeaderTemplate>
            <ItemTemplate>
            <span>Display Name: <asp:TextBox ID="tbDisplay" Width="200px" runat="server"></asp:TextBox>
            <asp:Button ID="btnDelete" style="float:right;" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'
                            CommandName="btnDelete" CssClass="submit" Text="X" />
            </span>
                    <div id="d<%# DataBinder.Eval(Container.DataItem, "Id") %>" style="display:none">
                        <table>
                        <tr>
                        <td align="right">Parm Name: @</td>
                        <td><asp:TextBox ID="tbName" Width="200px" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                                <td valign="top" align="right">Type:</td>
                                <td valign="top">
                                <asp:DropDownList ID="ddlType" runat="server">
                                        <asp:ListItem Text="TextBox" Value="0"/>
                                        <asp:ListItem Text="Datepicker" Value="1"/>
                                        <asp:ListItem Text="NumberPicker" Value="2"/>
                                        <asp:ListItem Text="DropDown" Value="3"/>
                                        <asp:ListItem Text="DropDown From Table" Value="5"/>
                                        <asp:ListItem Text="AutoComplete (Distinct)" Value="4"/>
                                        <asp:ListItem Text="AutoComplete from Table" Value="6"/>
                                </asp:DropDownList>
                                </td>
                        </tr>
                        <tr>
                        <td align="right" class="label">Parm Options: </td>
                        <td><asp:TextBox ID="tbOptions" Width="200px" runat="server" TextMode="MultiLine"></asp:TextBox></td>
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
        &nbsp;<asp:Button ID="btnSave" CssClass="btnSave" runat="server" Text="Save" /></div>
    </form>
</body>
</html>

<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="QueryBuilder.aspx.vb" Inherits="Reporting.QueryBuilder" %>
<!DOCTYPE HTML> 
<html lang="en"> 
    <head runat="server">
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
        <title>HTML5 Query Builder</title>
		
		<link href="//maxcdn.bootstrapcdn.com/font-awesome/4.1.0/css/font-awesome.min.css" rel="stylesheet">

		
        <!-- jquery -->

		<!--JSTREE-->
		<link rel="stylesheet" href="~/res/BaseClasses/Scripts.aspx?f=Reporting/jstreeTheme/style.min.css" />
		<script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/jstree.min.js"></script>
		
		<!--JSPlumb-->
		<script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/jquery.jsPlumb-1.6.2-min.js"></script>	
		
		
		<!--jquery.layout-->
		<script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/jquery.layout-latest.min.js"></script>
		<link rel="stylesheet" href="~/res/BaseClasses/Scripts.aspx?f=Reporting/layout-default-latest.css" />

		<link rel="stylesheet" href="~/res/BaseClasses/Scripts.aspx?f=Reporting/tablethemes/blue/style.css" type="text/css" />

		<!--jquery.dragtable-->
		<script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/jquery.dragtable.js"></script>

		<!--jquery.dropit -->
		<script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/dropit.js"></script>
		
		
        <!--jQuery.extendext -->
		<script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/jQuery.extendext.min.js"></script>

        <script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/QueryBuilder.js"></script>
        <script src="~/res/BaseClasses/Scripts.aspx?f=Reporting/QueryBuilder.bgTable.js"></script>
        <link href="~/res/BaseClasses/Scripts.aspx?f=Reporting/queryBuilder.css" rel="stylesheet">


    </head>
    <body>
        <form id="form1" runat="server">

		
<script>
    function parseQuery(qstr) {
        var query = {};
        var a = (qstr[0] === '?' ? qstr.substr(1) : qstr).split('&');
        for (var i = 0; i < a.length; i++) {
            var b = a[i].split('=');
            query[decodeURIComponent(b[0])] = decodeURIComponent(b[1] || '');
        }
        return query;
    }
    //Overwrite the table schema file location
    schemaExtendedJsonCall = "<%=HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/")%>/tables.json.js"
    $(function () {
        var button = $("<button>")
            .text("Use this SQL")
            .css({ "top": 0, "right": 0, "position": "absolute" })
            .click(function () {
                if (getQueryVariable("graphID") != "") {
                    localStorage.setItem("Graph_" + getQueryVariable("graphID"), getSQLString());
                    
                    return false;
                }
            })
        $(".sql").parent().append(button);
    });
    
    function getQueryVariable(variable) {
        var query = window.location.search.substring(1);
        var vars = query.split('&');
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split('=');
            if (decodeURIComponent(pair[0]) == variable) {
                return decodeURIComponent(pair[1]);
            }
        }
        console.log('Query variable %s not found', variable);
    }

    function getSQLString() {
        return $(".sql").html().replace(/<br>/g, "\n");
    }

    $(function () {
        $(document).bind('mousedown selectstart', function (e) {
            return $(e.target).is('input, textarea, select, option, html');
        });
        buildTable();
        $('body').layout({
            applyDefaultStyles: false
		, west__togglerLength_closed: '100%'
		, west__spacing_closed: 20
		, south__togglerLength_closed: '100%'
		, south__spacing_closed: 20
		, east__togglerLength_closed: '100%'
		, east__spacing_closed: 20
        });
        var sqlClick;
        $(".sql").click(function (e) {
            if (!$(e.target).is('input, textarea, select, option, html')) {
                window.clearTimeout(sqlClick);
                sqlClick = setTimeout(function () {
                    $(".sql").html("<textarea style='margin: -8px;width:" + ($(".sql").width()) + "px;height:" + ($(".sql").height()) + "px;'>" + $(".sql").html().replace(/<br>/g, "\n") + "</textarea>");
                }, 100);
            }
        });

        makeToggle("#jstree", "Tables", loadData);
        //loadData();

        var to = false;
        $('#searchnodes').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#searchnodes').val();
                $('.jstree').each(function () {
                    if ($(this).jstree().search)
                        $(this).jstree().search(v);
                });
            }, 250);
        });

        $("body").click(function (e) {
            if (!$(e.target).is('input, textarea, select, option, html'))
                updateSql();
            //$(".colddmenu").hide();
        });
        makeToggle("#views", "Views", addViewData);

        makeToggle("#otherDB", "Other Databases", addOtherDatabases);
        /*	$("#viewToggle").click(function(){
                addViewData();
                $(this).next().slideToggle();
                $(this).find("li").toggleClass("jstree-open").toggleClass("jstree-closed");
            });
    */
    });
</script>
		
            <!-- development area -->
  <div class="tableList ui-layout-west">
  search:
  <a href="#" onclick="$('#searchnodes').val('');$('.jstree').each(function () { $(this).jstree().search(''); });" style="float:right;margin-right: 6px;">clear</a>
  <br/>
  <input type="text" id="searchnodes" value class="input">

    <div id="jstree" class="jstree-default tableGroup"></div>

    <div id="views" class="jstree-default tableGroup"></div>
    
    <div id="otherDB" class="jstree-default tableGroup"></div>
  </div>
  
  <div class="dropzone drop ui-layout-center">
  
  	<table cellspacing="1" class="tablesorter">             
		<thead><tr></tr></thead> 
		<tbody></tbody> 
	</table>
  
  </div>
            <!-- /development area -->
<div class="ui-layout-south">
    <div class="sql"><br/><br/><br/><br/></div>
</div>

<!-- /Dialogue area -->
<div style="display:none">

	<div id="toggleElement" class="toggleElement jstree-default">
		<ul class="jstree-container-ul">
			<li class="jstree-node jstree-closed jstree-last" >
				<i class="jstree-icon jstree-ocl"></i><a class="jstree-anchor" href="#"><i class="labelAfter jstree-icon jstree-themeicon"></i></a>
			</li>
		</ul>
	</div>

	<i id="loadSpinner" class="hideWhenDone fa fa-cog fa-spin fa-2x fa-fw"></i>

	<div id="joindlg">
		Please select join columns <br><br>
		One row from:
		<div class="floatDiv1"><span id="joindlg_source"></span><br>
		<select id="joindlg_sourcedd">
		</select>
		</div>
		<div class="floatDiv1">joins to many:</div>
		<div class="floatDiv1"><span id="joindlg_dest"></span>
		<br><select id="joindlg_destdd"></select>
		</div>
	</div>

	<div id="concatdlg">
		String builder: <br>
		<input type="text" id="strtxt"><br/>
		Add column from table:
		<br><select id="strtblname"></select>
		<br><select id="strtblcols"></select>
	</div>
	
	
	<div id="colmenu" class="menugrp" >
		<button class="colmenubtn">Column Functions</button>
        <ul class="colddmenu">
            <li><a href="#">Sort Ascending</a></li>
			<li><a href="#">Sort Descending</a></li>
            <!--<li><a colwrap="group" href="#">Group this column</a></li>-->
			<li><a colwrap="count" href="#">Count</a></li>
			<li><a colwrap="sum" href="#">Sum</a></li>
			<li><a colwrap="avg" href="#">Average</a></li>
			<li><a colwrap="min" href="#">Minimum</a></li>
            <li><a colwrap="max" href="#">Maximum</a></li>
			<li><a colwrap="rem" href="#">Remove</a></li>
        </ul>
		
	</div>
</div>
    </form>
    </body>
</html> 



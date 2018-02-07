
/*                                                                                                                 
88888888ba                           88                                                                                 88     888888888888         88           88                   ,ad8888ba,                                                                                                 
88      "8b                          88                                                                                 88          88              88           88                  d8"'    `"8b                                                                 ,d                             
88      ,8P                          88                                                                                 88          88              88           88                 d8'                                                                           88                             
88aaaaaa8P'  ,adPPYYba,   ,adPPYba,  88   ,d8   ,adPPYb,d8  8b,dPPYba,   ,adPPYba,   88       88  8b,dPPYba,    ,adPPYb,88          88  ,adPPYYba,  88,dPPYba,   88   ,adPPYba,     88              ,adPPYba,  8b,dPPYba,    ,adPPYba,  8b,dPPYba,  ,adPPYYba,  MM88MMM  ,adPPYba,   8b,dPPYba,  
88""""""8b,  ""     `Y8  a8"     ""  88 ,a8"   a8"    `Y88  88P'   "Y8  a8"     "8a  88       88  88P'   `"8a  a8"    `Y88          88  ""     `Y8  88P'    "8a  88  a8P_____88     88      88888  a8P_____88  88P'   `"8a  a8P_____88  88P'   "Y8  ""     `Y8    88    a8"     "8a  88P'   "Y8  
88      `8b  ,adPPPPP88  8b          8888[     8b       88  88          8b       d8  88       88  88       88  8b       88          88  ,adPPPPP88  88       d8  88  8PP"""""""     Y8,        88  8PP"""""""  88       88  8PP"""""""  88          ,adPPPPP88    88    8b       d8  88          
88      a8P  88,    ,88  "8a,   ,aa  88`"Yba,  "8a,   ,d88  88          "8a,   ,a8"  "8a,   ,a88  88       88  "8a,   ,d88          88  88,    ,88  88b,   ,a8"  88  "8b,   ,aa      Y8a.    .a88  "8b,   ,aa  88       88  "8b,   ,aa  88          88,    ,88    88,   "8a,   ,a8"  88          
88888888P"   `"8bbdP"Y8   `"Ybbd8"'  88   `Y8a  `"YbbdP"Y8  88           `"YbbdP"'    `"YbbdP'Y8  88       88   `"8bbdP"Y8          88  `"8bbdP"Y8  8Y"Ybbd8"'   88   `"Ybbd8"'       `"Y88888P"    `"Ybbd8"'  88       88   `"Ybbd8"'  88          `"8bbdP"Y8    "Y888  `"YbbdP"'   88          
                                                aa,    ,88                                                                                                                                                                                                                                       
                                                 "Y8bbdP"                                                                                                                                                                                                                                                                                                                                                                                                         
*/

var groupedCols = {};
var sortCols = {};
var sortColOrder = [];
var colOrder = [];

function columnGroupFunction(event, ui) {
    var col = $(ui.item).parent().parent().parent().data().col;
    var func = $(ui.item).text();
    if (func == "Sort Ascending") { addOrd(col, "asc", sortCols, sortColOrder); }
    if (func == "Sort Descending") { addOrd(col, "desc", sortCols, sortColOrder); }
    if (func == "Count") { addOrd(col, "count", groupedCols); }
    if (func == "Minimum") { addOrd(col, "min", groupedCols); }
    if (func == "Maximum") { addOrd(col, "max", groupedCols); }
    if (func == "Average") { addOrd(col, "avg", groupedCols); }
    if (func == "Sum") { addOrd(col, "sum", groupedCols); }
}

function addOrd(col, value, list, orderlist) {
    if (orderlist) {
        orderlist.remove(col.name);
        orderlist.unshift(col.name);
    }
    list[col.name] = value;
}


function reorderCols(event, ui) {
    colOrder = [];
    $(ui.column).parent().find("th").each(function () {
        colOrder.push($(this).data().col.name);
    });
}

var prevSql = "";
function buildTable(sql) {
	if (!sql) sql = "";
	if (sql.trim() == "select *  \nfrom") sql = "";
	if (sql != "" && sql != prevSql) {
		prevSql = sql;
		$.getJSON(tableDataJsonCall, { sql: encodeURIComponent(sql) }, function (data) {
			if (data) {
				$("table.tablesorter thead tr").empty();
				$("table.tablesorter tbody").empty();
				tabledata = data;
				var htmlString = "";
				$.each(data.columns, function (i, col) {
					var th = $("<th>");
					th.text(col.name);
					th.attr("colType", col.type);
					th.attr("colname", col.name);
					th.data("col", col);
					$("table.tablesorter thead tr").append(th);
				});
				$.each(data.rows, function (i, row) {
					var tr = $("<tr>");
					$.each(row, function (key, val) {
						tr.append($("<td>").append($("<div class='cellvalue'>").text(val)));
						//$("<div class='cellvalue'>").text(val).appendTo(tr);
					});
					$("table.tablesorter tbody").append(tr);
				});
				$(".tablesorter th").each(function (index, column) {
					$(this).prepend("<span class='dragtable-drag-handle'>&nbsp;</span>");
					var colmenuID = "colmenu" + index;
					var colmnu = $("#colmenu").clone().attr("id", colmenuID);
					var btn = colmnu.children().first();
					var menu = btn.next();
					menu.menu({ select: columnGroupFunction });

					btn.button({
						icons: { primary: 'ui-icon-triangle-1-s' },
						text: false
					}).click(function () {
						//menu.slideToggle();
						$(".colddmenu").not(menu).hide();
						menu.css("top", "-500px").css("left", "-500px")
						menu.slideToggle("fast");
						menu.position({
							'my': 'right top',
							'at': 'right bottom',
							'of': $(this),
							'collision': 'fit flip'
							//,'within': $(".dataPreview")
						});
						return false;
					});
					$(this).append(colmnu);
				});

				//$('.colmenu1').menu();   //dropit({ action: 'mouseenter' });
				$(".tablesorter").dragtable({
					placeholder: 'dragtable-col-placeholder test3',
					items: 'thead th:not( .notdraggable ):not( :has( .dragtable-drag-handle ) ), .dragtable-drag-handle',
					appendTarget: $(".tablesorter"),
					change: reorderCols,
					scroll: true
				});
			}
		}).fail(ajaxError);
	}
}

/*
                                                                                                                    
 ad88888ba     ,ad8888ba,    88              88888888ba                88  88           88                          
d8"     "8b   d8"'    `"8b   88              88      "8b               ""  88           88                          
Y8,          d8'        `8b  88              88      ,8P                   88           88                          
`Y8aaaaa,    88          88  88              88aaaaaa8P'  88       88  88  88   ,adPPYb,88   ,adPPYba,  8b,dPPYba,  
  `"""""8b,  88          88  88              88""""""8b,  88       88  88  88  a8"    `Y88  a8P_____88  88P'   "Y8  
        `8b  Y8,    "88,,8P  88              88      `8b  88       88  88  88  8b       88  8PP"""""""  88          
Y8a     a8P   Y8a.    Y88P   88              88      a8P  "8a,   ,a88  88  88  "8a,   ,d88  "8b,   ,aa  88          
 "Y88888P"     `"Y8888Y"Y8a  88888888888     88888888P"    `"YbbdP'Y8  88  88   `"8bbdP"Y8   `"Ybbd8"'  88          
                                                                                                                    
                                                                                                                    
*/

function updateSql() {
    //$.cookie('layout_'+document.URL,$(".dropzone").html());
    $(".sql").html(buildSql().replace(/(?:\r\n|\r|\n)/g, '<br />'));
}

function clearSql() {
    $(".sql").html("");
    lastSQL = "";
    groupedCols = {};
    sortCols = {};
    sortColOrder = [];
    colOrder = [];
    buildTable(lastSQL);
}

var lastSQL = "";
function buildSql() {
    var sql = "";
    var columns = "";
    var colList = [];
    var lowercolList = [];
    tblList = [];
    var colSelects = {};
    var tableElems = $(".drop").find(".w");
    if(tableElems.length==0) clearSql();

    for (var ct = 0; ct < tableElems.length; ct++) {
        //$(".drop").find(".w").each(function () {
        //var tblElem = $(this);
        var tblElem = $(tableElems[ct]);
        var sqltable = tblElem.attr("tableName");
        var table = tblElem.attr("id");
        var cols = tblElem.jstree('get_selected');
        tblList.push(table);

        for (var i = 0; i < cols.length; i++) {
            var c = cols[i];
            if (c != table) {
                var data = tblElem.jstree().get_node(c).data.jstree
                if (!data)
                    data = tblElem.jstree().get_node(c).data
                var select = data.select.replace(table, table.replace("XX", "_"));
                var colname = data.name;
                if ($.inArray(colname.toLowerCase(), lowercolList) > -1) {
                    colname = c;    
                }
                colname = colname.replace("XX", "_").replace(".", "_")
                select = select.replace(table, table.replace("XX", "_"));
                colSelects[colname] = select;
                lowercolList.push(colname.toLowerCase());
                //$("#"+c).data()
            }
        }
    }

    $.each(colOrder, function (index, colname) {
        if (colSelects[colname]) {
            columns += "," + colSelects[colname] + " as " + colname;
            delete colSelects[colname];
            colList.push(colname);
        }
    });
    for (colname in colSelects) {
        columns += "," + colSelects[colname] + " as " + colname;
        colList.push(colname);
    }
    colOrder = colList;
    //sort tables so that parent tables are in the correct order. Tables with more connections are placed first.
    tblList.sort(function (a, b) {
        var ctA = instance.getConnections({ target: a }).length;
        var ctB = instance.getConnections({ target: b }).length;
        if (ctA == ctB) { return 0; }
        //sort descending
        if (ctA < ctB) return 1;
        else return -1;
    });

    if (tblList.length > 0) {
        var tblid = tblList[0];
        sql += buildJoin(tblList[0], getTableStr(tblid));
    }
    while (tblList.length > 0) {
        var tblid = tblList[0];
        sql += buildJoin(tblid, "",true)
    }

    var sortBy = "";
    $.each(sortColOrder, function (i, val) {
        var colname = sortColOrder[i];
        if (columns == "" || $.inArray(colname, colList) > -1)
            sortBy += ", " + colname + " " + sortCols[colname];
    });


    if (columns == "") columns = " * "
    sql = "select " + columns.substring(1) + " \nfrom \n" + sql;

    //If there's a grouping add an outer select
    if (Object.keys(groupedCols).length > 0) {
        if (columns == " * ") {

        }
        var groupby = "";
        var groupbyColumnLst = "";
        $.each(colList, function (i, val) {
            var colname = colList[i];
        
            if (groupedCols[colname]) {
                groupbyColumnLst += "," + groupedCols[colname] + "(" + colname + ") as " + colname;
            } else {
                groupbyColumnLst += "," + colname;
                groupby += "," + colname;
            }
        });
		sql = "Select " + groupbyColumnLst.substr(1) + "\n from (\n" + sql + "\n) as GroupTbl "
		if (groupby.substr(1) != "")
			sql += " \ngroup by " + groupby.substr(1) + " ";
    }

    
    if (sortBy.length > 0) sortBy = "\norder by " + sortBy.substr(1);
    sql = sql + sortBy;
    sql = sql.replace(/XX/g, ".");


    //console.log(sql)
    if (lastSQL != sql) {
        lastSQL = sql;
        buildTable(lastSQL);
    }

	if (sql == "select *  \nfrom \n")
		sql = "";
    return sql;
}


function getTableStr(tblid) {
    var sqltable = $("#" + tblid).attr("tableName");
    return sqltable + " as " + tblid.replace("XX", "_")
}

function getSchemaRelations(){
    var schema = [];
    $(".drop").find(".w").each(function(){
        schema.push($(this).attr("tableName"))
    })
    return schema;
}

/*                                                                                              
                                                                                                                                         
88888888ba                88  88           88             88               88                   ad88888ba     ,ad8888ba,    88           
88      "8b               ""  88           88             88               ""                  d8"     "8b   d8"'    `"8b   88           
88      ,8P                   88           88             88                                   Y8,          d8'        `8b  88           
88aaaaaa8P'  88       88  88  88   ,adPPYb,88             88   ,adPPYba,   88  8b,dPPYba,      `Y8aaaaa,    88          88  88           
88""""""8b,  88       88  88  88  a8"    `Y88             88  a8"     "8a  88  88P'   `"8a       `"""""8b,  88          88  88           
88      `8b  88       88  88  88  8b       88             88  8b       d8  88  88       88             `8b  Y8,    "88,,8P  88           
88      a8P  "8a,   ,a88  88  88  "8a,   ,d88     88,   ,d88  "8a,   ,a8"  88  88       88     Y8a     a8P   Y8a.    Y88P   88           
88888888P"    `"YbbdP'Y8  88  88   `"8bbdP"Y8      "Y8888P"    `"YbbdP"'   88  88       88      "Y88888P"     `"Y8888Y"Y8a  88888888888  
*/

function buildJoin(tblid, startingStr, reverseJoin) {
    var joinsql = "";
    if (!startingStr) startingStr = "";
    joinsql += startingStr + " ";
    tblList.remove(tblid);
    var conns = instance.getConnections({ target: tblid });
    if (conns.length == 0 && reverseJoin) joinsql = ",\n" + getTableStr(tblid);
    for (var i = 0; i < conns.length; i++) {
        var c = conns[i];
        var isSourceThere = $.inArray(c.sourceId, tblList) > -1
        if (isSourceThere || reverseJoin) {
            if (isSourceThere) tblList.remove(c.sourceId);
            if (c.getOverlay("joinType")) {
                var joinType = c.getOverlay("joinType").labelText;
                var tblName = getTableStr(c.sourceId);
                var joinCols = c.getOverlay("label2").labelText;
                if (!isSourceThere) {
                    joinType = joinType.replace("left ", "right ");
                    tblName = getTableStr(c.targetId);
                }
                joinsql += " \n " + joinType + " " + tblName + " on " + joinCols.replace("XX", "_") + " ";
                joinsql += buildJoin(c.sourceId);
            }
        }
    }
    if (!joinsql) joinsql = "";
    return joinsql;
}
//Ascii Font: Univers  http://patorjk.com/software/taag/#p=display&f=Univers&t=
/*
8b           d8                        88              88           88                         
`8b         d8'                        ""              88           88                         
 `8b       d8'                                         88           88                         
  `8b     d8'  ,adPPYYba,  8b,dPPYba,  88  ,adPPYYba,  88,dPPYba,   88   ,adPPYba,  ,adPPYba,  
   `8b   d8'   ""     `Y8  88P'   "Y8  88  ""     `Y8  88P'    "8a  88  a8P_____88  I8[    ""  
    `8b d8'    ,adPPPPP88  88          88  ,adPPPPP88  88       d8  88  8PP"""""""   `"Y8ba,   
     `888'     88,    ,88  88          88  88,    ,88  88b,   ,a8"  88  "8b,   ,aa  aa    ]8I  
      `8'      `"8bbdP"Y8  88          88  `"8bbdP"Y8  8Y"Ybbd8"'   88   `"Ybbd8"'  `"YbbdP"'  
*/


var schemaJsonCall = "tableData.aspx?schema=Y";
var tableDataJsonCall = "tableData.aspx";
var schemaExtendedJsonCall = "tables.json.js";
var schemaViewsJsonCall = "tableData.aspx?views=Y";
var schemaOtherDBJsonCall = "tableData.aspx?otherDB=Y";
//var tableSchemaJsonCall = "rawData.json";
var tableSchema = {};
var tableSchemaExtended = {};
var tabledata;
var jtree;
var treetypes = {
    "table": { "icon": "fa fa-table" },
    "default": { "icon": "fa fa-columns" },
    "col": { "icon": "fa fa-columns" },
    "date": { "icon": "fa fa-calendar" },
    "string": { "icon": "fa fa-font" },
    "int": { "icon": "fa fa-list-ol" },
    "key": { "icon": "fa fa-key" }
};
var tblList = [];

/*                                                                                                                                                                
88               88           88              88  88                            88888888ba,                           88                               88888888ba                                        
88               ""    ,d     ""              88  ""                            88      `"8b                          ""                               88      "8b                                       
88                     88                     88                                88        `8b                                                          88      ,8P                                       
88  8b,dPPYba,   88  MM88MMM  88  ,adPPYYba,  88  88  888888888   ,adPPYba,     88         88   ,adPPYba,  ,adPPYba,  88   ,adPPYb,d8  8b,dPPYba,      88aaaaaa8P'  ,adPPYYba,  8b,dPPYba,    ,adPPYba,  
88  88P'   `"8a  88    88     88  ""     `Y8  88  88       a8P"  a8P_____88     88         88  a8P_____88  I8[    ""  88  a8"    `Y88  88P'   `"8a     88""""""'    ""     `Y8  88P'   `"8a  a8P_____88  
88  88       88  88    88     88  ,adPPPPP88  88  88    ,d8P'    8PP"""""""     88         8P  8PP"""""""   `"Y8ba,   88  8b       88  88       88     88           ,adPPPPP88  88       88  8PP"""""""  
88  88       88  88    88,    88  88,    ,88  88  88  ,d8"       "8b,   ,aa     88      .a8P   "8b,   ,aa  aa    ]8I  88  "8a,   ,d88  88       88     88           88,    ,88  88       88  "8b,   ,aa  
88  88       88  88    "Y888  88  `"8bbdP"Y8  88  88  888888888   `"Ybbd8"'     88888888Y"'     `"Ybbd8"'  `"YbbdP"'  88   `"YbbdP"Y8  88       88     88           `"8bbdP"Y8  88       88   `"Ybbd8"'  
                                                                                                                           aa,    ,88                                                                    
                                                                                                                            "Y8bbdP"                                               
*/
var instance = null;
function createInstace() {
    if (!instance) {
        instance = jsPlumb.getInstance({
            Endpoint: ["Dot", { radius: 4}],
            HoverPaintStyle: { strokeStyle: "#1e8151", lineWidth: 2 },
            ConnectionOverlays: [
                ["Arrow", {
                    location: 0,
                    id: "arrow",
                    length: 14,
                    foldback: 0.1
                }],
                ["Label", { label: "Join", id: "label", cssClass: "aLabel"}]
            ],
            connector: ["Flowchart", { cornerRadius: 10}],
            anchors: ["Center", "Center"],
            Container: "dropzone"
        });
        instance.bind("click", function (c) {
            instance.detach(c);
            updateSql();
        });
        instance.bind("connection", function (info) {
            if (!info.connection.getOverlay("label2")) {
                var child = $(info.source).jstree().get_json()[0].id;
                var parent = $(info.target).jstree().get_json()[0].id;
                var parentName = $(info.target).jstree().get_json()[0].text;
                var childName = $(info.source).jstree().get_json()[0].text;
                var div = $("#joindlg");
                var s = div.find("#joindlg_sourcedd")
                s.find('option').remove();
                $.each($(info.target).jstree().get_json()[0].children, function (index, val) {
                    $('<option />', { text: val.data.jstree.select.replace(parent+".","") }).appendTo(s);
                });
                div.find("#joindlg_dest").text(child);
                div.find("#joindlg_source").text(parent);
                s = div.find("#joindlg_destdd");
                s.find('option').remove();
                $.each($(info.source).jstree().get_json()[0].children, function (index, val) {
                    $('<option />', { text: val.data.jstree.select.replace(child + ".", "") }).appendTo(s);
                });
                $(div).dialog({
                    resizable: false,
                    modal: true,
                    buttons: {
                        "Join": function () {
                            var joinOverlays = getJoin(parent, $(this).find("select").first().val(), child, $(this).find("select").last().val());
                            for (var i = 0; i < joinOverlays.length; i++)
                                info.connection.addOverlay(joinOverlays[i]);
                            updateSql();
                            $(this).dialog("close");
                        },
                        Cancel: function () {
                            instance.detach(info.connection);
                            $(this).dialog("close");
                        }
                    }
                });

            }
        });
    }
}

//Call after first tree is created, only call once
var dragInitialized = false;
function initializeDesignDrag(){
    if(!dragInitialized)
        $(document)
                .on('dnd_move.vakata', function (e, data) {
                    var t = $(data.event.target);
                    if (!t.closest('.jstree').length) {
                        if (t.closest('.drop').length && $(data.element.parentNode).hasClass('datatable')) {
                            data.helper.find('.jstree-icon').removeClass('jstree-er').addClass('jstree-ok');
                        }
                        else {
                            data.helper.find('.jstree-icon').removeClass('jstree-ok').addClass('jstree-er');
                        }
                    }
                })
                .on('dnd_stop.vakata', function (e, data) {
                    var t = $(data.event.target);
                    if (!t.closest('.jstree').length) {
                        if (t.closest('.drop').length) {
                            if ($(data.element.parentNode).hasClass('datatable')) {
                                createNode($(data.element).text(), data.event.clientX - $('.tableList').width(), data.event.clientY, data);
                                //$(data.element).clone().appendTo(t.closest('.drop'));
                                // node data: 
                                // if(data.data.jstree && data.data.origin) { console.log(data.data.origin.get_node(data.element); }
                            }
                        }
                    }
                });
    dragInitialized =true;
}

function setupData(schema, targetElement) {
		schema = makeUniversalRelations(schema);
		var htmlString = "";
		if (!targetElement)
              targetElement = '#jstree';
        $(targetElement).find(".hideWhenDone").hide();
        $.each(schema, function (key, val) {
			htmlString += createTreeString(key);
        });
        jtree = createTree(htmlString, $(targetElement), ["dnd", "search", "types", "html_data", "state"], true);
        initializeDesignDrag();
}


/*
88                                            88     88888888ba,                                     
88                                            88     88      `"8b                 ,d                 
88                                            88     88        `8b                88                 
88           ,adPPYba,   ,adPPYYba,   ,adPPYb,88     88         88  ,adPPYYba,  MM88MMM  ,adPPYYba,  
88          a8"     "8a  ""     `Y8  a8"    `Y88     88         88  ""     `Y8    88     ""     `Y8  
88          8b       d8  ,adPPPPP88  8b       88     88         8P  ,adPPPPP88    88     ,adPPPPP88  
88          "8a,   ,a8"  88,    ,88  "8a,   ,d88     88      .a8P   88,    ,88    88,    88,    ,88  
88888888888  `"YbbdP"'   `"8bbdP"Y8   `"8bbdP"Y8     88888888Y"'    `"8bbdP"Y8    "Y888  `"8bbdP"Y8 
*/

function loadData(){
	$.getJSON(schemaExtendedJsonCall, {}, function (data) { 
            tableSchemaExtended = data; 
    }).fail(ajaxError);
	$.getJSON(schemaJsonCall, {}, function (data) {
            tableSchema = data;
            var externalTables="";  //Tables in external Schema that aren't in the main database.
            $.each(tableSchemaExtended,function(key,val){ 
                if(!(key in tableSchema)) externalTables+= key + "##"
            });
			tableSchema = $.extendext(true, 'concat', {}, tableSchema, tableSchemaExtended);
			
            //jQuery.extend(true, tableSchema, tableSchemaExtended);
            if(externalTables=="")
                setupData(tableSchema);
            else 
                $.getJSON(schemaJsonCall+"&tablelist="+externalTables, function (data) { 
                    //jQuery.extend(true, tableSchema, data);
                    tableSchema = $.extendext(true, 'concat',{}, tableSchema, data);
                    setupData(tableSchema);   
                }).fail(ajaxError);
    }).fail(ajaxError);
    $("body").trigger("dataloaded");
}

var viewsLoaded = false;
function addViewData()
{
        $.getJSON(schemaViewsJsonCall, function (data) { 
            //jQuery.extend(true, tableSchema, data); 
            tableSchema = $.extendext(true, 'concat', tableSchema, data);
            setupData(data,'#views'); 
        }).fail(ajaxError);
         //$('#jstree').jstree().create_node('#' ,  { "id" : "ajson5", "text" : "newly added" }, "last", function(){});
}

function addOtherDatabases(){
        $.getJSON(schemaOtherDBJsonCall, function (data) { 
            $.each(data.rows, function (key, row) {
                var val=row.name;
                var dbGroup = $('<div class="jstree-default tableGroup"></div>').attr("id","OtherDB_"+val);
                dbGroup.appendTo($("#otherDB"));
                makeToggle(dbGroup,val,function(){
                    $.getJSON(schemaOtherDBJsonCall+"&otherDBname=" +val, function (data2) { 
                        //jQuery.extend(true, tableSchema, data2); 
                        tableSchema = $.extendext(true, 'concat', tableSchema, data2);
                        setupData(data2,'#'+"OtherDB_"+val);
                        //$('#'+"OtherDB_"+val).find(".hideWhenDone").hide();
                    })
                    .fail(function (a,b,c){ ajaxError(a,b,c,$('#'+"OtherDB_"+val));});
                },"fa-database");
                $("#otherDB").find(".hideWhenDone").first().hide();
            });
            //jQuery.extend(true, tableSchema, data); 
            //setupData(data,'#views')
        }).fail(function (a,b,c){ ajaxError(a,b,c,$("#otherDB"));});
}

/*
                                                                                                                       
888888888888                                    88888888ba                88  88           88                          
     88                                         88      "8b               ""  88           88                          
     88                                         88      ,8P                   88           88                          
     88  8b,dPPYba,   ,adPPYba,   ,adPPYba,     88aaaaaa8P'  88       88  88  88   ,adPPYb,88   ,adPPYba,  8b,dPPYba,  
     88  88P'   "Y8  a8P_____88  a8P_____88     88""""""8b,  88       88  88  88  a8"    `Y88  a8P_____88  88P'   "Y8  
     88  88          8PP"""""""  8PP"""""""     88      `8b  88       88  88  88  8b       88  8PP"""""""  88          
     88  88          "8b,   ,aa  "8b,   ,aa     88      a8P  "8a,   ,a88  88  88  "8a,   ,d88  "8b,   ,aa  88          
     88  88           `"Ybbd8"'   `"Ybbd8"'     88888888P"    `"YbbdP'Y8  88  88   `"8bbdP"Y8   `"Ybbd8"'  88          

*/
function createTreeString(tableName, target, plugins, key) {
    var items = [];
    var cols = [];
    if (!key)
        key = getNextSelector(tableName);
    var i = 0;

    var val = tableSchema[tableName];
    if (!val.columns) {
        console.log("There were no columns defined in the table:" + tableName + " fix the file: Tables.json.js");
    } else {
        $.each(val.columns, function (i, col) {
            cols.push(getColumnString(col.type, col.name, key + "XX" + col.name, key + "." + col.name));
            //cols.push( "<li data-jstree='{\"type\":\"" + col.type + "\"}' id='" + key + "XX" + col.name + "'>" + col.name + " (" + col.type + ")</li>" );
        });
    }
    var colString = "";
    if (cols.length > 0)
        colString = "<ul>" + cols.join("") + "</ul>";
    items.push("<li data-jstree='{\"type\":\"table\" }' class='datatable' id='" + key + "'><i class='fa fa-table'></i>" + tableName + colString + "</li>");

    var htmlString = items.join("");
    if (target)
        createTree(htmlString, target, plugins)
    return htmlString;
}

function createTree(htmlString, target, plugins, preventAdds) {
    if (!plugins)
        plugins = ["search","types", "html_data", "checkbox", "contextmenu", "crrm"];
    var core = {};
    if (!preventAdds) core = { 'check_callback': true }
    $("<ul/>", {
        "class": "my-new-list",
        html: htmlString
    }).appendTo(target);
    return target.jstree({
        'core': core,
        "checkbox": { "keep_selected_style": false },
        "types": treetypes,
        contextmenu: { items: customMenu },
        "search": { "show_only_matches": true,"search_method": "jstree_title_contains"},
        "plugins": plugins
    });
}

function getColumnString(type, name, id, select) {
    if (!select)
        select = name;
    return "<li data-jstree='{\"type\":\"" + type + "\",\"select\":\"" + select + "\", \"name\":\"" + name + "\"}' id='" + id + "'>" + name + " (" + type + ")</li>"
}


/*
  ,ad8888ba,                                                              88888888ba,                           88                               888888888888         88           88              
 d8"'    `"8b                                        ,d                   88      `"8b                          ""                                    88              88           88              
d8'                                                  88                   88        `8b                                                               88              88           88              
88             8b,dPPYba,   ,adPPYba,  ,adPPYYba,  MM88MMM  ,adPPYba,     88         88   ,adPPYba,  ,adPPYba,  88   ,adPPYb,d8  8b,dPPYba,           88  ,adPPYYba,  88,dPPYba,   88   ,adPPYba,  
88             88P'   "Y8  a8P_____88  ""     `Y8    88    a8P_____88     88         88  a8P_____88  I8[    ""  88  a8"    `Y88  88P'   `"8a          88  ""     `Y8  88P'    "8a  88  a8P_____88  
Y8,            88          8PP"""""""  ,adPPPPP88    88    8PP"""""""     88         8P  8PP"""""""   `"Y8ba,   88  8b       88  88       88          88  ,adPPPPP88  88       d8  88  8PP"""""""  
 Y8a.    .a8P  88          "8b,   ,aa  88,    ,88    88,   "8b,   ,aa     88      .a8P   "8b,   ,aa  aa    ]8I  88  "8a,   ,d88  88       88          88  88,    ,88  88b,   ,a8"  88  "8b,   ,aa  
  `"Y8888Y"'   88           `"Ybbd8"'  `"8bbdP"Y8    "Y888  `"Ybbd8"'     88888888Y"'     `"Ybbd8"'  `"YbbdP"'  88   `"YbbdP"Y8  88       88          88  `"8bbdP"Y8  8Y"Ybbd8"'   88   `"Ybbd8"'  
                                                                                                                     aa,    ,88                                                                    
                                                                                                                      "Y8bbdP"          
*/
function createNode(tableName, clientX, clientY, data) {
    createInstace()
    //$(data.element).clone().appendTo('.drop');
    var div = $("<div class='tableNode w'>" + tableName + "</div>")
    var tableKey = getNextSelector(tableName.substring(0,2));
    createTreeString(tableName, div, null, tableKey);
    div.appendTo('.drop');
    div.attr("tableName", tableName)
    instance.draggable(div);
    div.resizable({ resize: function (event, ui) { instance.repaintEverything(); } }).css('top', clientY + 'px').css('left', clientX + 'px');
    var i = 0;
    div.attr("id", tableKey)
    div.addClass("tbl_" + tableName);
    div.append($("<div class='ep'>"));

    $("<span style='position:absolute;' class='ui-icon ui-icon-closethick closeButton'></span>").button().click(
            function () {
                instance.detachAllConnections(div);
                div.remove();
            }
        ).appendTo(div)

    instance.makeSource(div, {
        filter: ".ep",
        anchor: "Continuous",
        connector: ["Flowchart", { curviness: 20 }],
        connectorStyle: { strokeStyle: "#5c96bc", lineWidth: 2, outlineColor: "transparent", outlineWidth: 4 },
        maxConnections: 5,
        onMaxConnections: function (info, e) {
            alert("Maximum connections (" + info.maxConnections + ") reached");
        }
    });
    instance.makeTarget(div, {
        dropOptions: { hoverClass: "dragHover" },
        anchor: "Continuous"
    });

    if (tableSchema[tableName].relations) {
        $.each(tableSchema[tableName].relations, function (i, rel) {
            $( "[tablename='" + rel.table + "']" ).each(function () {
            //$(".tbl_" + rel.table).each(function () {
                join(div, rel.parent_cols[0], $(this), rel.child_cols[0]);
            });
        });
    }
    $(".w").each(function () {
        var parentDiv = $(this);
        if (tableSchema[$(this).attr("tablename")].relations)
            $.each(tableSchema[$(this).attr("tablename")].relations, function (i, rel) {
                if (rel.table == tableName) {
                    join(parentDiv, rel.parent_cols[0], div, rel.child_cols[0]);
                }
            })
    })
    updateSql();
    jsPlumb.repaintEverything();
}

function join(source, sourceCol, dest, destCol) {
    instance.connect({
        source: source.attr("id"),
        target: dest.attr("id"),
        overlays: getJoin(dest.attr("id"), sourceCol, source.attr("id"), destCol),
        join: source.attr("id") + "." + sourceCol + "=" + dest.attr("id") + "." + destCol
    });
}

function getJoin(parent, parentCol, child, childCol) {
    return [["Label", { label: parent + "." + parentCol + " = " + child + "." + childCol, id: "label2", cssClass: "aLabel2"}],
        ["Label", { label: "left outer join", id: "joinType", cssClass: "joinLabel"}]];
}

/*                                                                                                                                   
88b           d88                                           88888888ba                88  88           88                          
888b         d888                                           88      "8b               ""  88           88                          
88`8b       d8'88                                           88      ,8P                   88           88                          
88 `8b     d8' 88   ,adPPYba,  8b,dPPYba,   88       88     88aaaaaa8P'  88       88  88  88   ,adPPYb,88   ,adPPYba,  8b,dPPYba,  
88  `8b   d8'  88  a8P_____88  88P'   `"8a  88       88     88""""""8b,  88       88  88  88  a8"    `Y88  a8P_____88  88P'   "Y8  
88   `8b d8'   88  8PP"""""""  88       88  88       88     88      `8b  88       88  88  88  8b       88  8PP"""""""  88          
88    `888'    88  "8b,   ,aa  88       88  "8a,   ,a88     88      a8P  "8a,   ,a88  88  88  "8a,   ,d88  "8b,   ,aa  88          
88     `8'     88   `"Ybbd8"'  88       88   `"YbbdP'Y8     88888888P"    `"YbbdP'Y8  88  88   `"8bbdP"Y8   `"Ybbd8"'  88                                                                                                                                   
*/
function customMenu(node) {
    var coltitle = node.id.substring(node.id.lastIndexOf("XX") + 2);
    var tableName = node.parent;
    var colname = tableName + "." + coltitle;
    // The default set of all items
    var items = {};

    if (node.type == "date") {
        items["Year"] = {
            label: "Year", action: function () { createCol(node, coltitle + "_year", "int", "year(" + colname + ")") }
        };
        items["Month"] = {
            label: "Month", action: function () { createCol(node, coltitle + "_month", "int", "month(" + colname + ")") }
        };
        items["Day"] = {
            label: "Day", action: function () { createCol(node, coltitle + "_day", "int", "day(" + colname + ")") }
        };
        items["Year/Month"] = {
            label: "Year/Month", action: function () {
                createCol(node, coltitle + "_ym", "string",
                    "cast(year(" + colname + ") as varchar)+'/'+cast(month(" + colname + ") as varchar)")
            }
        };
        items["Year/Month/Day"] = {
            label: "Year/Month/Day", action: function () {
                createCol(node, coltitle + "_ymd", "string",
                    "cast(year(" + colname + ") as varchar)+'/'+cast(month(" + colname + ") as varchar)+'/'+cast(day(" + colname + ") as varchar)")
            }
        };
    } else if (node.type == "string") {
        items["Concat"] = {
            label: "Concat", action: function () { createCol(node, colname, "int") }
        };
    }
    return items;
}

function createCol(node, name, type, select) {
    var key = getNextCol(node.id);
    var position = $("#" + node.id).index() + 1;
    if (!select) select = name;
    var newNode = { state: "open", type: type, data: { "type": type, "select": select,"name":name }, id: key, text: name + " (" + type + ")" };
    var parentNode = $("#" + node.parent);
    if (!parentNode.hasClass("tableNode"))
        parentNode = parentNode.parents(".tableNode").first();
    parentNode.jstree().create_node("#" + node.parent, newNode, position);
}

function getNextCol(selector) {
    var i = 0;
    selector = selector.toLowerCase();
    var ret = selector;
    while ($("#" + ret).length) {
        ret = selector + "_" + i;
        i++;
    }
    return ret;
}


/*
88        88              88                                                  
88        88              88                                                  
88        88              88                                                  
88aaaaaaaa88   ,adPPYba,  88  8b,dPPYba,    ,adPPYba,  8b,dPPYba,  ,adPPYba,  
88""""""""88  a8P_____88  88  88P'    "8a  a8P_____88  88P'   "Y8  I8[    ""  
88        88  8PP"""""""  88  88       d8  8PP"""""""  88           `"Y8ba,   
88        88  "8b,   ,aa  88  88b,   ,a8"  "8b,   ,aa  88          aa    ]8I  
88        88   `"Ybbd8"'  88  88`YbbdP"'    `"Ybbd8"'  88          `"YbbdP"'  
                              88                                              
                              88          
*/

var SQLKEYWORDS = [ "LI", "ADD", "EXTERNAL", "PROCEDURE", "ALL", "FETCH", "PUBLIC", "ALTER", "FILE", "RAISERROR", "AND", "FILLFACTOR", "READ", "ANY", "FOR", "READTEXT", "AS", "FOREIGN", "RECONFIGURE", "ASC", "FREETEXT", "REFERENCES", "AUTHORIZATION", "FREETEXTTABLE", "REPLICATION", "BACKUP", "FROM", "RESTORE", "BEGIN", "FULL", "RESTRICT", "BETWEEN", "FUNCTION", "RETURN", "BREAK", "GOTO", "REVERT", "BROWSE", "GRANT", "REVOKE", "BULK", "GROUP", "RIGHT", "BY", "HAVING", "ROLLBACK", "CASCADE", "HOLDLOCK", "ROWCOUNT", "CASE", "IDENTITY", "ROWGUIDCOL", "CHECK", "IDENTITY_INSERT", "RULE", "CHECKPOINT", "IDENTITYCOL", "SAVE", "CLOSE", "IF", "SCHEMA", "CLUSTERED", "IN", "SECURITYAUDIT", "COALESCE", "INDEX", "SELECT", "COLLATE", "INNER", "SEMANTICKEYPHRASETABLE", "COLUMN", "INSERT", "SEMANTICSIMILARITYDETAILSTABLE", "COMMIT", "INTERSECT", "SEMANTICSIMILARITYTABLE", "COMPUTE", "INTO", "SESSION_USER", "CONSTRAINT", "IS", "SET", "CONTAINS", "JOIN", "SETUSER", "CONTAINSTABLE", "KEY", "SHUTDOWN", "CONTINUE", "KILL", "SOME", "CONVERT", "LEFT", "STATISTICS", "CREATE", "LIKE", "SYSTEM_USER", "CROSS", "LINENO", "TABLE", "CURRENT", "LOAD", "TABLESAMPLE", "CURRENT_DATE", "MERGE", "TEXTSIZE", "CURRENT_TIME", "NATIONAL", "THEN", "CURRENT_TIMESTAMP", "NOCHECK", "TO", "CURRENT_USER", "NONCLUSTERED", "TOP", "CURSOR", "NOT", "TRAN", "DATABASE", "NULL", "TRANSACTION", "DBCC", "NULLIF", "TRIGGER", "DEALLOCATE", "OF", "TRUNCATE", "DECLARE", "OFF", "TRY_CONVERT", "DEFAULT", "OFFSETS", "TSEQUAL", "DELETE", "ON", "UNION", "DENY", "OPEN", "UNIQUE", "DESC", "OPENDATASOURCE", "UNPIVOT", "DISK", "OPENQUERY", "UPDATE", "DISTINCT", "OPENROWSET", "UPDATETEXT", "DISTRIBUTED", "OPENXML", "USE", "DOUBLE", "OPTION", "USER", "DROP", "OR", "VALUES", "DUMP", "ORDER", "VARYING", "ELSE", "OUTER", "VIEW", "END", "OVER", "WAITFOR", "ERRLVL", "PERCENT", "WHEN", "ESCAPE", "PIVOT", "WHERE", "EXCEPT", "PLAN", "WHILE", "EXEC", "PRECISION", "WITH", "EXECUTE", "PRIMARY", "WITHIN GROUP", "EXISTS", "PRINT", "WRITETEXT", "EXIT", "PROC"]
function isKeyword(key) {
    return $.inArray(key.toUpperCase(), SQLKEYWORDS) > -1;
}
function getNextSelector(selector) {
    var i = 0;
    if (isKeyword(selector)) selector += "0";
    selector = selector.toLowerCase();
    selector = replaceAll(selector,".","_");
    selector = replaceAll(selector,"[","");
    selector = replaceAll(selector,"]","");
    var ret = selector;
    if(selector)
    while ($("#" + ret).length) {
        ret = selector + "" + i;
        i++;
    }
    return ret;
}

function replaceAll(str, find, replace) {
  return str.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}

function escapeRegExp(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

Array.prototype.remove = function (v) { this.splice(this.indexOf(v) == -1 ? this.length : this.indexOf(v), 1); }
function htmlEscape(str) {
    return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
}

function ajaxError(jqxhr, textStatus, error,waitElem) {
    var err = textStatus + ", " + error;
    console.log("Request Failed: " + err);
    if($(waitElem).find(".hideWhenDone").length >0){
        waitElem = $(waitElem).find(".hideWhenDone");
        var errorSpan = $("<span style='color:firebrick;'>").text($($(jqxhr.responseText)[1]).text());
        $(waitElem).after(errorSpan);
         $(waitElem).hide();
    }
}

//Allows a universal relation so for example all columns called "customerID" will join to the customer table on the parent column ID
function makeUniversalRelations(schema) {
	if (!schema) schema = tableSchema;

	var containsCols = function (lookingForCols, colArray) {
		for (var i = 0; i < lookingForCols.length; i++) {
			var searchStr = lookingForCols[i].toLowerCase();
			var foundcol = colArray.find(function (column) { return column.name.toLowerCase() == searchStr; });
			if (!foundcol) return false;
		}
		return true;
	}

	$.each(schema, function (parentTableName, parentTable) {							//for each table in the schema
		if (parentTable.universalRelations) {											//if there is a universal relation
			$.each(parentTable.universalRelations, function (key, join) {				//look at all the universal relations..
				$.each(schema, function (childTableName, childTable) {					//and compare those to all the other tables
					if (parentTableName != childTableName && containsCols(join.parent_cols,parentTable.columns )) {					//except if it's the current table, or the parent table dosen't have the listed cols'
						if (containsCols(join.child_cols, childTable.columns)) {								//if the table has all the join columns
							var newRelation = { "table": parentTableName, "child_cols": join.child_cols, "parent_cols": join.parent_cols };
							var findRelation = childTable.relations.find(function (relation) {
								return JSON.stringify(relation) === JSON.stringify(newRelation)
							});
							if (!findRelation)
								childTable.relations.push(newRelation);
						}
					}
				});
			})
		}
	});
	return schema;
}

var cloneCount = 1;
var loadFunctList = [];
function makeToggle(elem,toggleTitle,runFunction,icon){
    elem = $(elem);
    elem.hide();
    var func = function(){};
    if(runFunction){
        if(elem.find(".hideWhenDone").length==0)
            elem.prepend($("#loadSpinner").clone().attr('id', 'id'+ cloneCount++));
        func= function(){
            if(!loadFunctList.includes(runFunction)){
                loadFunctList.push(runFunction);
                runFunction();    
            }
        }
    }
    var toggle = $("#toggleElement").clone()
        .attr('id', 'id'+ cloneCount++)
        .insertBefore(elem);
        //toggle.addClass("jstree-themeicon fa fa-database fa-lg jstree-themeicon-custom")
        //<i class="labelAfter jstree-icon jstree-themeicon fa fa-database fa-lg jstree-themeicon-custom"></i>
    if(icon)
            toggle.find(".labelAfter").addClass("fa fa-lg jstree-themeicon-custom "+ icon)
    toggle.find(".labelAfter").after(toggleTitle);

    toggle.click(function(){
            func();
            $(this).next().slideToggle();
            $(this).find("li").toggleClass("jstree-open").toggleClass("jstree-closed");
    });
}
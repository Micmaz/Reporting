(function($) {

    $.fn.inlineEdit = function(options) {
        // define some options with sensible default values
        // - hoverClass: the css classname for the hover style
        options = $.extend({
            hoverClass: 'hover'
        }, options);

        return $.each(this, function() {
            var self = $(this);
            self.value = self.html();
            self.addClass('contentElement');
            self.bind('click', function() {
                if (self.find('.dynamicItem').length == 0) {
                    self.html('<textarea style="width:100%;" class="dynamicItem">' + self.value + '</textarea>')
                        .find('textarea')
                            .bind('blur', function(event) {
                                self.value = $(this).val();
                                if (self.value == "") self.value = "&nbsp;"
                                self.html(self.value);
                            })
                            .focus();
                }

            })
            // on hover add hoverClass, on rollout remove hoverClass
                .hover(
                    function() {
                        self.addClass(options.hoverClass);
                    },
                    function() {
                        self.removeClass(options.hoverClass);
                    }
                );

        });
    }
})(jQuery);

    //setupDragable('reportTarget',
    //    "item1{z-index: 26; left: 256px; top: 24px; position: absolute; width: 118px; height: 15px;}##item2{height: 200px; width: 300px; z-index: 27; left: 737px; top: 119.2px;}##item3{background-color: rgb(17, 136, 255); z-index: 26; left: 317px; top: 148px; position: absolute; width: 375px; height: 133px;}##newaa{z-index: 26; left: 739px; top: 6px; position: absolute; width: 211px; height: 68px;}##new{z-index: 26; left: 98px; top: 187px; position: absolute; width: 166px; height: 116px;}##newasd{z-index: 26; left: 457px; top: 78px; position: absolute; width: 109px; height: 13px;}##newa2{z-index: 26; left: 157px; top: 374px; position: absolute; width: 449px; height: 22px;}##nee32w{z-index: 26; left: 595px; top: 31px; position: absolute; width: 107px; height: 13px;}##newd4{z-index: 26; left: 85px; top: 83px; position: absolute; width: 147px; height: 61px;}##newg4{z-index: 26; left: 424px; top: 29px; position: absolute; width: 108px; height: 15px;}##neh5w{z-index: 26; left: 259px; top: 79px; position: absolute; width: 118px; height: 16px;}",
    //    "aAA####Item 3 here! Blue!##1adsasd##2adsasd##3adsasd##4adsasd##5adsasd##6dsasd##7adsasd##8adsasd##9adasasd");


var elements = [];
function setupDragable(containerID, layoutString, contentsString) {
    $('#' + containerID).css('position', 'relative');
    var DragableElements = layoutString.split("##");
    var DragableContents = contentsString.split("##");
    jQuery.each(DragableElements, function(i) {

        var item = DragableElements[i];
        var contents = "";
        var extracss = "";
        if (DragableContents.length > i) { contents = DragableContents[i]; }

        if (item.indexOf("{") > -1) {
            extracss = item.substring(item.indexOf("{") + 1).replace("}", "");
            item = item.substring(0, item.indexOf("{"));
        }
        addItem(item, contents, extracss, containerID, true);
    });
    $('.dragable').draggable({ containment: "parent", stack: ".dragable" }).resizable();

}

var DragEditOn = true;
var lastContainerID = ''
function addItem(item, contents, extracss, containerID, init) {
    if (!init) init = false;
    if (!containerID) containerID = lastContainerID;
    lastContainerID = containerID;
    var newitem = $(document.createElement('div'));
    if (contents == null || contents == "")
        contents = "&nbsp;";
    var contentElement = $(document.createElement('div')).html(contents);
    newitem.attr('class', 'contentElement');
    newitem.append(contentElement);
    newitem.attr('class', 'positionable');
    newitem.attr('id', item);
    newitem.attr('style', extracss);
    if (DragEditOn) {
        newitem.append($('<span class="ui-state-default ui-corner-all ui-icon ui-icon-closethick" style="background-color: gainsboro;cursor: pointer;position: absolute;right: -9px;top: -9px;">X</span>').click(function() {
            deleteItem($(this).parent());
        }));
        newitem.attr('class', 'positionable dragable ui-corner-all');
        contentElement.inlineEdit({
            save: function(e, data) {
                return false;
            }
        });
    }
    $('#' + containerID).prepend(newitem);
    elements.push(newitem);
    if (!init)
        $('.dragable').draggable({ containment: "parent", stack: ".dragable" }).resizable();

}

function getLayoutString() {
    var ret = "";
    jQuery.each(elements, function(i) {
        var itm = elements[i];
        ret += '##' + itm.attr('id') + '{' + itm.attr('style') + '}';
    });
    if (ret.length == 0) ret = '##';
    return ret.substring(2);
}

function getContentsString() {
    var ret = "";
    jQuery.each(elements, function(i) {
        var itm = $(elements[i]).find('.contentElement');
        ret += '##' + itm.html();
    });
    if (ret.length == 0) ret = '##';
    return ret.substring(2);
}

function deleteItem(itm) {
    var removeitm = -1;
    jQuery.each(elements, function(i) {
        if (itm.attr('id') == elements[i].attr('id')) {
            removeitm = i;
        }
    });
    if (removeitm >= 0) {
        itm.remove();
        elements.splice(removeitm, 1);
    }
}

function getCssString(itm, cssName) {
    if ($(itm).css(cssName) + "" != "") {
        return cssName + ":" + $(itm).css(cssName) + ";";
    }
    return "";
}    
var _tree;

function logEvent(event, data, msg) {
    //        var args = $.isArray(args) ? args.join(", ") :
    msg = msg ? ": " + msg : "";
    $.ui.fancytree.info("Event('" + event.type + "', node=" + data.node + ")" + msg);
}

var option1 = {
    keyPathSeparator: "|", // Used by node.getKeyPath() and tree.loadKeyPath()
    debugLevel: 0, // 0:quiet, 1:errors, 2:warnings, 3:infos, 4:debug
    focusOnSelect: true, // Set focus when node is checked by a mouse click
    escapeTitles: false, // Escape `node.title` content for display

    //autoActivate: true, // Automatically activate a node when it is focused using keyboard
    autoCollapse: true, // Automatically collapse all siblings, when a node is expanded
    //autoScroll: false, // Automatically scroll nodes into visible area
    clickFolderMode: 3, // 1:activate, 2:expand, 3:activate and expand, 4:activate (dblclick expands)
    selectMode: 1, // 1:single, 2:multi, 3:multi-hier

    //icon: true, // Display node icons
    icon: function (event, data) {
        return !data.node.isTopLevel();
    },

    checkbox: false, // Show checkboxes

    //keyboard: true, // Support keyboard navigation
    //keydown: function (event, data) {
    //    switch ($.ui.fancytree.eventToString(data.originalEvent)) {
    //        case "return":
    //        case "space":
    //            data.node.toggleExpanded();
    //            break;
    //    }
    //}

    source: { url: "json/tree.json" },

    //activate: function (event, data) {
    //    var node = data.node;
    //    console.log("activate: node=", node);
    //    //console.log("activate: event=", event, ", data=", data); 
    //    //if (!$.isEmptyObject(node.data)) {
    //    //    console.log("custom node data: " + JSON.stringify(node.data));
    //    //}
    //},

    ///////////////////////////////////////////////////////////////
    // --- Tree events -------------------------------------------------
    //blurTree: function (event, data) {
    //    logEvent(event, data);
    //},
    //create: function (event, data) {
    //    logEvent(event, data);
    //},
    init: function (event, data, flag) { tree_onReady(event, data, flag); },
    //focusTree: function (event, data) {
    //    logEvent(event, data);
    //},
    //restore: function (event, data) {
    //    logEvent(event, data);
    //},
    // --- Node events -------------------------------------------------
    lazyLoad: tree_lazyLoad,
    postProcess: tree_postProcess,
    activate: tree_nodeActive,
    //beforeActivate: function (event, data) {
    //    logEvent(event, data, "current state=" + data.node.isActive());
    //    // return false to prevent default behavior (i.e. activation)
    //    //              return false;
    //},
    //beforeExpand: function (event, data) {
    //    logEvent(event, data, "current state=" + data.node.isExpanded());
    //    // return false to prevent default behavior (i.e. expanding or collapsing)
    //    //				return false;
    //},
    //beforeSelect: function (event, data) {
    //    //				console.log("select", event.originalEvent);
    //    logEvent(event, data, "current state=" + data.node.isSelected());
    //    // return false to prevent default behavior (i.e. selecting or deselecting)
    //    //				if( data.node.isFolder() ){
    //    //					return false;
    //    //				}
    //},
    //blur: function (event, data) {
    //    logEvent(event, data);
    //    $("#echoFocused").text("-");
    //},
    //click: function (event, data) {
    //    logEvent(event, data, ", targetType=" + data.targetType);
    //    // return false to prevent default behavior (i.e. activation, ...)
    //    //return false;
    //},
    //collapse: function (event, data) {
    //    logEvent(event, data);
    //},
    //createNode: function (event, data) {
    //    // Optionally tweak data.node.span or bind handlers here
    //    logEvent(event, data);
    //},
    //dblclick: function (event, data) {
    //    logEvent(event, data);
    //    //				data.node.toggleSelect();
    //},
    //deactivate: function (event, data) {
    //    logEvent(event, data);
    //    //$("#echoActive").text("-");
    //},
    //expand: function (event, data) {
    //    logEvent(event, data);
    //},
    //enhanceTitle: function (event, data) {
    //    logEvent(event, data);
    //},
    //focus: function (event, data) {
    //    logEvent(event, data);
    //    //$("#echoFocused").text(data.node.title);
    //},
    //keydown: function (event, data) {
    //    logEvent(event, data);
    //    switch (event.which) {
    //        case 32: // [space]
    //            data.node.toggleSelected();
    //            return false;
    //    }
    //},
    //keypress: function (event, data) {
    //    // currently unused
    //    logEvent(event, data);
    //},
    //loadChildren: function (event, data) {
    //    logEvent(event, data);
    //},
    //loadError: function (event, data) {
    //    logEvent(event, data);
    //},
    //modifyChild: function (event, data) {
    //    logEvent(event, data, "operation=" + data.operation +
    //        ", child=" + data.childNode);
    //},
    //renderNode: function (event, data) {
    //    // Optionally tweak data.node.span
    //    //              $(data.node.span).text(">>" + data.node.title);
    //    logEvent(event, data);
    //},
    //renderTitle: function (event, data) {
    //    // NOTE: may be removed!
    //    // When defined, must return a HTML string for the node title
    //    logEvent(event, data);
    //    //				return "new title";
    //},
    //select: function (event, data) {
    //    logEvent(event, data, "current state=" + data.node.isSelected());
    //    //var s = data.tree.getSelectedNodes().join(", ");
    //    //$("#echoSelected").text(s);
    //}
};

$(document).ready(function () {
    setTimeout(function () { _tree = $("#ui-tree").fancytree(option1); }, 100);
});

function tree_lazyLoad(event, data) {
    //logEvent(event, data);

    var url = '';
    console.log('LAZY_LOAD = ', data.node.key);

    if (data && data.node) {
        var node = data.node;
        switch (node.key) {
            case 'history':
                url = 'json/' + node.key + '.json';
                break;
            case 'playlist':
                url = 'json/' + node.key + '.json';
                break;
            case 'book':
                url = 'json/' + node.key + '.json';
                break;
            case 'youtube':
                url = 'json/' + node.key + '.json';
                break;
            case 'googledrive':
                url = 'json/' + node.key + '.json';
                break;
            case 'pc_office':
                url = 'json/' + node.key + '.json';
                break;
            case 'pc_home':
                url = 'json/' + node.key + '.json';
                break;
            case 'test':
                url = 'json/test-subs.json';
                break;
            default:
                url = 'json/ajax-sub2.json';
                break;
        }
    }

    data.result = { url: url };
}

function tree_postProcess(event, data) {
    if (data == null) return;

    //logEvent(event, data);
    //// either modify the Ajax response directly
    //data.response[0].title += " - hello from postProcess";
    //// or setup and return a new response object
    ////				data.result = [{title: "set by postProcess"}];

    var node = data.node;
    console.log('POST_PROCESS = ', data.node.key);

    if (node.key == 'root_1') {
        // level root 
        data.response.forEach(function (o) { o.level = 0; });;
    } else {
        // level subs
        data.response.forEach(function (o) {
            o.level = node.data.level + 1;
        });
    }

    //data.node.info(response);
    //switch (response.class) {
    //    case "gov.usgs.itis.itis_service.metadata.SvcKingdomNameList":
    //        data.result = $.map(response.kingdomNames, function (o) {
    //            return o && { title: o.kingdomName, key: o.tsn, folder: true, lazy: true };
    //        });
    //        break;
    //    case "gov.usgs.itis.itis_service.data.SvcHierarchyRecordList":
    //        data.result = $.map(response.hierarchyList, function (o) {
    //            return o && { title: o.taxonName, key: o.tsn, folder: true, lazy: true };
    //        });
    //        break;
    //    default:
    //        $.error("Unsupported class: " + response.class);
    //}
}

function tree_onReady(event, data, flag) {
    console.log('TREE.READY');
    //logEvent(event, data, "flag=" + flag);

    // select the items in lazy nodes (all of our topics are lazy nodes)
    var lazy_paths = ["|test|test11"];
    //alert(lazy_paths);
    data.tree.loadKeyPath(lazy_paths, function (node, status) {
        //if (status === "ok") {
        //    node.setSelected(true);
        //    if (lazy_paths.length == 1) {
        //        node.setActive(true);
        //    }
        //}
        switch (status) {
            case "loaded":
                node.makeVisible();
                break;
            case "ok":
                node.setActive();
                break;
        }
    });
}

function tree_nodeActive(event, data) {
    var json = data.node.toDict(true, function (dict) {
        // Remove keys, so they will be re-generated when this dict is passed to addChildren()
        delete dict.key;
    });
    console.log('NODE_ACITVE node = ', json);
    var path = data.node.getKeyPath();
    console.log('NODE_ACITVE path = ', path);

    //logEvent(event, data);
    //var node = data.node;
    //// acces node attributes
    //$("#echoActive").text(node.title);
    //if (!$.isEmptyObject(node.data)) {
    //    //					alert("custom node data: " + JSON.stringify(node.data));
    //}
}

//// For our demo: toggle auto-collapse mode:
//$("input[name=autoCollapse]").on("change", function (e) {
//    $.ui.fancytree.getTree().options.autoCollapse = $(this).is(":checked");
//});

// Render hidden <input> elements for active and selected nodes
//_tree.generateFormElements(); 


$(function () {
});

$(document).ready(function () {
    setTimeout(function () {
        // Attach the fancytree widget to an existing <div id="tree"> element
        // and pass the tree options as an argument to the fancytree() function:
        $("#ui-tree").fancytree({
            autoCollapse: true,
            clickFolderMode: 3,
            icon: function (event, data) {
                return !data.node.isTopLevel();
            },
            source: { url: "json/tree.json" },
            lazyLoad: function (event, data) {
                data.result = { url: "json/ajax-sub2.json" };
            },
            keydown: function (event, data) {
                switch ($.ui.fancytree.eventToString(data.originalEvent)) {
                    case "return":
                    case "space":
                        data.node.toggleExpanded();
                        break;
                }
            }
        });
        // For our demo: toggle auto-collapse mode:
        $("input[name=autoCollapse]").on("change", function (e) {
            $.ui.fancytree.getTree().options.autoCollapse = $(this).is(":checked");
        });
    }, 100);
});
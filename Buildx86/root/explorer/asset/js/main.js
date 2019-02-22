var _list;

$(function () { 
    var top_ =
        '<div id="title">Learn English</div>' +
        '<div class="playbackCtrl">' +
        '   <div class="playback unavailable backward"><i class="icon-backward"></i></div>' +
        '   <div class="playback play"><i class="icon-play"></i></div>' +
        '   <div class="playback unavailable forward"><i class="icon-forward"></i></div>' +
        '</div>' +

        '<div class="volume">' +
        '   <i class="icon-volume-down"></i>' +
        '   <i class="icon-volume-up"></i>' +
        '   <div class="vol" style="width:50%;"></div>' +
        '   <input type="range" min="1" max="100" value="50" class="slider" id="ui-volume">' +
        '</div>' +

        '<input type="range" min="1" max="100" value="50" class="slider" id="ui-timeline">' +
        '<div id="ui-timeline-val" style="width:50%;"></div>' +
        '<div id="ui-timeline-line"></div>' +

        '<div class="searchBox">' +
        '   <i class="icon-search"></i>' +
        '   <input type="text" name="keywords" id="keywords" placeholder="Search Playlist">' +
        '</div>';

    var status_ =
        '<div id="status">' +
        '   <div class="ctrlIcons">' +
        '       <i class="icon-plus"></i>' +
        '       <i class="icon-random"></i>' +
        '       <i class="icon-refresh"></i>' +
        '       <i class="icon-upload"></i>' +
        '   </div>' +
        '   <div class="listInfo">10 songs, 1.1 hours, 154.5 MB</div>' +
        '   <div class="ctrlIcons">' +
        '       <i class="icon-sign-in"></i>' +
        '   </div>' +
        '</div>';

    $('#ui-app').w2layout({
        name: 'layout',
        padding: 0,
        panels: [
            { type: 'top', size: 78, resizable: false, content: '<div id="ui-top">' + top_ + '</div>', overflow: 'hidden', style: 'border:none;padding:0;' },
            { type: 'left', size: 200, resizable: true, content: '<div id="ui-side"><div id="ui-tree"></div></div>', overflow: 'hidden', style: 'border:none;padding:0;' },
            { type: 'main', content: '<div id="ui-list"></div>', style: 'border:none;padding:0;' },
            { type: 'preview', size: 150, resizable: true, hidden: true, content: '<div id="ui-info"></div>', style: 'border:none;padding:0;' },
            { type: 'right', size: 200, resizable: true, hidden: true, content: '<div id="ui-property"></div>', style: 'border:none;padding:0;' },
            { type: 'bottom', size: 24, resizable: false, hidden: false, content: '<div id="ui-status">' + status_ + '</div>', overflow: 'hidden', style: 'border:none;padding:0;' }
        ]
    });
});

$(document).ready(function () {
    $('#ui-list').w2grid({
        name: 'list',
        style: 'border:none;padding:0;border-left:1px solid rgb(165,165,165);',
        method: 'GET', // need this to avoid 412 error on Safari
        show: { lineNumbers: true },
        columns: [ // Name Time Artist Album Genre Rating Plays
            { field: 'fname', caption: 'Name', size: '250px', frozen: true, sortable: true },
            { field: 'lname', caption: 'Time', size: '150px', sortable: true },
            { field: 'email', caption: 'Artist', size: '200px', sortable: true },
            { field: 'sdate', caption: 'Album', size: '200px', sortable: true },
            { field: 'sdate', caption: 'Genre', size: '200px', sortable: true },
            { field: 'sdate', caption: 'Rating', size: '200px', sortable: true },
            { field: 'sdate', caption: 'Plays', size: '200px', sortable: true },
        ]
    });
    w2ui['list'].load('json/list.json');

});

var contractors_editor = {
    __proto__: tc_class,
    onStart: function (sender) {
        $(sender.id('contractor_inn')).textbox({
            icons: [{
                iconCls: 'icon-search',
                handler: function (e) {
                    let iid = sender.id('contractor_full_name')
                    $(iid).textbox('setValue', 'aaaaa');
                }
            }]
        });
    },
}


var fi_contractors = {
    __proto__: finder,
    editor_class: contractors_editor
};
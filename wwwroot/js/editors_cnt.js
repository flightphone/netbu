var contractors_editor = {
    __proto__: tc_class,
    onStart: function (sender) {
        $(sender.id('contractor_inn')).textbox({
            icons: [{
                iconCls: 'icon-search',
                handler: function (e) {
                    let inn = sender.record['contractor_inn']
                    if (!inn)
                        return;
                    inn = '/usmart/getcntinfo/' + inn.split('/')[0];
                    $.get(inn, function (data) {
                        if (data.suggestions.length == 0)
                            return;
                        let info = data.suggestions[0].data;
                        let iid = sender.id('contractor_full_name');
                        $(iid).textbox('setValue', info.name.short_with_opf);
                        iid = sender.id('contractor_alias');
                        $(iid).textbox('setValue', info.name.short);
                        iid = sender.id('contractor_post_address');
                        $(iid).textbox('setValue', info.address.data.postal_code + ', ' + info.address.value);
                    });



                }
            }]
        });
    },
}


var fi_contractors = {
    __proto__: finder,
    editor_class: contractors_editor
};
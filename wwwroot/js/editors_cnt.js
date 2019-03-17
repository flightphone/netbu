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



var FlightCardsList = {
    __proto__: finder,
    init: function () {
        let now = new Date();
        this.SQLParams = {};
        this.SQLParams.FC_Date = now.getFullYear().toString() + '-' + (now.getMonth()+1).toString() + '-' + now.getDate().toString();
        this.startdate = 0;
    },
    addInitGrid: function (sender) {

        var toolbar = $('<div style="padding:2px 4px" align="right"></div>').appendTo('body');
        $('<span> <b>Дата рейса:</b> </span>').appendTo(toolbar);
        sender.fc_date = $('<input>').appendTo(toolbar);
        sender.fc_date.width(100);



        sender.fc_date.datebox({
            onChange: function (newValue, oldValue) {
                if (sender.startdate == 1) {
                    sender.SQLParams.FC_Date = app.dateparser(newValue);
                    sender.UpdateTab(sender);
                }
                else
                    sender.startdate = 1;
            }
        });

        let now = new Date();
        let setval = now.getDate().toString() + '.' + (now.getMonth()+1).toString() + "." + now.getFullYear().toString();
        sender.fc_date.datebox('setValue', setval);


        $(sender.id('maintab')).datagrid({
            toolbar: toolbar
        });

    }
};
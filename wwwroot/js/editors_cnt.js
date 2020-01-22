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
    editor_class: contractors_editor,


    addTool:function(sender, toolbar)
    {

        sender.combut = $('<a>').appendTo(toolbar);
        sender.combut.linkbutton({
            iconCls: 'tree-file',
            text: 'Замечания',
            plain: true,
            onClick: function () 
            {
                //sender.editor.deleterecord(sender.editor);
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                var node_text = 'Замечания(контрагенты) ' +  row['contractor_id'].toString();
                var node_id = 'contractor_id_' + row['contractor_id'].toString();
                var link1 = '/Docfiles/comments?ag_id=' + row['contractor_id'].toString() + '&ag_type=cnt';
                app.showLink(node_text, node_id, link1);
                
            }
        });

    }


};

var fi_agreements = {
    __proto__: finder,
    addTool:function(sender, toolbar)
    {
        sender.lbut = $('<a>').appendTo(toolbar);
        sender.lbut.linkbutton({
            iconCls: 'tree-file',
            text: 'Файлы',
            plain: true,
            onClick: function () 
            {
                //sender.editor.deleterecord(sender.editor);
                //console.debug(JSON.stringify(sender.editor.save2json(sender.editor)));
                //console.debug(sender.editor.save2json(sender.editor));
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                var node_text = 'Файлы ' +  row['agr_key'].toString();
                var node_id = 'agr_' + row['agr_key'].toString();
                var link1 = '/Docfiles/dir?id=' + row['agr_key'].toString() + '/';
                app.showLink(node_text, node_id, link1);
                
            }
        });


        
        sender.combut = $('<a>').appendTo(toolbar);
        sender.combut.linkbutton({
            iconCls: 'tree-file',
            text: 'Замечания',
            plain: true,
            onClick: function () 
            {
                //sender.editor.deleterecord(sender.editor);
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                var node_text = 'Замечания ' +  row['agr_key'].toString();
                var node_id = 'agr_' + row['agr_key'].toString();
                var link1 = '/Docfiles/comments?ag_id=' + row['agr_key'].toString() + '&ag_type=agr';
                app.showLink(node_text, node_id, link1);
            }
        });

    }
}

var DaDataLoads = {
    __proto__: finder,
    
    handleFiles: function (files) {

        if (!files.length)
            return;
        
        var findobj = new FormData();
        findobj.append('img', files[0]);
        sender = this;

        

        $.ajax({
	        type: 'POST',
	        url: '/DaData/upload',
	        processData: false, 
        	contentType: false,
	        data: findobj, 
	        //error : show_error,
	        success: function(msg)   
						{
						
							$("#tariffile").val('');
							if (msg.error)
							{
                                $.messager.alert('Ошибка загруки файла', msg.error, 'error');
							}
							else
							{
								sender.UpdateTab(sender);
							}							
							
						}
	    });

        

    },


    addInitGrid: function (sender) {

        var toolbar = $('<div style="padding:2px 4px"></div>').appendTo('body');
        sender.file = $('<input type="file" id="tariffile" style="display:none" accept="text/txt" onchange="' + sender.prop('handleFiles(this.files)') + '"/>').appendTo(toolbar);
        //Не стирать! Обработка файла!
        sender.fbut = $('<a>').appendTo(toolbar);
        sender.fbut.linkbutton({
            iconCls: 'icon-add',
            text: 'Добавить',
            plain: true,
            onClick: function () {
                sender.file.click();
                //  sender.UpdateTab(sender);
            }
        });
    
    

        $(sender.id('maintab')).datagrid({
            toolbar: toolbar
        });

    }

}

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



var AccessDgs = {
    __proto__: rootObj,
    addAcc: function (sender) {
        var row = sender.form.LeftTab.MainTab.datagrid('getSelected');
        if (row) {
            var freerow = sender.form.FreeTab.MainTab.datagrid('getSelected');
            if (freerow) {
                var sql = "exec p_cnt_subsGroups_Subscribers_EDIT '" + row["grp"] + "', '" + freerow["Account"] + "'";

                $.post('/pg/runsql',
                    {
                        sql: sql//,
                        //account: app.account,
                        //password: app.password
                    },
                    function (data) {
                        sender.form.FreeTab.ReloadTab(sender.form.FreeTab);
                        sender.form.AccessTab.ReloadTab(sender.form.AccessTab);
                    });


            }
        }

    },
    deleteAcc: function (sender) {
        var row = sender.form.LeftTab.MainTab.datagrid('getSelected');
        if (row) {
            var freerow = sender.form.AccessTab.MainTab.datagrid('getSelected');
            if (freerow) {
                var sql = "exec p_cnt_subsGroups_Subscribers_DEL '" + row["grp"] + "', '" + freerow["Account"] + "'";

                $.post('/pg/runsql',
                    {
                        sql: sql//,
                        //account: app.account,
                        //password: app.password
                    },
                    function (data) {
                        sender.form.FreeTab.ReloadTab(sender.form.FreeTab);
                        sender.form.AccessTab.ReloadTab(sender.form.AccessTab);
                    });


            }
        }
    },
    start: function () {
        this.AccessTab = Object.create(finder);
        this.AccessTab.SQLParams = {};
        this.AccessTab.absid = 'AccessTab1010';
        this.AccessTab.IdDeclare = '1458';
        this.AccessTab.form = this;
        this.AccessTab.addInitGrid = function (sender) {
            $(sender.id('maintab')).datagrid({
                onDblClickRow: function (index, row) { sender.form.deleteAcc(sender); }
            });
        };
        this.AccessTab.start();

        this.FreeTab = Object.create(finder);
        this.FreeTab.SQLParams = {};
        this.FreeTab.absid = 'FreeTab1010';
        this.FreeTab.IdDeclare = '1459';
        this.FreeTab.form = this;

        this.FreeTab.addInitGrid = function (sender) {
            $(sender.id('maintab')).datagrid({
                toolbar: [{
                    iconCls: 'icon-up',
                    text: 'Добавить',
                    handler: function () {
                        sender.form.addAcc(sender);
                    }
                }, '-', {
                    iconCls: 'icon-down',
                    text: 'Удалить',
                    handler: function () {
                        sender.form.deleteAcc(sender);
                    }
                }],
                onDblClickRow: function (index, row) { sender.form.addAcc(sender); }
            });
        }

        this.FreeTab.start();

        this.LeftTab = Object.create(finder);
        this.LeftTab.SQLParams = {};
        this.LeftTab.absid = 'LeftTab1010';
        this.LeftTab.IdDeclare = '1457';
        this.LeftTab.form = this;
        this.LeftTab.addInitGrid = function (sender) { };

        this.LeftTab.onSelect = function (index, row, sender) {
            sender.form.AccessTab.SQLParams.grp = row["grp"];
            sender.form.AccessTab.UpdateTab(sender.form.AccessTab);

            sender.form.FreeTab.SQLParams.grp = row["grp"];
            sender.form.FreeTab.UpdateTab(sender.form.FreeTab);

        };



        this.LeftTab.start();


    },
    template: function () {

        var s = $('#access').text();
        return s;
    },
};
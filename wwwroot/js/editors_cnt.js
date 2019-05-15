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

var fi_agreements = {
    __proto__: finder,
    addTool:function(sender, toolbar)
    {
        sender.lbut = $('<a>').appendTo(toolbar);
        sender.lbut.linkbutton({
            iconCls: 'tree-file',
            text: 'Файлы',
            plain: true,
            onClick: function () {
                //sender.editor.deleterecord(sender.editor);
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                var node_text = 'Файлы ' +  row['agr_key'].toString();
                var node_id = 'agr_' + row['agr_key'].toString();
                var tab = $('#tabs').tabs('getTab', node_text);
                if (tab)
                    $('#tabs').tabs('select', node_text);
                else {
                    let form1 = Object.create(simpleHtml);
                    let link1 = '/Docfiles/dir?id=' + row['agr_key'].toString() + '/';
                    //let link1 = 'http://localhost:5000/index.html?exclusiveFolder=/' + row['agr_key'].toString() + '/';
                    form1.page = link1;
                    form1.absid = 'app.forms.form' + node_id.toString();
                    form1.node = node_id.toString();
                    app.forms['form' + node_id.toString()] = form1;
                    var cnt = form1.template();
                    $('#tabs').tabs('add', {
                        title: node_text,
                        content: cnt,
                        closable: true,
                        selected: true,
                        fit: true
                    });
                    form1.start();
                }


            }
        });
    }
};

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
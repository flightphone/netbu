var po_editor = {
    __proto__: tc_class,

    detailIdDec: 0,
    masterField: '',
    detailField: '',

    onEdit: function (sender) {
        sender.detail.SQLParams = {};
        sender.detail.DefaultValues = {};
        sender.detail.SQLParams[sender.detailField] = sender.record[sender.masterField].toString();
        sender.detail.DefaultValues[sender.detailField] = sender.record[sender.masterField].toString();
        sender.detail.UpdateTab(sender.detail);
    },



    htmlContent: function (sender, ed) {

        //Удаляем компоненты
        $(sender.id('winedit')).remove();
        $(sender.id('detail_maintab')).remove();
        for (var i = 0; i < sender.fields.length; i++) {
            $(sender.id(sender.fields[i]['field'])).remove();
        }

        sender.winedit = $('<div id="' + sender.idh('winedit') + '" style="width:1200px;height:550px;padding:10px 10px 0px 10px"></div>').appendTo('body');
        var tb = '<table id="' + sender.idh('detail_maintab') + '"></table>';

        var s = $('#leftright').html();
        s = s.replace('OKFun', sender.prop('okclick'));
        s = s.replace('CloseFun', sender.prop('cancelclick'));
        s = s.replace('leftContent', ed);
        s = s.replace('rigthContent', tb);
        return s;
    },
    onStart: function (sender) {

        sender.detail = Object.create(finder);
        sender.detail.absid = sender.prop('detail');
        sender.detail.editform = sender;
        //sender.detail.IdDeclare = 135;
        sender.detail.IdDeclare = sender.detailIdDec;
        sender.detail.start();

    }
};

var porders = {
    __proto__: finder,
    editor_class: po_editor
};


Poo = function () {
    var res = Object.create(porders);
    res.editor_class.detailIdDec = 135;
    res.editor_class.masterField = 'po_pk';
    res.editor_class.detailField = 'pd_po';
    return res;
}

var tariffs = {
    __proto__: finder,
    handleFiles: function (files) {

        if (!files.length)
            return;
        var row = this.MainTab.datagrid('getSelected');
        var nn = row['nn'].toString();
        var f = new FormData();
        f.append('nn', row['nn'].toString());
        f.append('tfile', files[0]);

        var settings = {
            "async": true,
            "url": "/tffile",
            "method": "POST",
            "processData": false,
            "contentType": false,
            "mimeType": "multipart/form-data",
            "data": f
        }

        $.ajax(settings).done(function (msg) {
            $.messager.alert('Загрузка файла', msg, 'info');
        });

        $("#tariffile").val('');

    },

    addInitGrid: function (sender) {

        var toolbar = $('<div style="padding:2px 4px"></div>').appendTo('body');
        sender.abut = $('<a>').appendTo(toolbar);
        sender.ebut = $('<a>').appendTo(toolbar);
        sender.dbut = $('<a>').appendTo(toolbar);
        //Не стирать! Обработка файла!
        sender.fbut = $('<a>').appendTo(toolbar);
        sender.lbut = $('<a>').appendTo(toolbar);
        sender.file = $('<input type="file" id="tariffile" style="display:none" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" onchange="' + sender.prop('handleFiles(this.files)') + '"/>').appendTo(toolbar);
        //DnLoad
        sender.dlbut = $('<a>').appendTo(toolbar);
        sender.runbut = $('<a>').appendTo(toolbar);
        sender.exbut = $('<a>').appendTo(toolbar);


        //Редакторы
        sender.editor = Object.create(tc_class);
        sender.editor.absid = sender.prop('editor');
        sender.editor.finder = sender;
        sender.editor.start(sender.editor);


        sender.abut.linkbutton({
            iconCls: 'icon-add',
            text: 'Добавить',
            plain: true,
            onClick: function () {
                sender.editor.addrecord(sender.editor);
            }
        });

        sender.ebut.linkbutton({
            iconCls: 'icon-edit',
            text: 'Редактировать',
            plain: true,
            onClick: function () {
                sender.editor.editrecord(sender.editor);
            }
        });

        sender.dbut.linkbutton({
            iconCls: 'icon-cancel',
            text: 'Удалить',
            plain: true,
            onClick: function () {
                sender.editor.deleterecord(sender.editor);
            }
        });


        sender.lbut.linkbutton({
            iconCls: 'tree-file',
            text: 'Строки',
            plain: true,
            onClick: function () {
                //sender.editor.deleterecord(sender.editor);
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                var node_text = row['al_utg'] + ' ' + row['tf_datebeg'].toString().substr(0, 10);
                var node_id = 'tf_' + row['nn'].toString();
                var tab = $('#tabs').tabs('getTab', node_text);
                if (tab)
                    $('#tabs').tabs('select', node_text);
                else {
                    let form1 = Object.create(finder);
                    form1.absid = 'app.forms.form' + node_id.toString();
                    form1.node = node_id.toString();
                    form1.IdDeclare = 134;
                    form1.SQLParams = {};
                    form1.DefaultValues = {};
                    form1.SQLParams['nn'] = row['nn'].toString();
                    form1.DefaultValues['nn'] = row['nn'].toString();


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

        //Не стирать! Обработка файла!

        sender.fbut.linkbutton({
            iconCls: 'icon-magic',
            text: 'Файл',
            plain: true,
            onClick: function () {
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                sender.file.click();
            }
        });

        //Выгрузка тарифа sql
        sender.dlbut.linkbutton({
            iconCls: 'icon-download',
            text: 'Экспорт (sql)',
            plain: true,
            onClick: function () {
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                window.document.location = "/pg/tariffs/" + row["nn"].toString();
            }
        });


        //Выгрузка тарифа uSmart
        sender.runbut.linkbutton({
            iconCls: 'icon-download',
            text: 'Экспорт (uSmart)',
            plain: true,
            onClick: function () {
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }

                $.messager.confirm(
                    {
                        title: 'Экспорт тарифов',
                        msg: 'Перенести данные в справочник uSmart?',
                        fn: function (r) {
                            if (r) {
                                var url = "/pg/runtariffs/" + row["nn"].toString();
                                $.post(url, {},
                                    function (data) {
                                        $.messager.alert('Экспорт тарифов', data, 'info');
                                    });

                            }

                        },

                        ok: "Да",
                        cancel: "Нет"
                    });

            }
        });


        //Выгрузка файла excel
        sender.exbut.linkbutton({
            iconCls: 'icon-download',
            text: 'Экспорт (xlsx)',
            plain: true,
            onClick: function () {
                var row = sender.MainTab.datagrid('getSelected');
                if (!row) {
                    $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                    return;
                }
                window.document.location = "/pg/excel/" + row["nn"].toString();
            }
        });


        $(sender.id('maintab')).datagrid({
            toolbar: toolbar,
            onDblClickRow: function (index, row) {
                sender.editor.editrecord(sender.editor);
            }
        });
    }
}
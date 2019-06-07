$.fn.datebox.defaults.formatter = function (date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return (d < 10 ? '0' + d : d) + '.' + (m < 10 ? '0' + m : m) + '.' + y;
};

$.fn.datebox.defaults.parser = function (s) {
    if (s) {

        var a = s.split('.');
        var d = new Number(a[0]);
        var m = new Number(a[1]);
        var y = new Number(a[2].substring(0, 4));
        var dd = new Date(y, m - 1, d);
        return dd;
    } else {
        return new Date();
    }
};



var app = {
    isP: false,
    account: '',
    password: '',
    startid: '',
    colorParse: function (color) {
        var number = Number(color);
        var b = (number & 0x000000FF);
        var g = (number & 0x0000FF00) >> 8;
        var r = (number & 0x00FF0000) >> 16;
        var decColor = 0x1000000 + b + 0x100 * g + 0x10000 * r;
        return '#' + decColor.toString(16).substr(1);
    },


    dateparser: function (s) {
        if (s) {
            var a = s.split('.');
            var d = a[2].substr(0, 4) + '-' + a[1] + '-' + a[0] + 'T00:00:00.000Z';
            return d;
        } else {
            return null;
        }
    },




    datetimeparser: function (s) {
        if (s) {
            var a = s.split('.');
            var d = a[2].substr(0, 4) + '-' + a[1] + '-' + a[0] + 'T' + a[2].substr(5, 2) + ':' + a[2].substr(8, 2) + ':00.000Z';
            return d;
        } else {
            return null;
        }
    },

    dateformat: function (d, f) {
        if (!d)
            return d;


        if (d.length != 24) {
            let re = new RegExp('0\.(0?)');
            let res = f.match(/0\.(0+)/);

            let n = 0;
            if (res)
                if (res.length > 1) {
                    n = res[1].length;
                }


            if (n > 0)
                return Number(d.toString()).toFixed(n);
            else
                return d;



        }
        f = f.replace('yyyy', d.substr(0, 4));
        f = f.replace('yy', d.substr(2, 2));
        f = f.replace('MM', d.substr(5, 2));
        f = f.replace('dd', d.substr(8, 2));
        f = f.replace('HH', d.substr(11, 2));
        f = f.replace('mm', d.substr(14, 2));
        return f;
    },

    expand: function (e) {
        if ($('#' + e).hasClass('prop-collapsed')) {
            $('#' + e).removeClass('prop-collapsed');
            $('#' + e).addClass('prop-expanded');
            $('#div' + e).css({ display: "block" });
        } else {
            $('#' + e).removeClass('prop-expanded');
            $('#' + e).addClass('prop-collapsed');
            $('#div' + e).css({ display: "none" });
        }
    },

    forms: {},
    treeSelect: function (node) {
        if ($("#tree").tree("isLeaf", node.target)) {
            var tab = $('#tabs').tabs('getTab', node.text);
            if (tab)
                $('#tabs').tabs('select', node.text);
            else {
                
                {
                    var form = {};
                    if (node.attributes.link1.indexOf('/') > -1) {
                        form = Object.create(simpleHtml);
                        form.page = node.attributes.link1;
                    }
                    else
                        if (node.attributes.link1 == "RegulationPrint.Tools.TLDocs") {
                            form = Object.create(TLDocs);
                        }
                        else
                            if (node.attributes.link1 == "tariffs") {

                                form = Object.create(tariffs);

                            } else if (node.attributes.link1 == "RegulationPrint.Tools.TLVins" ||
                                node.attributes.link1 == "RegulationPrint.US.USStoreFinder"
                            ) {
                                form = Object.create(TLVins);

                            }
                            else if (node.attributes.link1 == "Bureau.Access") {
                                form = Object.create(Access);

                            }
                            else if (node.attributes.link1 == "RegulationPrint.Dgs.groupSubs") {
                                form = Object.create(AccessDgs);

                            }
                            else
                                if (node.attributes.params == '1465') {
                                    form = Object.create(fi_declare);
                                }
                                else
                                    if (node.attributes.params == '1444') {
                                        form = Object.create(fi_contractors);
                                    }
                                    else
                                        if (node.attributes.params == '1445') {
                                            form = Object.create(fi_agreements);
                                        }
                                        else
                                            if (node.attributes.params == '1451') {
                                                form = Object.create(FlightCardsList);
                                            }
                                            else
                                                if (node.attributes.params == '1453') {
                                                    form = Object.create(DaDataLoads);
                                                }
                                                else
                                                    if (node.attributes.link1 == "porders") {
                                                        //form = Object.create(porders);
                                                        form = new Poo();
                                                    }
                                                    else {
                                                        if (!node.attributes.params)
                                                            return;
                                                        else
                                                            form = Object.create(finder);
                                                    };

                    form.absid = 'app.forms.form' + node.id.toString();
                    form.node = node.id.toString();
                    form.IdDeclare = node.attributes.params;

                    form.SQLParams = {};
                    form.DefaultValues = {};
                    form.DefaultValues.last_change_user = app.account;
                    form.DefaultValues.audtuser = app.account;

                    app.forms['form' + node.id.toString()] = form;

                    var cnt = form.template();

                    $('#tabs').tabs('add', {
                        title: node.text,
                        content: cnt,
                        closable: true,
                        selected: true,
                        iconCls: node.iconCls,
                        fit: true
                    });

                    form.start();
                }
            }


        }
        else {
            $("#tree").tree("toggle", node.target);
        }
    },
    t_sysfieldmap: [],
    start: function () {
        $('#main').panel({
            fit: true,
            href: 'content.html',
            onLoad: function () {
                //app.init();
            }

        });
    },
    init: function () {

        $('#winlogon').window('close');
        this.account = $('#account').val();
        this.password = $('#password').val();


        $('#tree').tree({
            fit: true,
            url: 'ustore/gettree',
            method: 'post',
            queryParams: {
                account: this.account,
                password: this.password
            },
            onSelect: this.treeSelect,
            onLoadSuccess: function (node, data) {
                $.post('/pg/runsql', {
                    sql: 'select  idmap, decname, dstfield, srcfield, iddeclare, classname, groupdec, keyfield from t_sysfieldmap'//,
                    //account: app.account,
                    //password: app.password
                },
                    function (data) {
                        if (data.message) {
                            $.messager.alert('Ошибка.', data.message, 'error');
                        }
                        else {
                            app.t_sysfieldmap = data.rows;
                            if (app.startid) {
                                var node = $('#tree').tree('find', app.startid);
                                $('#tree').tree('select', node.target);
                            }
                            //console.debug(this.t_sysfieldmap);
                        }
                    });
            }
        });
        $('#tabs').tabs({
            fit: true
        });




    }
};

var Access = {
    __proto__: rootObj,
    addAcc: function (sender) {
        var row = sender.form.LeftTab.MainTab.datagrid('getSelected');
        if (row) {
            var freerow = sender.form.FreeTab.MainTab.datagrid('getSelected');
            if (freerow) {
                //var sql = "select p_sysaccesuser('" + row["objectname"] + "', '" + freerow["account"] + "', 1)";
                var sql = "exec p_sysAccesUser '" + row["ObjectName"] + "', '" + freerow["Account"] + "', 1";
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
                //var sql = "select p_sysaccesuser('" + row["objectname"] + "', '" + freerow["account"] + "', 2)";
                var sql = "exec p_sysAccesUser '" + row["ObjectName"] + "', '" + freerow["ACCOUNT"] + "', 2";

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
        this.AccessTab.IdDeclare = '118';
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
        this.FreeTab.IdDeclare = '119';
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
        this.LeftTab.IdDeclare = '117';
        this.LeftTab.form = this;
        this.LeftTab.addInitGrid = function (sender) { };

        this.LeftTab.onSelect = function (index, row, sender) {
            //sender.form.AccessTab.SQLParams.objectname = row["objectname"];
            sender.form.AccessTab.SQLParams.ObjectName = row["ObjectName"];
            sender.form.AccessTab.UpdateTab(sender.form.AccessTab);

            //sender.form.FreeTab.SQLParams.grp = row["objectname"];
            sender.form.FreeTab.SQLParams.GRP = row["ObjectName"];
            sender.form.FreeTab.UpdateTab(sender.form.FreeTab);

        };



        this.LeftTab.start();


    },
    template: function () {

        var s = $('#access').text();
        return s;
    },
};

var simpleHtml = {
    __proto__: rootObj,
    start: function () { },
    page: '',
    template: function () {
        var s = '<iframe crossorigin src="' + this.page + '" style = "height:100%;width:100%; border-width: 0"/>';
        s = '<div class="easyui-layout" fit="true"><div data-options="region:\'center\', split:false">' + s + '</div></div>';
        return s;
    }
}

var finder = {
    __proto__: rootObj,
    IdDeclare: 0,
    columns: [],
    frozenColumns: [],
    prepareSQL: '',
    //SQLParams: {},
    //DefaultValues: {},
    editor_class: tc_class,
    editor: {},
    pagination: true,
    width: '800',
    height: '550',
    text: 'Поиск',
    singleSelect: true,
    show: function (sender) {
        sender.win.window('open');
    },
    setSQLParams: function (sender) {
        if (sender.sql)
            sender.prepareSQL = sender.sql;
        else
            sender.prepareSQL = sender.t_rpdeclare.decsql;

        for (var par in sender.SQLParams) {
            var val = sender.SQLParams[par];
            if (!val)
                val = '';
            val = val.replace(/'/g, "''");
            var re = new RegExp('@' + par, 'g');
            sender.prepareSQL = sender.prepareSQL.replace(re, "'" + val + "'");


            re = new RegExp(':' + par, 'g');
            sender.prepareSQL = sender.prepareSQL.replace(re, "'" + val + "'");


            re = new RegExp('\\[' + par + '\\]', 'g');
            sender.prepareSQL = sender.prepareSQL.replace(re, val)

        }

    },
    template: function () {
        var s = '<table id="' + this.idh('maintab') + '"></table>';
        s = '<div class="easyui-layout" fit="true"><div data-options="region:\'center\', split:false">' + s + '</div></div>';
        return s;
    },
    createOKhtml: function () {
        $(this.id('win')).remove();
        $(this.id('maintab')).remove();
        this.win = $('<div id="' + this.idh('win') + '" style="width:' + this.width + 'px;height:' + this.height + 'px;padding:10px 10px 0px 10px"></div>').appendTo('body');
        var w = $('#okcancel').html();
        w = w.replace('OKFun', this.prop('okclick()'));
        w = w.replace('CloseFun', this.prop('cancelclick()'));
        var edw = '<table id="' + this.idh('maintab') + '"></table>';
        w = w.replace('Content', edw);
        this.win.window({
            title: this.text,
            modal: true,
            collapsible: false,
            minimizable: false,
            closed: true,
            content: w
        });

    },
    okclicksender: function (sender) {

    },
    cancelclick: function () {
        this.win.window('close');
    },
    okclick: function () {
        this.OKFun(this);
    },
    start: function () {
        if (this.OKFun) {
            this.createOKhtml();
        }

        this.init();
        this.getcolumn();
    },
    UpdateTab: function (sender) {
        sender.setSQLParams(sender);
        //console.debug(sender.prepareSQL);

        sender.MainTab.datagrid('load', {

            sql: sender.prepareSQL,
            SumFields: sender.SumFields,
            LabelField: sender.LabelField,
            LabelText: sender.LabelText,
            pagination: 1//,
            //account: app.account,
            //password: app.password
        });
    },
    ReloadTab: function (sender) {
        sender.setSQLParams(sender);
        sender.MainTab.datagrid('reload', {

            sql: sender.prepareSQL,
            SumFields: sender.SumFields,
            LabelField: sender.LabelField,
            LabelText: sender.LabelText,
            pagination: 1//,
            //account: app.account,
            //password: app.password
        });
    },
    createColumns: function (data, sender) {

        if (data.message) {
            console.debug(data.message);
            return;
        }
        else {
            sender.t_rpdeclare = data.rows[0];
            if (app.isP)
                sender.t_rpdeclare.decsql = sender.t_rpdeclare.decsql.toLowerCase();
            //sender.t_rpdeclare.decsql = sender.t_rpdeclare.decsql.replace(/\(nolock\)/g, '');
            //sender.t_rpdeclare.decsql = sender.t_rpdeclare.decsql.replace(/\(NOLOCK\)/g, '');
            sender.setSQLParams(sender);

            var xml = $.parseXML(sender.t_rpdeclare.paramvalue);
            var fcol = $(xml).find('GRID').attr('FROZENCOLS');

            sender.SumFields = $(xml).find('GRID').attr('SumFields');
            sender.LabelField = $(xml).find('GRID').attr('LabelField');
            sender.LabelText = $(xml).find('GRID').attr('LabelText');


            if (fcol == '')
                fcol = '0';
            var nfcol = Number(fcol);
            var cols = [];
            var fcols = [];
            var ncol = 0;

            if (!sender.singleSelect) {
                fcols.push({
                    field: 'is_closebit',
                    checkbox: true
                });
            }
            $(xml).find('COLUMN').each(function () {

                if ($(this).attr('Visible') == '1') {

                    var fn = 'field_' + $(this).attr('FieldName').toLowerCase() + sender.IdDeclare.toString();
                    if (!app.isP)
                        fn = 'field_' + $(this).attr('FieldName') + sender.IdDeclare.toString();
                    var wd = localStorage[fn];
                    if (!wd)
                        wd = $(this).attr('Width');
                    var fm = function (value, row, index) {
                        return value;
                    };

                    if ($(this).attr('FieldName').toLowerCase() == 'image_bmp') {
                        fm = function (value, row, index) {
                            if (value)
                                return '<img src="data:image/gif;base64,' + value + '"/>';
                            else
                                return value;
                        }
                    };
                    if ($(this).attr('DisplayFormat')) {

                        fm = function (value, row, index) {
                            return app.dateformat(value, this.DisplayFormat);
                        }
                    };



                    var align = 'left';
                    if ($(this).attr('DisplayFormat').indexOf('#') > -1)
                        align = 'right';

                    if (ncol < nfcol) {

                        fcols.push(
                            {
                                field: app.isP ? $(this).attr('FieldName').toLowerCase() : $(this).attr('FieldName'),
                                title: $(this).attr('FieldCaption'),
                                width: wd,
                                sortable: true,
                                DisplayFormat: $(this).attr('DisplayFormat'),
                                align: align,
                                formatter: fm,
                                group: $(this).attr('group')

                            }
                        );

                    }
                    else {
                        cols.push(
                            {
                                field: app.isP ? $(this).attr('FieldName').toLowerCase() : $(this).attr('FieldName'),
                                title: $(this).attr('FieldCaption'),
                                width: wd,
                                sortable: true,
                                DisplayFormat: $(this).attr('DisplayFormat'),
                                align: align,
                                formatter: fm,
                                group: $(this).attr('group')
                            }
                        );
                    }
                    ncol++;
                }
            });
            sender.columns = cols;
            sender.frozenColumns = fcols;



            sender.addInitGrid(sender);
            sender.initGrid(sender);

        }
    },
    init: function () {
    },
    addInitGrid: function (sender) {
        if (sender.OKFun) {
            $(sender.id('maintab')).datagrid({
                onDblClickRow: function (index, row) {
                    sender.OKFun(sender);
                }
            });
            return;
        }

        if (!sender.t_rpdeclare['editproc'] && !sender.t_rpdeclare['keyvalue'])
            return;
        if (sender.t_rpdeclare['editproc'] == '' && !sender.t_rpdeclare['keyvalue'])
            return;


        

        var toolbar = $('<div style="padding:2px 4px"></div>').appendTo('body');

        if (sender.t_rpdeclare['editproc']) 
        if (sender.t_rpdeclare['editproc'] != '') 
        {

            //Редакторы
            sender.editor = Object.create(sender.editor_class);
            sender.editor.absid = sender.prop('editor');
            sender.editor.finder = sender;
            sender.editor.start(sender.editor);

            sender.abut = $('<a>').appendTo(toolbar);
            sender.ebut = $('<a>').appendTo(toolbar);
            sender.dbut = $('<a>').appendTo(toolbar);

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
        }

        if (sender.t_rpdeclare['keyvalue'] > 0)
        {
            //Кнопочка со строками 06/06/2019
            sender.lbut = $('<a>').appendTo(toolbar);
            sender.lbut.linkbutton({
                iconCls: 'tree-file',
                text: 'Строки',
                plain: true,
                onClick: function () {
                    var row = sender.MainTab.datagrid('getSelected');
                    if (!row) {
                        $.messager.alert(sender.t_rpdeclare.descr, 'Выберете запись', 'info');
                        return;
                    }
                    var node_text = sender.t_rpdeclare.descr + "_" + row[sender.t_rpdeclare.dispfield].toString() ;
                    var node_id = sender.t_rpdeclare.descr + "_" + row[sender.t_rpdeclare.keyfield].toString();
                    var tab = $('#tabs').tabs('getTab', node_text);
                    if (tab)
                        $('#tabs').tabs('select', node_text);
                    else {
                        let form1 = Object.create(finder);
                        form1.absid = 'app.forms.form' + node_id.toString();
                        form1.node = node_id.toString();
                        form1.IdDeclare = sender.t_rpdeclare['keyvalue'];
                        form1.SQLParams = {};
                        form1.DefaultValues = {};
                        form1.SQLParams['Account'] = app.account;
                        form1.SQLParams[sender.t_rpdeclare.keyfield] = row[sender.t_rpdeclare.keyfield].toString();
                        form1.DefaultValues[sender.t_rpdeclare.keyfield] = row[sender.t_rpdeclare.keyfield].toString();
                        form1.DefaultValues['last_change_user'] = app.account;
                        form1.DefaultValues['audtuser'] = app.account;

    
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
        sender.addTool(sender, toolbar);
        $(sender.id('maintab')).datagrid({
            toolbar: toolbar,
            onDblClickRow: function (index, row) { sender.editor.editrecord(sender.editor); }
        });



    },
    addTool: function (sender, toolbar) {

    },
    onSelect: function (index, row, sender) { },
    onInitGrid: function (sender) { },
    initGrid: function (sender) {
        sender.MainTab = $(sender.id('maintab'));
        sender.MainTab.datagrid({
            url: '/pg/runsql',
            method: 'POST',
            queryParams: {
                sql: sender.prepareSQL,
                SumFields: sender.SumFields,
                LabelField: sender.LabelField,
                LabelText: sender.LabelText,
                pagination: 1//,
                //account: app.account,
                //password: app.password
            },
            columns: [sender.columns],
            frozenColumns: [sender.frozenColumns],
            fit: true,
            singleSelect: sender.singleSelect,
            idField: sender.t_rpdeclare.keyfield,
            pagination: sender.pagination,
            pageNumber: 1,
            pageSize: 30,
            pageList: [30, 50, 100],
            remoteSort: true,
            multiSort: true,
            remoteFilter: true,
            showFooter: true,
            onResizeColumn: function (field, width) {
                var fn = 'field_' + field + sender.IdDeclare.toString();
                localStorage[fn] = width;
            },
            rowStyler: function (index, row) {
                if (row.color)
                    return 'background-color: ' + app.colorParse(row.color);
                if (row.Color)
                    return 'background-color: ' + app.colorParse(row.Color);    

            },
            onSelect: function (index, row) {
                $('.datagrid-row-selected').removeAttr('style'); this.rowselect = row;
                sender.onSelect(index, row, sender);
            },
            onBeforeSelect: function (index, row) {

                if (this.rowselect) {
                    if (this.rowselect.color)
                        $('.datagrid-row-selected').css({ background: app.colorParse(this.rowselect.color) });
                    if (this.rowselect.Color)
                        $('.datagrid-row-selected').css({ background: app.colorParse(this.rowselect.Color) });    

                }
            }
        });
        if (sender.pagination)
            sender.MainTab.datagrid('enableFilter');

        sender.onInitGrid(sender);

    },
    getcolumn: function () {

        var sql = "select iddeclare, decname, descr, dectype, decsql, keyfield, dispfield, keyvalue, dispvalue, keyparamname, dispparamname, isbasename, descript, addkeys, tablename, editproc, delproc, image_bmp, savefieldlist, p.paramvalue from t_rpdeclare d left join t_sysparams p on 'GridFind' || d.decname = p.paramname where iddeclare = " + this.IdDeclare.toString();
        var form = new FormData();
        form.append('sql', sql);
        //form.append('account', app.account);
        //form.append('password', app.password);

        var fun = this.createColumns;
        var sender = this;

        fetch('/pg/runsql', {
            method: 'POST',
            body: form
        }).then(function (result) {
            return result.json();
        }).then(function (MyJson) {
            fun(MyJson, sender);
        });
    }

};

var fi_declare = {
    __proto__: finder,
    editor_class: declare_editor
};


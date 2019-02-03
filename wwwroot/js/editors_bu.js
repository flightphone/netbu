var rootObj = {
                
    absid : '',

    id : function (d)
            {
                return ('#' + this.absid.replace(/\./g, '_').replace(/\[/g, '_').replace(/\]/g, '_') + '_' + d);
            },
    idh : function (d)
            {
                var res = this.absid.replace(/\./g, '_').replace(/\[/g, '_').replace(/\]/g, '_');
                if (d)
                    res = res + '_' + d;
                return (res);
            },        
    prop : function (p)        
            {
                return (this.absid + '.' + p);
            },
    localid : function()
    {
        let l = this.absid.split('.');
        return l[l.length - 1];
    }        

};

var tc_class = {
    __proto__:rootObj,
    record : {},
    fields :[],
    flagSet : false,
    okclick : function() {
        this.save(this);
    },
    cancelclick : function() {
         this.winedit.window('close');   
    },
    save : function (sender)
    {
        if (!sender.flagEdit)
        {
            this.winedit.window('close');
            return;
        }
        sender.saveDB(sender, sender.record, sender.aftersave);    
    },

    createSQLSave: function(sender, rec)
    {
        var savefieldlist = [];
        var vals = [];
            if (sender.finder.t_rpdeclare.savefieldlist!=null && sender.finder.t_rpdeclare.savefieldlist!='')
                savefieldlist = sender.finder.t_rpdeclare.savefieldlist.toLowerCase().split(',');
            else
                for (var key in rec)    
                savefieldlist.push(key);
        for (var i = 0; i < savefieldlist.length; i++)
        {
            //Здесь вставляем значение полей по умолчанию
            if (sender.finder.DefaultValues[savefieldlist[i]] != null)
                rec[savefieldlist[i]] = sender.finder.DefaultValues[savefieldlist[i]];
            
            var val = "null";

            if (rec[savefieldlist[i]]  || rec[savefieldlist[i]] === 0)
                val = "'" + rec[savefieldlist[i]].toString().replace(/'/g, "''") + "'";
            val = '_' + savefieldlist[i] + ' => ' + val;
            vals.push(val);
        }
        var sql = 'select * from ' + sender.finder.t_rpdeclare.editproc + '(' + vals.join(',') + ')';
        //console.debug(sql);
        return sql;
        
    },
    
    saveDBrecs: function(sender, recs, aftersave)
    {
        var sql = '';
        for (var i = 0; i < recs.length; i++)
        sql = sql + sender.createSQLSave(sender, recs[i]) + ';';

        $.post('/pg/runsql', {
            sql:sql,
            account: app.account,
            password: app.password
        }, 
        function(data){
         if (data.message)   
         {
             $.messager.alert('Ошибка.', data.message, 'error');
             
         }
         else
         {
             aftersave(sender);
         }
        });

    },

    saveDB: function(sender, rec, aftersave)
    {
        
        var sql = sender.createSQLSave(sender, rec);
        $.post('/pg/runsql', {
                                                   sql:sql,
                                                   account: app.account,
                                                   password: app.password
                                               }, 
                                               function(data){
                                                if (data.message)   
                                                {
                                                    $.messager.alert('Ошибка.', data.message, 'error');
                                                    
                                                }
                                                else
                                                {
                                                    if (data.rows.length > 0)
                                                    {
                                                        var rw = data.rows[0];
                                                        for (var key in rw)
                                                           rec[key] = rw[key];
                                                    }
                                                    aftersave(sender, rec);
                                                }
                                        });
                 
    },
    aftersave : function(sender, record){
        
           
           var row = {};
           if (sender.action == 1)   
           {
                row = sender.finder.MainTab.datagrid('getSelected');
                var rowindex = sender.finder.MainTab.datagrid('getRowIndex', row);
                for (var key in record) 
                    row[key] = record[key];
                
                sender.finder.MainTab.datagrid('refreshRow', rowindex);    
           }     
           else
           {
                for (var key in record) 
                    row[key] = record[key];
                sender.finder.MainTab.datagrid('appendRow', row);    
                var editIndex = sender.finder.MainTab.datagrid('getRows').length-1;
                sender.finder.MainTab.datagrid('selectRow', editIndex);
           }
           sender.flagEdit = false;
           sender.winedit.window('close');
    },
    
    okcombo : function(sender, n, row)
    {
        if (sender.flagSet)
            return;
        sender.flagSet = true;    
        for (var key in sender.fields[n].joinRow.fields)
        {
            sender.record[key] = row[sender.fields[n].joinRow.fields[key]];
        }
        sender.setField(sender);
        sender.flagEdit = true;
        sender.flagSet = false;
    },
    oklooksender: function (sender, n)
    {

        var row = sender.fields[n].finder.MainTab.datagrid('getSelected');
        if (!row)
        {
            $.messager.alert(sender.fields[n].finder.t_rpdeclare.descr, 'Выберете запись', 'info');    
            return;
        }
        
       
        for (var key in sender.fields[n].joinRow.fields)
        {
            
            sender.record[key] = row[sender.fields[n].joinRow.fields[key]];
        }

        sender.setField(sender);
        sender.flagEdit = true;
        sender.fields[n].finder.win.window('close');
    }, 
    cancelookup: function(n)
    {
        this.fields[n].win.window('close');
    },
    onStart: function(sender){
    },
    htmlContent: function(sender, ed)
    {

        //Удаляем компоненты
        $(sender.id('winedit')).remove();
        for (var i = 0; i < sender.fields.length; i++)
        {
           $(sender.id(sender.fields[i]['field'])).remove();
        }
        
        sender.winedit = $('<div id="'+ sender.idh('winedit') + '" style="width:700px;height:500px;padding:10px 10px 0px 10px"></div>').appendTo('body');
        var s = $('#okcancel').html();
        s = s.replace('OKFun', sender.prop('okclick'));
        s = s.replace('CloseFun', sender.prop('cancelclick'));
        s = s.replace('Content', ed);
        return s;
    },
    start: function(sender){

        if (sender.fields.length==0)
        {
            sender.fields = sender.finder.frozenColumns.concat(sender.finder.columns);

        //Типы полей
        for (var i = 0; i < sender.fields.length; i++)    
        {
            var f = sender.fields[i].DisplayFormat;
            var tp = 'text';
            var precision = 0;
            if (sender.fields[i].field == 'color')
               tp = 'color'; 
            
            if (f)
            {
                if (f.indexOf('dd.')> -1)
                {
                    tp = 'date';
                    if (f.indexOf('HH:mm')> -1)
                       tp = 'datetime';
                       
                    //tp = 'text';
                }
                else
                if (f.indexOf('#')>-1)
                {
                    tp = 'numeric';
                    var res = f.match(/0\.(0+)/);
                    if (res)
                    if (res.length > 1)
                    precision = res[1].length;
                }
            }
            sender.fields[i].tp = tp;
            sender.fields[i].precision = precision;

            
            if (tp == 'text')
            {
            var jr = app.t_sysfieldmap.filter(function(value){
                return (
                    value['decname'] == sender.finder.t_rpdeclare['decname'] &&
                    value['dstfield'].toLowerCase() == sender.fields[i].field &&
                    value['classname'] != null
                 );
            });
            if (jr.length == 1)
            {
                
                sender.fields[i].joinRow = {};
                sender.fields[i].joinRow.IdDeclare = jr[0]['iddeclare'];
                sender.fields[i].joinRow.classname = jr[0]['classname'];
                sender.fields[i].joinRow.fields = {};

                var jrf = app.t_sysfieldmap.filter(function(value){
                    return (
                        value['decname'] == sender.finder.t_rpdeclare['decname'] && 
                        value['groupdec'] == jr[0]['groupdec']	
                     );
                });
                for (var j = 0; j < jrf.length; j++)
                {
                    sender.fields[i].joinRow.fields[jrf[j]['dstfield'].toLowerCase()] = jrf[j]['srcfield'].toLowerCase();
                }
                
                if  (sender.fields[i].joinRow.classname == 'Bureau.GridCombo')
                {
                    sender.fields[i].tp = 'combo'; 
                    sender.fields[i].joinRow.textField = jr[0]['srcfield'].toLowerCase(); 
                }
            }
            }    


        }
        };

        $(sender.id('proptab')).remove();
        
        var ngroup = 0;
        var groupname = '';
        var groupid = sender.idh('group'); 

        var proptab = '<table class="datagrid-btable" cellspacing="0" cellpadding="0" style="width:100%" id="' + sender.idh('proptab') + '">' 
        + '<tbody>';  

        var ed =  '<tr class="treegrid-tr-tree">'
                + '<td colspan="2" style="border:0px">'
                + '<div style="display:block;" id="div' + groupid + ngroup.toString() + '">'
                + '<table class="datagrid-btable" cellspacing="1" cellpadding="0" style="width:100%;background-color:lightgray"><tbody>';

        for (var i = 0; i < sender.fields.length; i++)
        {
        
            let edrw = '<tr class="datagrid-row">';
            edrw = edrw + '<td style="width:50%;background-color: white"><div style="height:auto;" class="datagrid-cell">'+ sender.fields[i]['title'] + '</div></td>';
            edrw = edrw + '<td style="width:50%;background-color: white"><div style="height:auto;" class="datagrid-cell">'
            if (sender.fields[i]['field'] == 'color')
            {
                edrw = edrw + '<input  style="width:100%" class="jscolor"  id="' + sender.idh(sender.fields[i]['field']) + '"/>';
            }
            else
            {
                edrw = edrw + '<input  style="width:100%" id="' + sender.idh(sender.fields[i]['field']) + '"/>';
            }
            edrw = edrw + '</div></td></tr>';

            let newgroup = sender.fields[i].group;
            if (!newgroup || newgroup == '')
                newgroup = groupname;
            
            if (newgroup != groupname) 
            {
                groupname = newgroup;
                if (i > 0)
                {
                    //Закрываем старую группу
                    ed = ed + '</tbody></table>'
                            + '</div>'
                            + '</td>'
                            + '</tr>';
                    proptab = proptab + ed;
                    ngroup = ngroup + 1;
                    ed =   '<tr class="datagrid-row datagrid-row-over">'  
                           + '<td colspan="2" class="datagrid-cell" ><div class="tree-hit prop-expanded" onclick="app.expand(this.id)" id="' + groupid + ngroup.toString() + '"></div><b> '+ groupname +'</b></td>' 
                           + '</tr>'
                           + '<tr class="treegrid-tr-tree">'
                           + '<td colspan="2" style="border:0px">'
                           + '<div style="display:block;" id="div' + groupid + ngroup.toString() + '">'
                           + '<table class="datagrid-btable" cellspacing="1" cellpadding="0" style="width:100%;background-color:lightgray"><tbody>';
                }
                else
                {
                    ed =     '<tr class="datagrid-row datagrid-row-over">'  
                           + '<td colspan="2" class="datagrid-cell"><div class="tree-hit prop-expanded" onclick="app.expand(this.id)" id="' + groupid + ngroup.toString() + '"></div><b> '+ groupname +'</b></td>' 
                           + '</tr>' + ed;
                }

            }   

            ed = ed + edrw;
        }
        
        ed = ed + '</tbody></table>'
                + '</div>'
                + '</td>'
                + '</tr>';
        
        proptab = proptab 
                + ed
                + '</tbody>'
                + '</table>';

        
        var s = sender.htmlContent(sender, proptab);
        
        

        sender.winedit.window({
            title: sender.finder.t_rpdeclare.descr,
            modal:true,
            collapsible:false,
            minimizable:false,
            closed:true,
            
            content: s,
            onBeforeClose : function(){
                if (sender.flagEdit)
                {
                    $.messager.confirm(
                        {
                           title: sender.finder.t_rpdeclare.descr, 
                           msg: 'Сохранить изменения в записи?', 
                           fn: function (r) {if (r) {
                                    sender.save(sender);
                                }
                                else
                                {
                                    sender.flagEdit = false;
                                    sender.winedit.window('close');
                                }
                            },
                          ok: "Да",
                          cancel: "Нет"  
                        });    
                    return false;
                }
                else
                {
                    return true;
                }    
            }
            });
        
        for (var i = 0; i < sender.fields.length; i++)
        { 
            let fname = sender.fields[i]['field'];
            let tp = sender.fields[i].tp;
            if (tp == 'date')
                $(sender.id(fname)).datebox({
                    
                    
                    onChange:function(newValue, oldValue){
                        let date = app.dateparser(newValue);
                        sender.record[fname] = date;
                        //console.debug(date);
                        sender.flagEdit = true;
                    }
                });
            else
            if (tp == 'datetime')
                $(sender.id(fname)).datetimebox({
                   
                    showSeconds: false,
                    onChange:function(newValue, oldValue){
                        let date = app.datetimeparser(newValue);
                        sender.record[fname] = date;
                        //console.debug(date);
                        sender.flagEdit = true;
                    }
                });
            else
            if (tp == 'numeric')
                $(sender.id(fname)).numberbox({precision: sender.fields[i].precision,
                    decimalSeparator: ',',
                    onChange: function (newValue, oldValue){
                        sender.record[fname] = newValue;
                        sender.flagEdit = true;
    
                    }
                });
            else
            if (tp=='combo')
            {
                let cnum = i;
                let textfield = sender.fields[cnum].joinRow.textField;
                
                $(sender.id(fname)).combobox({
                    url: '/pg/runsql',
                    method: 'POST',
                    queryParams: {
                        IdDeclare: sender.fields[i].joinRow.IdDeclare,
                        array : 1,
                        account : app.account,
                        password : app.password
                    },
                    valueField : textfield,
                    textField : textfield,
                    onSelect : function (record) {
                        sender.okcombo(sender, cnum, record)
                    }
                }); 
            }
            else
            if (tp == 'color')
            {
                
                var elm = document.getElementById(sender.idh(fname));
                let fnn = fname;
                sender.Color = new jscolor(elm, 
                    {   
                        closable:true,
                        closeText:'Ok', 
                        required:false,
                        onFineChange: function()
                        {
                            if ($(sender.id(fnn)).val() == '')
                                sender.record['color'] = '';
                            else
                            {    
                                let c = (255 << 24 | Math.round(sender.Color.rgb[0]) << 16 | Math.round(sender.Color.rgb[1]) << 8 | Math.round(sender.Color.rgb[2]));
                                sender.record['color'] = c;
                            }
                            sender.flagEdit = true;
                        }
                    });
                
            }
            else
            {
            $(sender.id(fname)).textbox({
                onChange: function (newValue, oldValue){
                    sender.record[fname] = newValue;
                    sender.flagEdit = true;

                }    
            });
            
            if (sender.fields[i].joinRow || sender.fields[i].field == 'image_bmp')
            {
                let fnum = i;
                if (!sender.fields[i].joinRow)
                    sender.fields[i].joinRow = {};
                    
                if (sender.fields[i].joinRow.finder_class)
                    sender.fields[i].finder = Object.create(sender.fields[i].joinRow.finder_class)
                else
                if (sender.fields[i].field == 'image_bmp')
                {
                    sender.fields[i].finder = Object.create(image_editor);
                }    
                else
                    sender.fields[i].finder = Object.create(finder);


                sender.fields[i].finder.absid = sender.prop('fields[' + i.toString()+ '].finder');
                sender.fields[i].finder.IdDeclare = sender.fields[i].joinRow.IdDeclare;
                sender.fields[i].finder.editform = sender;
                sender.fields[i].finder.fnum = fnum;
                sender.fields[i].finder.OKFun = function (sen){
                    sen.editform.oklooksender(sen.editform, sen.fnum);
                };
                sender.fields[i].finder.start();
                $(sender.id(fname)).textbox({icons: [{
                    iconCls:'icon-combo',
                    handler: function(e){
                        sender.showFinder(sender, fnum);
                    }
                }]
            });
            }
           }
        }    
        sender.onStart(sender);
    },
    showFinder : function(sender, n)
    {
        //sender.fields[n].finder.win.window('open');
        sender.fields[n].finder.show(sender.fields[n].finder);
        sender.onShowFinder(sender , n);
    },
    onShowFinder: function(sender , n)
    {},
    onEdit :  function (sender)
    {},
    editrecord :function(sender)
    {
        var row = sender.finder.MainTab.datagrid('getSelected');
        if (!row)
        {
            $.messager.alert(sender.finder.t_rpdeclare.descr, 'Выберете запись', 'info');    
            return;
        }
        sender.record = {};
        for (var key in row)    
            sender.record[key] = row[key];
        sender.action = 1;
        sender.flagEdit = false;    
        sender.setField(sender);
        sender.onEdit(sender);
        sender.winedit.window('open');

    },
    setField:function(sender)
    {
        for (var i = 0; i < sender.fields.length; i++)
        {

            let tp = sender.fields[i].tp;
            let val = sender.record[sender.fields[i]['field']];
            let iid = sender.id(sender.fields[i]['field']); 
            let iih = sender.idh(sender.fields[i]['field']); 

            if (tp == 'date')
            {
                let setval = app.dateformat(val, 'dd.MM.yyyy');
                $(iid).datebox('setValue', setval);
                
            }    
            else
            if (tp == 'datetime')
            {
                let setval = app.dateformat(val, 'dd.MM.yyyy HH:mm');
                $(iid).datetimebox('setValue', setval);
            }
            else
            if (tp == 'numeric')
                $(iid).numberbox('setValue', val);
            else
            if (tp == 'combo')
                $(iid).combobox('setValue', val);
            else    
            if (tp =='color')    
            {
                if (val)  
                {  
                    let cval = app.colorParse(val);
                    cval = cval.toUpperCase().substr(1);
                    sender.Color.fromString(cval);
                }
                else
                {
                    sender.Color.fromString('FFFFFF');
                    $(iid).val('');
                }   
                   
            }
            else     
                $(iid).textbox('setValue', val);

        }
        sender.flagEdit = false;
    },
    addrecord :function(sender)
    {
        var table_name = sender.finder.t_rpdeclare.editproc.toLowerCase();
        var keyfield = sender.finder.t_rpdeclare.keyfield.toLowerCase();
        table_name = table_name.replace('p_', '').replace('_edit','');
        var url = '/pg/getid/' + table_name;
        $.get( url, function( data ) {
            sender.record = {};
            sender.record[keyfield] = data.id;
            sender.setField(sender);
            sender.action = 0;
            sender.flagEdit = false;  
            sender.onEdit(sender);  
            sender.winedit.window('open');    
          });
        

    },
    deleterecord: function(sender)
    {
        var row = sender.finder.MainTab.datagrid('getSelected');
        if (!row){
            $.messager.alert(sender.finder.t_rpdeclare.descr, 'Выберете запись', 'info');    
            return;
        }
        var rowindex = sender.finder.MainTab.datagrid('getRowIndex', row);  
        
        $.messager.confirm(
            {
               title: sender.finder.t_rpdeclare.descr, 
               msg: 'Удалить запись?', 
               fn: function (r) { if (r) {
                        var val = "'" + row[sender.finder.t_rpdeclare.keyfield.toLowerCase()].toString().replace(/'/g, "''") + "'"
                        var sql = 'select * from ' + sender.finder.t_rpdeclare.delproc + '(' + val + ')';
                        $.post('/pg/runsql', {
                                                           sql:sql,
                                                           account: app.account,
                                                           password: app.password
                                                       }, 
                                                       function(data){
                                                        if (data.message)   
                                                        {
                                                            $.messager.alert('Ошибка.', data.message, 'error');
                                                            
                                                        }
                                                        else
                                                        {
                                                            sender.finder.MainTab.datagrid('deleteRow', rowindex);
                                                        }
                                                });
                         
                        }
                            
                    },
                
              ok: "Да",
              cancel: "Нет"  
            });   
        
            
        
    }
};



var column_editor = {
    __proto__:rootObj,
    rows : [],
    show : function(sender)
        {
            //sender.win.window('open');
            var sql = "select paramvalue from t_sysParams  where  paramname = 'GridFind" + sender.editform.record['decname'] + "'";
            $.post('/pg/runsql', {
                sql:sql,
                account: app.account,
                password: app.password
            }, 
            function(data){
                 var sXML = '';
                 if (data.rows.length > 0)
                 {
                     sXML = data.rows[0]['paramvalue'];
                 }
                 sender.initdata(sender, sXML);
             
            });

        },

    initdata(sender, sXML) 
    {
        $('#teXML').textbox('setValue', sXML);
        sender.rows = [];
        
        if (sXML && sXML != '')
        {
            try
            {
              var xml = $.parseXML(sXML);
              

              $('#FROZENCOLS').textbox('setValue', $(xml).find('GRID').attr('FROZENCOLS'));
              $('#SumFields').textbox('setValue', $(xml).find('GRID').attr('SumFields'));
              $('#LabelField').textbox('setValue', $(xml).find('GRID').attr('LabelField'));
              $('#LabelText').textbox('setValue', $(xml).find('GRID').attr('LabelText'));
              var n = 0;
              $(xml).find('COLUMN').each(function(){

                sender.rows.push(
                    {
                        Visible : $(this).attr('Visible'),
                        NN : n,
                        FieldName : $(this).attr('FieldName').toLowerCase(), 
                        FieldCaption: $(this).attr('FieldCaption'), 
                        Width : $(this).attr('Width'), 
                        DisplayFormat : $(this).attr('DisplayFormat'),
                        group : $(this).attr('group')
                        
                    });
                n = n + 1;    
              });

            } 
            catch(ex)  
            {

            }
        }
        else
        {
              $('#FROZENCOLS').textbox('setValue', '0');
              $('#SumFields').textbox('setValue', '');
              $('#LabelField').textbox('setValue', '');
              $('#LabelText').textbox('setValue', 'Итого:');
        }
        var sql = sender.editform.record['decsql'] + " limit 1";
        $.post('/pg/runsql', {
            sql:sql,
            account: app.account,
            password: app.password
        }, 
        function(data){
             //console.debug(data);
             var fi = [];
             if (data.fields)
             {
                fi = data.fields;
             }
             sender.addinitdata(sender, fi);
         
        });
        
    },  
    
    addinitdata : function (sender, fi)
    {
        var n = sender.rows.length;
        for (var i = 0; i < fi.length; i++)
        {
            let fname = fi[i].name;
            let a = sender.rows.filter(function(e){
                return (e.FieldName == fname);
            });
            if (a.length == 0)
            {
                sender.rows.push(
                    {
                        Visible : '0',
                        NN : n,
                        FieldName : fname, 
                        FieldCaption: fname, 
                        Width : '100', 
                        DisplayFormat : '',
                        group : ''
                        
                    });
                n = n + 1;
            }
        }
        sender.win.window('open');
        $('#customtab').editIndex = undefined;
        $('#customtab').datagrid({data: sender.rows});
    },
    Save : function (sender)
    {
        var rs = sender.rows.sort(function(a, b){
            return (a.NN - b.NN);
        });


        var t ='<GRID FROZENCOLS="' + $('#FROZENCOLS').val()  
                + '" SumFields = "' + $('#SumFields').val()
                + '" LabelField = "' + $('#LabelField').val()
                + '" LabelText = "' + $('#LabelText').val() + '">';
                                    

        for (var i= 0; i <rs.length; i++)
        {
            let gp = '';
            if (rs[i].group)    
                gp = rs[i].group;
            t = t + '<COLUMN FieldName="' + rs[i].FieldName
                            + '" FieldCaption = "' + rs[i].FieldCaption
                            + '" DisplayFormat = "' + rs[i].DisplayFormat
                            + '" Width = "' + rs[i].Width
                            + '" Visible = "' + rs[i].Visible
                            + '" group = "' + gp;
            t = t + '" Sum = "0"  ColSort="1" />';

        }

        t = t +  '<SAFEDEF SAFEDEF="1"/></GRID>';
        $('#teXML').textbox('setValue', t);
        sender.SaveXML(sender);

    },
    SaveXML : function(sender){
        
        var ParamName = 'GridFind' + sender.editform.record['decname'];
        var ParamValue = $('#teXML').val();
        var sql = "select p_lbrsetparam ('" + ParamName + "', '" + ParamValue + "', '" + ParamName + "')";

        $.post('/pg/runsql', {
            sql:sql,
            account: app.account,
            password: app.password
        }, 
        function(data){
            //console.debug(sql);
            //console.debug(data);
            sender.win.window('close');
        });

    },
    start : function()
    {
            $(this.id('win')).remove();   
            var s = $('#frmcustom').html();
            this.win = $('<div id="'+ this.idh('win') + '" style="width:700px;height:500px;"></div>').appendTo('body');
            this.win.window({
                title: 'Колонки',
                modal:true,
                collapsible:false,
                minimizable:false,
                closed:true,
                content: s
                });

           let sender = this;
                
                $('#customtab').datagrid({
                    idField: 'FieldName',
                    checkOnSelect : false,
                    selectOnCheck : false,
                    columns:[[
                        {field: 'Visible', title: '', width : 20, 
                        
                        editor:{
                            type:'checkbox',
                            options:{
                                on: '1',
                                off: '0'
                            }
                        },
                        styler: function(value,row,index){
                                return 'background-color:#ffffff;';
                        }
                        },
                        {field: 'NN', title: 'NN', width : 30, editor:'numberbox', sortable : true },
                        {field: 'FieldName', title: 'Поле',  width : 100 },
                        {field: 'FieldCaption', title: 'Заголовок',  width : 100, editor:'textbox'},
                        {field: 'Width', title: 'Ширина',  width : 100, editor:'numberbox'},
                        {field: 'DisplayFormat', title: 'Формат',  width : 100, editor:'textbox'},
                        {field: 'group', title: 'Группа',  width : 100, editor:'textbox'}
                    ]],
                    fitColumns : true,
                    fit:true,
                    singleSelect: true,
                    sortName : 'NN',
                    sortOrder: 'asc',
                    remoteSort: false,
                    toolbar: [ {
                        iconCls: 'icon-save',
                        text: 'Сохранить',
                        handler: function(){
                            sender.Save(sender);
                        }
                        },'-',{
                        iconCls: 'tree-file',
                        text: 'Сохранить XML',
                        handler: function()
                        {
                            sender.SaveXML(sender);
                        }
                        }],
                        onClickCell: function (index, field){
                            if (this.editIndex != index){
                                
                                var endEditing = true;   
                                if (this.editIndex == undefined)
                                { 
                                    endEditing =  true; 
                                }
                                else
                                {
                                if ($(this).datagrid('validateRow', this.editIndex)){
                                    $(this).datagrid('endEdit', this.editIndex);
                                    this.editIndex = undefined;
                                    endEditing = true;
                                } else {
                                    endEditing = false;
                                }
                                }

                                if (endEditing){
                                    $(this).datagrid('selectRow', index)
                                            .datagrid('beginEdit', index);
                                    this.editIndex = index;
                                } else {
                                    setTimeout(function(){
                                        $(this).datagrid('selectRow', editIndex);
                                    },0);
                                }
                            }
                        }
                          


                    });    
    }
};

var image_editor = {
    __proto__:rootObj,
    show : function(sender)
    {
        sender.file.click();
    },
    start : function()
    {
        $(this.id('fileElem')).remove();  
        this.file = $('<input type="file" id="' + this.idh('fileElem') +  '" accept="image/*" style="display:none" onchange="' + this.prop('handleFiles(this.files)') + '">').appendTo('body');
    },
    handleFiles : function (files) {
		if (!files.length) 
            return;	
        var reader = new FileReader();
        var iid = this.editform.id('image_bmp');
        reader.onloadend = function() {
            var base64data = reader.result;                
            var base64 = base64data.split(',')[1];
            $(iid).textbox('setValue', base64);
            
        }
        reader.readAsDataURL(files[0]);
    }        
};

var declare_editor = {
    __proto__ : tc_class,
    onShowFinder : function (sender, n)
    {},
    onStart : function (sender)
    {
        $(sender.id('winedit')).window('open');
        let grid = sender.idh('group1');
        app.expand(grid);
        $(sender.id('winedit')).window('close');
    },
    fields :[
        {title: 'Код', field : 'iddeclare', tp: 'numeric'},
        {title: 'Название', field : 'decname'},
        {title: 'Описание', field : 'descr'},
        {title: 'Тип', field : 'dectype', tp: 'numeric'},
        {title: 'Ключевое поле', field : 'keyfield'},
        {title: 'Отображаемое поле', field : 'dispfield'},
        /*{title: 'DetailDeclare', field : 'keyvalue'},
        {title: 'EditForm', field : 'dispvalue'},
        {title: 'CloseBitProc', field : 'keyparamname'},
        {title: 'DispParamName', field : 'dispparamname'},
        {title: 'AddKeys', field : 'addkeys'},
        */
        {title: 'Таблица', field : 'tablename'},
        {title: 'Запрос', field : 'decsql'},
        {title: 'Картинка', field : 'image_bmp'},
        {title: 'Колонки', field : 'col_grid',
        joinRow : {
                     finder_class: column_editor
                  }
        },

        {title: 'Функция обновления', field : 'editproc'},
        {title: 'Функция удаления', field : 'delproc'},
        {title: 'Список полей', field : 'savefieldlist'}

    ]
};   


//export {rootObj, tc_class, column_editor, image_editor, declare_editor};
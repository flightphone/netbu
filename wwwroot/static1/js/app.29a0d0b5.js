(function(t){function e(e){for(var a,o,c=e[0],s=e[1],l=e[2],u=0,p=[];u<c.length;u++)o=c[u],Object.prototype.hasOwnProperty.call(i,o)&&i[o]&&p.push(i[o][0]),i[o]=0;for(a in s)Object.prototype.hasOwnProperty.call(s,a)&&(t[a]=s[a]);d&&d(e);while(p.length)p.shift()();return r.push.apply(r,l||[]),n()}function n(){for(var t,e=0;e<r.length;e++){for(var n=r[e],a=!0,c=1;c<n.length;c++){var s=n[c];0!==i[s]&&(a=!1)}a&&(r.splice(e--,1),t=o(o.s=n[0]))}return t}var a={},i={app:0},r=[];function o(e){if(a[e])return a[e].exports;var n=a[e]={i:e,l:!1,exports:{}};return t[e].call(n.exports,n,n.exports,o),n.l=!0,n.exports}o.m=t,o.c=a,o.d=function(t,e,n){o.o(t,e)||Object.defineProperty(t,e,{enumerable:!0,get:n})},o.r=function(t){"undefined"!==typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(t,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(t,"__esModule",{value:!0})},o.t=function(t,e){if(1&e&&(t=o(t)),8&e)return t;if(4&e&&"object"===typeof t&&t&&t.__esModule)return t;var n=Object.create(null);if(o.r(n),Object.defineProperty(n,"default",{enumerable:!0,value:t}),2&e&&"string"!=typeof t)for(var a in t)o.d(n,a,function(e){return t[e]}.bind(null,a));return n},o.n=function(t){var e=t&&t.__esModule?function(){return t["default"]}:function(){return t};return o.d(e,"a",e),e},o.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},o.p="/";var c=window["webpackJsonp"]=window["webpackJsonp"]||[],s=c.push.bind(c);c.push=e,c=c.slice();for(var l=0;l<c.length;l++)e(c[l]);var d=s;r.push([0,"chunk-vendors"]),n()})({0:function(t,e,n){t.exports=n("56d7")},1907:function(t,e,n){"use strict";var a=n("c3d5"),i=n.n(a);i.a},"56d7":function(t,e,n){"use strict";n.r(e),n.d(e,"openMap",(function(){return Y})),n.d(e,"mainObj",(function(){return z})),n.d(e,"openIDs",(function(){return X})),n.d(e,"prodaction",(function(){return H})),n.d(e,"baseUrl",(function(){return G}));n("4ec9"),n("d3b7"),n("3ca3"),n("ddb0"),n("e260"),n("e6cf"),n("cca6"),n("a79d");var a=n("2b0e"),i=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("v-app",[n("v-dialog",{attrs:{persistent:""},model:{value:t.mainObj.openAlert,callback:function(e){t.$set(t.mainObj,"openAlert",e)},expression:"mainObj.openAlert"}},[n("v-card",[n("v-card-title",{staticClass:"headline"},[t._v(t._s(t.mainObj.alertTitle))]),n("v-card-text",[t._v(t._s(t.mainObj.alertText))]),n("v-card-actions",[n("v-spacer"),n("v-btn",{attrs:{color:"green darken-1",text:""},on:{click:function(e){return t.handleClose()}}},[t._v("ОК")]),t.mainObj.alertConfirm?n("v-btn",{attrs:{color:"green darken-1",text:""},on:{click:function(e){t.mainObj.openAlert=!1}}},[t._v("Отмена")]):t._e()],1)],1)],1),n("v-navigation-drawer",{attrs:{absolute:"",temporary:"",width:"auto",left:""},model:{value:t.mainObj.drawer,callback:function(e){t.$set(t.mainObj,"drawer",e)},expression:"mainObj.drawer"}},[n("v-app-bar",{attrs:{app:""}},[n("v-btn",{attrs:{icon:"",disabled:!(t.mainObj.curhistory>0)},on:{click:function(e){return t.back()}}},[n("v-icon",[t._v("mdi-chevron-left")])],1),n("v-btn",{attrs:{icon:"",disabled:!(t.mainObj.curhistory<t.mainObj.history.length-1)},on:{click:function(e){return t.next()}}},[n("v-icon",[t._v("mdi-chevron-right")])],1),n("v-spacer"),n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.exit()}}},[n("v-icon",[t._v("mdi-exit-to-app")])],1)],1),n("v-main",[t.loading?n("p",[t._v("Загрузка...")]):n("v-treeview",{attrs:{items:t.treejson,hoverable:t.hoverable,"open-on-click":t.openOnClick,dense:""},scopedSlots:t._u([{key:"label",fn:function(e){var a=e.item;return[n("div",{on:{click:function(e){return t.handleselect(a)}}},[t._v(t._s(a.text))])]}}])})],1)],1),t._l(t.openIDs,(function(t){return[n("uni-comp",{key:t,attrs:{id:t}})]}))],2)},r=[],o=(n("a434"),n("96cf"),n("1da1")),c=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("div",{attrs:{hidden:!t.visible}},[n("v-app-bar",{attrs:{app:"",color:"primary",dark:""}},[n("v-app-bar-nav-icon",{on:{click:function(e){t.mainObj.drawer=!0}}}),n("v-toolbar-title"),n("v-spacer")],1),n("v-main")],1)},s=[],l={name:"Comp1",data:function(){return{mainObj:z}},props:{visible:{type:Boolean,required:!0}}},d=l,u=n("2877"),p=n("6544"),m=n.n(p),f=n("40dc"),v=n("5bc1"),h=n("f6c4"),b=n("2fa4"),g=n("2a7f"),w=Object(u["a"])(d,c,s,!1,null,null,null),y=w.exports;m()(w,{VAppBar:f["a"],VAppBarNavIcon:v["a"],VMain:h["a"],VSpacer:b["a"],VToolbarTitle:g["a"]});var k=n("f397"),_=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("Finder",{ref:"dogFind",attrs:{id:t.id,visible:t.visible,params:t.params}},[[n("a",{ref:"fileLink",attrs:{target:"_blanck",hidden:""}}),n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.openfile()}}},[n("v-icon",[t._v("mdi-file-document")])],1)]],2)},O=[],D=(n("25f0"),{name:"Dogovors",data:function(){return{}},props:{visible:{type:Boolean,required:!0},id:String,params:String},components:{Finder:k["default"]},methods:{openfile:function(){var t=Y.get(this.id).data.curRow;this.$refs.fileLink.href=this.fileUrl(t),this.$refs.fileLink.click()},fileUrl:function(t){if(!(t<0||t>Y.get(this.id).data.MainTab.length-1)){var e=Y.get(this.id).data.MainTab[t]["agr_key"].toString(),n=G+"Docfiles/dir?id="+e+"/";return n}}}}),F=D,S=n("8336"),R=n("132d"),x=Object(u["a"])(F,_,O,!1,null,null,null),M=x.exports;m()(x,{VBtn:S["a"],VIcon:R["a"]});var j=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("Finder",{ref:"uxrepFind",attrs:{id:t.id,visible:t.visible,params:t.params}},[[n("a",{ref:"fileLink",attrs:{target:"_blanck",hidden:""}}),n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.openfile()}}},[n("v-icon",[t._v("mdi-file-document")])],1)]],2)},T=[],P={name:"Uxrep",data:function(){return{}},props:{visible:{type:Boolean,required:!0},id:String,params:String},components:{Finder:k["default"]},methods:{openfile:function(){var t=Y.get(this.id).data.curRow;this.$refs.fileLink.href=this.fileUrl(t),this.$refs.fileLink.click()},fileUrl:function(t){if(!(t<0||t>Y.get(this.id).data.MainTab.length-1)){var e=Y.get(this.id).data.MainTab[t]["FC_PK"].toString(),n=G+"Flight?FC_PK="+e;return n}}}},C=P,V=Object(u["a"])(C,j,T,!1,null,null,null),E=V.exports;m()(V,{VBtn:S["a"],VIcon:R["a"]});var L={name:"App",data:function(){return{openIDs:X,mainObj:z,loading:!0,treejson:[],hoverable:!1,openOnClick:!0}},methods:{back:function(){z.curhistory>0&&(z.curhistory=z.curhistory-1,z.drawer=!1,z.current=z.history[z.curhistory])},next:function(){z.curhistory<z.history.length-1&&(z.curhistory=z.curhistory+1,z.drawer=!1,z.current=z.history[z.curhistory])},exit:function(){window.location=G+"Home/logout"},handleselect:function(t){t.children||(this.open(t),z.drawer=!1)},handleClose:function(){z.openAlert=!1,z.alertConfirm&&z.confirmAction()},open:function(t){var e=t.id;if(null==Y.get(e)){var n=this.getForm(t),a={Control:n.Conrol,Params:n.Params,SQLParams:n.SQLParams,data:{}};Y.set(e,a),X.push(e)}z.current=e,z.curhistory=z.curhistory+1,z.history.splice(z.curhistory,z.history.length,e)},getForm:function(t){var e=t.attributes,n=e.params?k["default"]:y,a=e.params,i=null;return"RegulationPrint.Dgs.DogovorList"==e.link1&&(n=M),"1550"==a&&(n=E),"1451"==a&&(n=E),{Conrol:n,Params:a,SQLParams:i}}},mounted:function(){var t=Object(o["a"])(regeneratorRuntime.mark((function t(){var e,n,a,i;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return e=document.getElementsByTagName("html")[0],e.style.overflowY="hidden",n=G+"ustore/gettree",t.next=5,fetch(n,{method:"POST",mode:H?"no-cors":"cors",cache:"no-cache",credentials:H?"include":"omit"});case 5:return a=t.sent,t.next=8,a.json();case 8:i=t.sent,this.treejson=i,this.loading=!1;case 11:case"end":return t.stop()}}),t,this)})));function e(){return t.apply(this,arguments)}return e}()},N=L,W=n("7496"),$=n("b0af"),I=n("99d9"),A=n("169a"),B=n("f774"),K=n("eb2a"),Q=Object(u["a"])(N,i,r,!1,null,null,null),U=Q.exports;m()(Q,{VApp:W["a"],VAppBar:f["a"],VBtn:S["a"],VCard:$["a"],VCardActions:I["a"],VCardText:I["b"],VCardTitle:I["c"],VDialog:A["a"],VIcon:R["a"],VMain:h["a"],VNavigationDrawer:B["a"],VSpacer:b["a"],VTreeview:K["a"]});n("5363");var J=n("f309");a["a"].use(J["a"]);var q=new J["a"]({theme:{},icons:{iconfont:"mdi"}});a["a"].config.productionTip=!0;var H=!0,z={message:"ого",drawer:!1,current:"-1",openAlert:!1,alert:function(t,e){this.alertConfirm=!1,this.alertTitle=t,this.alertText=e,this.openAlert=!0},confirm:function(t,e,n){this.alertConfirm=!0,this.alertTitle=t,this.alertText=e,this.confirmAction=n,this.openAlert=!0},history:["-1"],curhistory:0},G=H?"":"http://192.168.43.81:5000/",Y=new Map;Y.set("-1",{Control:y,Params:"",SQLParams:{},data:{}});var X=[];X.push("-1"),a["a"].component("uni-comp",{data:function(){return{openMap:Y,mainObj:z}},props:{id:{type:String,required:!0}},render:function(t){var e=Y.get(this.id).Control,n=Y.get(this.id).Params,a=z.current==this.id;return t(e,{props:{id:this.id,params:n,visible:a}})}}),new a["a"]({vuetify:q,render:function(t){return t(U)}}).$mount("#app")},c3d5:function(t,e,n){},f397:function(t,e,n){"use strict";n.r(e);var a=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("div",{staticStyle:{height:"100vh",maxheight:"100vh",overflow:"auto"},attrs:{hidden:!t.visible}},[n("div",{staticStyle:{height:"100vh",maxheight:"100vh",overflow:"auto"},attrs:{hidden:"grid"!=t.mode}},[t.stateDrawer?t._e():n("v-app-bar",{attrs:{app:"",color:"primary",dark:"","max-width":"100vw"}},[null==t.editid?n("v-app-bar-nav-icon",{on:{click:function(e){t.mainObj.drawer=!0}}}):t._e(),n("v-toolbar-title",[t._v(t._s(t.Descr))]),n("v-spacer"),null!=t.editid?[n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.selectFinder(t.editid)}}},[n("v-icon",[t._v("mdi-check")])],1),n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.clearFinder()}}},[n("v-icon",[t._v("mdi-window-close")])],1)]:t._e(),t._t("default"),n("v-menu",{attrs:{left:""},scopedSlots:t._u([{key:"activator",fn:function(e){var a=e.on,i=e.attrs;return[n("v-btn",t._g(t._b({attrs:{icon:""}},"v-btn",i,!1),a),[n("v-icon",[t._v("mdi-dots-vertical")])],1)]}}],null,!1,3221905750)},[n("v-list",[n("v-list-item-group",[null!=t.editid||t.load?t._e():[t.OpenMapData().EditProc?n("v-list-item",{key:"6",on:{click:function(e){return t.add()}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-plus")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Добавить")])],1)],1):t._e(),n("v-list-item",{key:"7",on:{click:function(e){return t.edit()}}},[n("v-list-item-icon",[t.OpenMapData().EditProc?n("v-icon",[t._v("mdi-pencil")]):n("v-icon",[t._v("mdi-magnify")])],1),n("v-list-item-content",[t.OpenMapData().EditProc?n("v-list-item-title",[t._v("Редактировать")]):n("v-list-item-title",[t._v("Просмотр")])],1)],1),t.OpenMapData().DelProc?n("v-list-item",{key:"8",on:{click:function(e){return t.confirmDelete()}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-delete")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Удалить")])],1)],1):t._e()],t.load?t._e():[n("v-list-item",{key:"1",on:{click:function(e){t.mode="filter"}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-filter-menu")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Фильтровка и сортировка")])],1)],1),n("v-list-item",{key:"2",on:{click:function(e){t.stateDrawer=!0}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-code-tags")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Страницы")])],1)],1)],null!=t.editid||t.load?t._e():[n("v-list-item",{key:"3",on:{click:function(e){return t.csv()}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-cloud-download")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Экспорт в CSV")])],1)],1),n("v-list-item",{key:"11",on:{click:function(e){return t.loadFile()}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-database-plus")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Файлы")]),n("a",{ref:"fileLink2",attrs:{target:"_blanck",hidden:""}})],1)],1),t.OpenMapData().KeyValue?n("v-list-item",{key:"4",on:{click:function(e){return t.openDetail()}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-details")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Детали")])],1)],1):t._e()],t.OpenMapData().IdDeclareSet&&!t.load?n("v-list-item",{key:"5",on:{click:function(e){return t.editSetting()}}},[n("v-list-item-icon",[n("v-icon",[t._v("mdi-cog")])],1),n("v-list-item-content",[n("v-list-item-title",[t._v("Параметры")])],1)],1):t._e()],2)],1)],1)],2),t.stateDrawer?n("Pagination",{attrs:{findData:t.OpenMapData()}}):t._e(),n("v-main",[t.load?t._e():n("v-simple-table",{attrs:{dense:""},scopedSlots:t._u([{key:"default",fn:function(){return[n("thead",[n("tr",t._l(t.OpenMapData().Fcols,(function(e){return n("th",{key:e.FieldName},[t._v(t._s(e.FieldCaption))])})),0)]),t.nupdate>0?n("tbody",t._l(t.OpenMapData().MainTab,(function(e,a){return n("tr",{key:a,class:{selected:a==t.current},on:{click:function(e){return t.handleClick(a)}}},t._l(t.OpenMapData().Fcols,(function(a){return n("td",{key:a.FieldName},[t._v(t._s(""==a.DisplayFormat?e[a.FieldName]:t.dateformat(e[a.FieldName],a.DisplayFormat)))])})),0)})),0):t._e()]},proxy:!0}],null,!1,1845836641)})],1)],1),n("div",{staticStyle:{height:"100vh",maxheight:"100vh",overflow:"auto"},attrs:{hidden:"filter"!=t.mode}},[n("v-app-bar",{attrs:{app:"",color:"primary",dark:"","max-width":"100vw"}},[n("v-toolbar-title",[t._v("Фильтры, сортировка")]),n("v-spacer"),n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.updateTab()}}},[n("v-icon",[t._v("mdi-check")])],1),n("v-btn",{attrs:{icon:""},on:{click:function(e){t.mode="grid"}}},[n("v-icon",[t._v("mdi-window-close")])],1)],1),n("v-main",[t.load?t._e():n("v-simple-table",{attrs:{dense:""},scopedSlots:t._u([{key:"default",fn:function(){return[n("tbody",t._l(t.OpenMapData().Fcols,(function(e,a){return n("tr",{key:e.FieldName},[n("td",[n("v-text-field",{attrs:{label:e.FieldCaption},model:{value:e.FindString,callback:function(n){t.$set(e,"FindString",n)},expression:"column.FindString"}})],1),n("td",[t._v(" "+t._s(e.SortOrder)+" "),n("span",{attrs:{hidden:""}},[t._v(t._s(t.rangSort))])]),n("td",[n("v-select",{attrs:{items:t.items},on:{change:function(e){return t.sortChange(e,a)}},model:{value:e.Sort,callback:function(n){t.$set(e,"Sort",n)},expression:"column.Sort"}})],1)])})),0)]},proxy:!0}],null,!1,3915815090)})],1)],1),n("div",{staticStyle:{height:"100vh",maxheight:"100vh",overflow:"auto"},attrs:{hidden:!("edit"==t.mode||"add"==t.mode)}},[null!=t.editid||t.load?t._e():n("Editor",{attrs:{save:t.save,closeEditor:t.closeEditor,action:t.mode,findData:t.OpenMapData(),uid:t.uid,readonly:!t.OpenMapData().EditProc}})],1),n("div",{staticStyle:{height:"100vh",maxheight:"100vh",overflow:"auto"},attrs:{hidden:!("setting"==t.mode)}},[t.OpenMapData().IdDeclareSet&&!t.load?n("Editor",{attrs:{save:t.saveSetting,closeEditor:t.closeEditor,action:t.mode,findData:t.OpenMapData().Setting,uid:t.uid2}}):t._e()],1)])},i=[],r=(n("c975"),n("d81d"),n("a434"),n("a9e3"),n("b680"),n("d3b7"),n("ac1f"),n("25f0"),n("3ca3"),n("466d"),n("5319"),n("ddb0"),n("2b3d"),n("96cf"),n("1da1")),o=n("56d7"),c=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("v-app-bar",{attrs:{app:"",color:"primary",dark:"","max-width":"100vw"}},[n("span",{attrs:{hidden:""}},[t._v(t._s(t.$parent.action))]),t._v(" "+t._s(t.page()*t.rowsPerPage+1)+" - "+t._s(Math.min((t.page()+1)*t.rowsPerPage,t.count()))+" из "+t._s(t.count())+" "),n("v-spacer"),n("v-btn",{attrs:{icon:"",disabled:0===t.page()},on:{click:function(e){return t.$parent.onChangePage(0)}}},[n("v-icon",[t._v("mdi-page-first")])],1),n("v-btn",{attrs:{icon:"",disabled:0===t.page()},on:{click:function(e){t.$parent.onChangePage(t.page()-1)}}},[n("v-icon",[t._v("mdi-chevron-left")])],1),t._v(" "+t._s(t.page()+1)+" из "+t._s(Math.max(0,Math.ceil(t.count()/t.rowsPerPage)-1)+1)+" "),n("v-btn",{attrs:{icon:"",disabled:t.page()>=Math.ceil(t.count()/t.rowsPerPage)-1},on:{click:function(e){t.$parent.onChangePage(t.page()+1)}}},[n("v-icon",[t._v("mdi-chevron-right")])],1),n("v-btn",{attrs:{icon:"",disabled:t.page()>=Math.ceil(t.count()/t.rowsPerPage)-1},on:{click:function(e){t.$parent.onChangePage(Math.max(0,Math.ceil(t.count()/t.rowsPerPage)-1))}}},[n("v-icon",[t._v("mdi-page-last")])],1),n("v-btn",{attrs:{icon:""},on:{click:function(e){t.$parent.stateDrawer=!1}}},[n("v-icon",[t._v("mdi-window-close")])],1)],1)},s=[],l={name:"Pagination",data:function(){return{rowsPerPage:30}},props:{findData:Object},methods:{OpenMapData:function(){return this.findData},count:function(){return this.OpenMapData().TotalTab?this.OpenMapData().TotalTab[0].n_total:0},page:function(){return this.OpenMapData().page?this.OpenMapData().page-1:0}}},d=l,u=n("2877"),p=n("6544"),m=n.n(p),f=n("40dc"),v=n("8336"),h=n("132d"),b=n("2fa4"),g=Object(u["a"])(d,c,s,!1,null,null,null),w=g.exports;m()(g,{VAppBar:f["a"],VBtn:v["a"],VIcon:h["a"],VSpacer:b["a"]});var y=function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("div",{staticStyle:{height:"100vh",maxheight:"100vh",overflow:"auto"}},[n("div",{staticStyle:{height:"100vh",maxheight:"100vh",overflow:"auto"},attrs:{hidden:"edit"!=t.mode}},[n("v-app-bar",{attrs:{app:"",color:"primary",dark:"","max-width":"100vw"}},["setting"!=t.action?n("v-app-bar-nav-icon",{on:{click:function(e){t.mainObj.drawer=!0}}}):t._e(),n("v-toolbar-title",[t._v(t._s(t.Descr()))]),n("v-spacer"),t.readonly?t._e():n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.save()}}},[n("v-icon",[t._v("mdi-check")])],1),n("v-btn",{attrs:{icon:""},on:{click:function(e){return t.closeEditor()}}},[n("v-icon",[t._v("mdi-window-close")])],1)],1),n("v-main",[n("v-simple-table",{scopedSlots:t._u([{key:"default",fn:function(){return[t.nupdate>0&&""!=t.uid?n("tbody",[t.readonly?t._l(t.findData.Fcols,(function(e,a){return n("tr",{key:a},[n("td",[n("v-text-field",{key:e.FieldName+t.uid,attrs:{label:e.FieldCaption,value:t.findData.WorkRow[e.FieldName],readonly:""}})],1)])})):t._l(t.findData.ReferEdit.Editors,(function(e,a){return n("tr",{key:a},[n("td",[null==e.joinRow?n("v-text-field",{key:e.FieldName+t.uid,attrs:{label:e.FieldCaption},model:{value:t.findData.WorkRow[e.FieldName],callback:function(n){t.$set(t.findData.WorkRow,e.FieldName,n)},expression:"findData.WorkRow[column.FieldName]"}}):["Bureau.Finder"==e.joinRow.classname?n("v-text-field",{key:e.FieldName+t.uid,attrs:{label:e.FieldCaption,"append-icon":"mdi-magnify",readonly:""},on:{"click:append":function(e){return t.open(a)}},model:{value:t.findData.WorkRow[e.FieldName],callback:function(n){t.$set(t.findData.WorkRow,e.FieldName,n)},expression:"findData.WorkRow[column.FieldName]"}}):t._e(),"Bureau.GridCombo"==e.joinRow.classname?n("v-select",{key:e.FieldName+t.uid,attrs:{label:e.FieldCaption,items:e.joinRow.FindConrol.MainTab,"item-value":e.joinRow.keyField,"item-text":e.joinRow.FindConrol.DispField},on:{change:function(n){return t.sortChange(n,e)}},model:{value:t.findData.WorkRow[e.joinRow.valField],callback:function(n){t.$set(t.findData.WorkRow,e.joinRow.valField,n)},expression:"findData.WorkRow[column.joinRow.valField]"}}):t._e()]],2)])}))],2):t._e()]},proxy:!0}])})],1)],1),t.readonly?t._e():[t._l(t.findData.ReferEdit.Editors,(function(e,a){return[null!=e.joinRow?["Bureau.Finder"==e.joinRow.classname?n("Finder",{key:a,attrs:{visible:t.mode=="finder_"+a.toString(),params:e.joinRow.IdDeclare,editid:a,findData:e.joinRow.FindConrol,selectFinder:t.selectFinder,clearFinder:t.clearFinder}}):t._e()]:t._e()]}))]],2)},k=[],_={name:"Editor",data:function(){return{mode:"edit",mainObj:o["mainObj"],nupdate:1}},props:{save:Function,closeEditor:Function,action:String,findData:Object,uid:String,readonly:Boolean},beforeCreate:function(){this.$options.components.Finder=n("f397").default},methods:{Descr:function(){return"add"==this.action?"Новая запись":"edit"==this.action?this.readonly?"Просмотр":"Редактирование":"setting"==this.action?"Параметры":void 0},open:function(t){this.mode="finder_"+t.toString()},selectFinder:function(t){var e=this.findData.ReferEdit.Editors[t],n=this.findData.ReferEdit.Editors[t].joinRow.FindConrol.curRow,a=e.joinRow.FindConrol.MainTab[n];for(var i in e.joinRow.fields)this.findData.WorkRow[e.joinRow.fields[i]]=a[i];this.nupdate=this.nupdate+1,this.mode="edit"},sortChange:function(t,e){var n=this;e.joinRow.FindConrol.MainTab.map((function(a){if(a[e.joinRow.keyField]==t)for(var i in e.joinRow.fields)n.findData.WorkRow[e.joinRow.fields[i]]=a[i]})),this.nupdate=this.nupdate+1},clearFinder:function(){this.mode="edit"},textChange:function(t,e){this.findData.WorkRow[e]=t}}},O=_,D=n("5bc1"),F=n("f6c4"),S=n("b974"),R=n("1f4f"),x=n("8654"),M=n("2a7f"),j=Object(u["a"])(O,y,k,!1,null,null,null),T=j.exports;m()(j,{VAppBar:f["a"],VAppBarNavIcon:D["a"],VBtn:v["a"],VIcon:h["a"],VMain:F["a"],VSelect:S["a"],VSimpleTable:R["a"],VSpacer:b["a"],VTextField:x["a"],VToolbarTitle:M["a"]});var P={name:"Finder",components:{Pagination:w,Editor:T},data:function(){return{mainObj:o["mainObj"],openMap:o["openMap"],load:!0,mode:"grid",Descr:"Загрузка...",current:0,stateDrawer:!1,action:1,nupdate:1,nadd:1,items:["Нет","По возрастанию","По убыванию"],rangSort:0,uid:"zz",uid2:"yy"}},props:{visible:{type:Boolean,required:!0},id:String,params:String,editid:Number,findData:Object,selectFinder:Function,clearFinder:Function},computed:{},methods:{sortChange:function(t,e){var n=0,a=this.OpenMapData().Fcols;a.map((function(t,a){a!=e&&t.SortOrder&&t.SortOrder>n&&(n=t.SortOrder)})),a[e].SortOrder=n+1,this.rangSort=n+1},dateformat:function(t,e){if(!t)return t;if(24!=t.length){var n=e.match(/0\.(0+)/),a=0;return n&&n.length>1&&(a=n[1].length),a>0?Number(t.toString()).toFixed(a):t}return e=e.replace("yyyy",t.substr(0,4)),e=e.replace("yy",t.substr(2,2)),e=e.replace("MM",t.substr(5,2)),e=e.replace("dd",t.substr(8,2)),e=e.replace("HH",t.substr(11,2)),e=e.replace("mm",t.substr(14,2)),e},OpenMapData:function(){return null!=this.id?o["openMap"].get(this.id).data:this.findData},OpenMapId:function(){return o["openMap"].get(this.id)},setLoad:function(t){this.load=t},handleClick:function(t){this.OpenMapData().curRow==t?(this.current=t,null==this.editid?this.OpenMapData().EditProc?this.edit():this.OpenMapData().KeyValue?this.openDetail():this.edit():this.selectFinder(this.editid)):(this.OpenMapData().curRow=t,this.current=t)},openDetail:function(){var t=this.OpenMapData();if(null!=t.curRow){var e,n,a,i=t.MainTab[t.curRow],r={};if(i["iddeclare"]&&i["keyf"]){var c=i["keyf"],s=i["dispfield"],l=i["iddeclare"];e=i[c],r[c]=e,n=l,a=i[s]}else e=i[t.KeyF],r[t.KeyF]=e,n=t.KeyValue,a=i[t.DispField];var d={Control:P,Params:n,TextParams:r,title:a,data:{}},u=this.id+"_"+i[t.KeyF];null==o["openMap"].get(u)&&(o["openMap"].set(u,d),o["openIDs"].push(u)),o["mainObj"].current=u,o["mainObj"].curhistory=o["mainObj"].curhistory+1,o["mainObj"].history.splice(o["mainObj"].curhistory,o["mainObj"].history.length,u)}},csv:function(){var t=o["baseUrl"]+"React/csv",e=new FormData,n=this.OpenMapData(),a=this.params;e.append("id",a),e.append("Fc",JSON.stringify(n.Fcols)),n.SQLParams&&e.append("SQLParams",JSON.stringify(n.SQLParams)),n.TextParams&&e.append("TextParams",JSON.stringify(n.TextParams)),fetch(t,{method:"POST",mode:o["prodaction"]?"no-cors":"cors",cache:"no-cache",credentials:o["prodaction"]?"include":"omit",body:e}).then((function(t){return t.blob()})).then((function(t){var e=document.createElement("a");e.href=URL.createObjectURL(t),e.setAttribute("download","data.csv"),e.click()}))},loadFile:function(){var t=this.OpenMapData();if(null!=t.curRow){var e=t.MainTab[t.curRow],n=this.params,a="F"+n+"_"+e[t.KeyF],i=t.Descr+", "+e[t.DispField],r=o["baseUrl"]+"Docfiles/dir?id="+a+"/&caption="+i;this.$refs.fileLink2.href=r,this.$refs.fileLink2.click()}},confirmDelete:function(){var t=this.OpenMapData();if(null!=t.curRow){var e=t.MainTab[t.curRow],n=e[t.DispField];o["mainObj"].confirm(this.Descr,"Удалить запись '"+n+"'?",this.rowDelete)}},rowDelete:function(){var t=Object(r["a"])(regeneratorRuntime.mark((function t(){var e,n,a,i,r,c;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return e=this.OpenMapData(),n={},n[e.KeyF]=e.MainTab[e.curRow][e.KeyF],e.DelProc.toLowerCase().indexOf("_del_1")>-1&&(n["AUDTUSER"]=e.Account),a=o["baseUrl"]+"React/exec",i=new FormData,i.append("EditProc",e.DelProc),i.append("SQLParams",JSON.stringify(n)),i.append("KeyF",e.KeyF),t.next=11,fetch(a,{method:"POST",mode:o["prodaction"]?"no-cors":"cors",cache:"no-cache",credentials:o["prodaction"]?"include":"omit",body:i});case 11:return r=t.sent,t.next=14,r.json();case 14:if(c=t.sent,"OK"==c.message||"Invalid storage type: DBNull."==c.message){t.next=18;break}return o["mainObj"].alert("Ошибка",c.message),t.abrupt("return");case 18:e.MainTab.splice(e.curRow,1),this.nupdate=this.nupdate+1;case 20:case"end":return t.stop()}}),t,this)})));function e(){return t.apply(this,arguments)}return e}(),onChangePage:function(t){this.action=this.action+1,this.OpenMapData().page=t+1,this.updateTab()},updateTab:function(){var t=Object(r["a"])(regeneratorRuntime.mark((function t(){var e,n,a,i,r,c;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return e=o["baseUrl"]+"React/FinderStart",n=new FormData,a=this.OpenMapData(),i=this.params,n.append("id",i),n.append("mode","data"),n.append("page",a.page.toString()),n.append("Fc",JSON.stringify(a.Fcols)),a.SQLParams&&n.append("SQLParams",JSON.stringify(a.SQLParams)),a.TextParams&&n.append("TextParams",JSON.stringify(a.TextParams)),t.next=12,fetch(e,{method:"POST",mode:o["prodaction"]?"no-cors":"cors",cache:"no-cache",credentials:o["prodaction"]?"include":"omit",body:n});case 12:return r=t.sent,t.next=15,r.json();case 15:c=t.sent,c.Error?o["mainObj"].alert("Ошибка",c.Error):(a.MainTab=c.MainTab,a.TotalTab=c.TotalTab,a.page=c.page,this.mode,this.mode="grid",this.nupdate=this.nupdate+1);case 17:case"end":return t.stop()}}),t,this)})));function e(){return t.apply(this,arguments)}return e}(),saveSetting:function(){var t=this.OpenMapData().Setting,e=t.MainTab[0];t.ColumnTab.map((function(n){e[n]=t.WorkRow[n]}));var n=this.OpenMapData();t.ReferEdit.SaveFieldList.map((function(e){n.SQLParams["@"+e]=t.MainTab[0][e]})),this.updateTab()},save:function(){var t=Object(r["a"])(regeneratorRuntime.mark((function t(){var e,n,a,i,r,c,s,l,d,u;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:for(n in e=this.OpenMapData(),e.DefaultValues)e.WorkRow[n]=e.DefaultValues[n];for(a in e.TextParams)e.WorkRow[a]=e.TextParams[a];return i={},e.ReferEdit.SaveFieldList.map((function(t){i[t]=e.WorkRow[t]})),r=o["baseUrl"]+"React/exec",c=new FormData,c.append("EditProc",e.EditProc),c.append("SQLParams",JSON.stringify(i)),c.append("KeyF",e.KeyF),t.next=12,fetch(r,{method:"POST",mode:o["prodaction"]?"no-cors":"cors",cache:"no-cache",credentials:o["prodaction"]?"include":"omit",body:c});case 12:return s=t.sent,t.next=15,s.json();case 15:if(l=t.sent,"OK"==l.message){t.next=21;break}return o["mainObj"].alert("Ошибка",l.message),t.abrupt("return");case 21:1==l.ColumnTab.length?e.WorkRow[e.KeyF]=l.MainTab[0][l.ColumnTab[0]]:l.ColumnTab.map((function(t){e.WorkRow[t]=l.MainTab[0][t]}));case 22:d={},"edit"==this.mode&&(u=e.curRow,d=e.MainTab[u]),e.ColumnTab.map((function(t){d[t]=e.WorkRow[t]})),"add"==this.mode&&e.MainTab.push(d),this.mode="grid",this.nupdate=this.nupdate+1;case 28:case"end":return t.stop()}}),t,this)})));function e(){return t.apply(this,arguments)}return e}(),closeEditor:function(){this.mode="grid"},add:function(){var t=this.OpenMapData();null==t.WorkRow&&(t.WorkRow={}),t.ColumnTab.map((function(e){t.WorkRow[e]=""})),this.nadd=this.nadd+1,this.uid="uid"+this.nadd.toString(),this.mode="add"},edit:function(){var t=this,e=this.OpenMapData();null==e.WorkRow&&(e.WorkRow={});var n=e.curRow,a=e.MainTab[n];e.ColumnTab.map((function(t){e.WorkRow[t]=null==a[t]?"":a[t]})),e.Fcols.map((function(n){""!=n.DisplayFormat&&(e.WorkRow[n.FieldName]=t.dateformat(e.WorkRow[n.FieldName],n.DisplayFormat))})),this.nadd=this.nadd+1,this.uid="uid"+this.nadd.toString(),this.mode="edit"},editSetting:function(){var t=this,e=this.OpenMapData().Setting;e.WorkRow={};var n=e.MainTab[0];e.ColumnTab.map((function(t){e.WorkRow[t]=null==n[t]?"":n[t]})),e.Fcols.map((function(n){""!=n.DisplayFormat&&(e.WorkRow[n.FieldName]=t.dateformat(e.WorkRow[n.FieldName],n.DisplayFormat))})),this.nadd=this.nadd+1,this.uid2="uid2"+this.nadd.toString(),this.mode="setting"}},mounted:function(){var t=Object(r["a"])(regeneratorRuntime.mark((function t(){var e,n,a,i,r,c,s,l,d,u,p;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:if(e=this.OpenMapData,n=this.OpenMapId,a=this.setLoad,i=this.editid,r=this.params,null==i){t.next=10;break}return e().curRow=0,this.Descr=e().Descr+" (выбор)",a(!1),t.abrupt("return");case 10:return c=o["baseUrl"]+"React/FinderStart",s=new FormData,s.append("id",r),l=n(),l.SQLParams&&s.append("SQLParams",JSON.stringify(l.SQLParams)),l.TextParams&&s.append("TextParams",JSON.stringify(l.TextParams)),t.next=18,fetch(c,{method:"POST",mode:o["prodaction"]?"no-cors":"cors",cache:"no-cache",credentials:o["prodaction"]?"include":"omit",body:s});case 18:return d=t.sent,t.next=21,d.json();case 21:u=t.sent,u.Error?o["mainObj"].alert("Ошибка",u.Error):(u.curRow=0,u.WorkRow={},u.ColumnTab.map((function(t){u.WorkRow[t]=""})),p=n(),p.data=u,this.Descr=u.Descr,l.title&&(this.Descr=this.Descr+" ("+l.title+")"),a(!1));case 23:case"end":return t.stop()}}),t,this)})));function e(){return t.apply(this,arguments)}return e}()},C=P,V=C,E=(n("1907"),n("8860")),L=n("da13"),N=n("5d23"),W=n("1baa"),$=n("34c3"),I=n("e449"),A=Object(u["a"])(V,a,i,!1,null,"4128642d",null);e["default"]=A.exports;m()(A,{VAppBar:f["a"],VAppBarNavIcon:D["a"],VBtn:v["a"],VIcon:h["a"],VList:E["a"],VListItem:L["a"],VListItemContent:N["a"],VListItemGroup:W["a"],VListItemIcon:$["a"],VListItemTitle:N["b"],VMain:F["a"],VMenu:I["a"],VSelect:S["a"],VSimpleTable:R["a"],VSpacer:b["a"],VTextField:x["a"],VToolbarTitle:M["a"]})}});
//# sourceMappingURL=app.29a0d0b5.js.map
"use strict";(self["webpackChunkvue2a_test"]=self["webpackChunkvue2a_test"]||[]).push([[524],{615:function(t,e,i){i.d(e,{h:function(){return n}});i(1249);var s=i(8262),n=(0,s.Q_)("main",{state:function(){return{count:10,drawer:!1,openFilter:!1,menuSelectedItem:0,menuItems:[],menuVisible:!0}},getters:{double:function(t){return 2*t.count}},actions:{increment:function(){this.count++},clearItem:function(){this.menuItems=[],this.menuVisible=!1},addItem:function(t){var e=this;this.menuItems=[],t.map((function(t){e.menuItems.push(t)})),this.menuSelectedItem=0,this.menuVisible=!0}}})},6569:function(t,e,i){i.r(e),i.d(e,{default:function(){return y}});var s=function(){var t=this,e=t.$createElement,i=t._self._c||e;return i("FinderCore",{attrs:{id:t.id,params:t.params,visible:""},scopedSlots:t._u([{key:"table",fn:function(){return[t.nupdate>0?i("v-simple-table",{attrs:{dense:"",height:t.gridHeight},scopedSlots:t._u([{key:"default",fn:function(){return[t.nupdate>0?i("tbody",t._l(t.OpenMapData().MainTab,(function(e,s){return i("tr",{key:s,style:{"background-color":s==t.current?t.selectedColor:"white"},on:{click:function(e){return t.handleClick(s)}}},[i("td",[i("v-list-item-avatar",[i("v-img",{attrs:{src:t.avatar(s)}})],1)],1),i("td",[i("v-card",{staticClass:"mx-auto",style:{"background-color":s==t.current?t.selectedColor:"white"},attrs:{tile:"",flat:"","max-width":"350"}},[i("v-list-item",{attrs:{dense:""}},[i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(e["Должность"]))]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(e["ПолнИмяСотрудника"]))])],1)],1),i("v-list-item",{attrs:{dense:""}},[i("v-list-item-content",[i("v-list-item-subtitle",[t._v("Табельный номер")]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(e["ТабN"]))])],1)],1)],1)],1),i("td",[i("v-card",{staticClass:"mx-auto",style:{"background-color":s==t.current?t.selectedColor:"white"},attrs:{tile:"",flat:"","max-width":"300"}},[i("v-list-item",{attrs:{dense:""}},[i("v-list-item-content",[i("v-list-item-subtitle",[t._v("Компания")]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(e["Организация"]))])],1)],1),i("v-list-item",{attrs:{dense:""}},[i("v-list-item-content",[i("v-list-item-subtitle",[t._v("Подразделение")]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(e["Подразделение"]))])],1)],1)],1)],1),i("td",[i("v-card",{staticClass:"mx-auto",style:{"background-color":s==t.current?t.selectedColor:"white"},attrs:{tile:"",flat:"","max-width":"300"}},[i("v-list-item",{attrs:{dense:""}},[i("v-list-item-icon",[i("v-icon",[t._v("mdi-home")])],1),i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(e["ОфисСотрудника"]))])],1)],1),i("v-list-item",{attrs:{dense:""}},[i("v-list-item-icon",[i("v-icon",[t._v("mdi-phone")])],1),i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(e["CorpMobile"]))])],1)],1),i("v-list-item",{attrs:{dense:""}},[i("v-list-item-icon",[i("v-icon",[t._v("mdi-email-outline")])],1),i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(e["Email"]))])],1)],1)],1)],1)])})),0):t._e()]},proxy:!0}],null,!1,328128163)}):t._e()]},proxy:!0},{key:"editor",fn:function(){return[i("v-card",{staticClass:"mx-auto",attrs:{"max-width":"850"}},[i("table",[i("tr",[i("td",{staticStyle:{"vertical-align":"top"}},[i("v-img",{attrs:{"max-height":"250","max-width":"250",src:t.avatar(t.current)}})],1),i("td",[i("v-list-item",[i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(t.CurrentRow()["Должность"]))]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(t.CurrentRow()["ПолнИмяСотрудника"]))]),i("v-list-item-subtitle",[t._v("Табельный номер")]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(t.CurrentRow()["ТабN"]))]),i("v-list-item-subtitle",[t._v("Компания")]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(t.CurrentRow()["Организация"]))])],1)],1),i("v-list-item",[i("v-list-item-content",[i("v-list-item-subtitle",[t._v("Подразделение")]),i("v-list-item-title",{staticClass:"caption"},[t._v(t._s(t.CurrentRow()["Подразделение"]))])],1)],1)],1)])]),i("v-list-item",{attrs:{dense:""}},[i("v-list-item-icon",[i("v-icon",[t._v("mdi-home")])],1),i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(t.CurrentRow()["ОфисСотрудника"]))])],1)],1),i("v-list-item",{attrs:{dense:""}},[i("v-list-item-icon",[i("v-icon",[t._v("mdi-phone")])],1),i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(t.CurrentRow()["CorpMobile"]))])],1)],1),i("v-list-item",{attrs:{dense:""}},[i("v-list-item-icon",[i("v-icon",[t._v("mdi-email-outline")])],1),i("v-list-item-content",[i("v-list-item-subtitle",[t._v(t._s(t.CurrentRow()["Email"]))])],1)],1),i("v-card-title",[t._v(" "+t._s(t.CurrentRow()["ПолнИмяСотрудника"])+" ")])],1)]},proxy:!0}])})},n=[],l=i(615),a=i(377),r=i(5767),o={name:"UserCust",components:{FinderCore:a.Z},setup:function(){var t=(0,l.h)(),e=[];return t.addItem(e),{store:t}},beforeDestroy:function(){var t=document.getElementsByTagName("html")[0];t.style.overflowY="scroll"},created:function(){var t=document.getElementsByTagName("html")[0];t.style.overflowY="hidden"},data:function(){return{params:"1467",id:"user_XIXVII",current:0,selectedColor:r.uW.selectedColor,nupdate:1,gridHeight:r.uW.gridHeight()}},methods:{resize:function(){this.gridHeight=r.uW.gridHeight()},updateTab:function(){this.nupdate=this.nupdate+1},OpenMapData:function(){return r.M8.get(this.id).data},CurrentRow:function(){return r.M8.get(this.id).data.MainTab[this.current]},avatar:function(t){return r.FH+"Docfiles/getphoto?audtuser="+r.M8.get(this.id).data.MainTab[t]["AD_Name"]},handleClick:function(t){this.current=t,r.M8.get(this.id).handleClick(t)}},computed:{userSrcImage:function(){return this.avatar(this.current)}},mounted:function(){var t=r.M8.get(this.id);t.updateTab=this.updateTab,t.resize=this.resize}},u=o,c=i(1001),m=i(3453),v=i.n(m),d=i(3237),h=i(7118),_=i(6428),p=i(7047),f=i(7620),C=i(5457),b=i(1317),g=i(459),w=i(3568),I=(0,c.Z)(u,s,n,!1,null,null,null),y=I.exports;v()(I,{VCard:d.Z,VCardTitle:h.EB,VIcon:_.Z,VImg:p.Z,VListItem:f.Z,VListItemAvatar:C.Z,VListItemContent:b.km,VListItemIcon:g.Z,VListItemSubtitle:b.oZ,VListItemTitle:b.V9,VSimpleTable:w.Z})}}]);
//# sourceMappingURL=524-legacy.4f3ebd82.js.map
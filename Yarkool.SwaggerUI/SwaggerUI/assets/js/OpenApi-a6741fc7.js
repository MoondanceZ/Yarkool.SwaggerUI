import{l as s,D as v,_ as b,V as A,d as j,a as D,b as C,c as I,f as R,u as x,m as u,o as U,j as E,w as i,k as c,q as M,r as l,t as m,v as P,N as S}from"./index-5309f902.js";import{C as V}from"./clipboard-4c21dffe.js";import{a as r}from"./ace-eb2d5ee9.js";import{m as L,a as k,t as B,e as N}from"./ext-language_tools-e2443b09.js";import{C as T}from"./CopyOutlined-d10300e6.js";var $={icon:{tag:"svg",attrs:{viewBox:"64 64 896 896",focusable:"false"},children:[{tag:"path",attrs:{d:"M505.7 661a8 8 0 0012.6 0l112-141.7c4.1-5.2.4-12.9-6.3-12.9h-74.1V168c0-4.4-3.6-8-8-8h-60c-4.4 0-8 3.6-8 8v338.3H400c-6.7 0-10.4 7.7-6.3 12.9l112 141.8zM878 626h-60c-4.4 0-8 3.6-8 8v154H214V634c0-4.4-3.6-8-8-8h-60c-4.4 0-8 3.6-8 8v198c0 17.7 14.3 32 32 32h684c17.7 0 32-14.3 32-32V634c0-4.4-3.6-8-8-8z"}}]},name:"download",theme:"outlined"};const q=$;function g(n){for(var e=1;e<arguments.length;e++){var t=arguments[e]!=null?Object(arguments[e]):{},a=Object.keys(t);typeof Object.getOwnPropertySymbols=="function"&&(a=a.concat(Object.getOwnPropertySymbols(t).filter(function(o){return Object.getOwnPropertyDescriptor(t,o).enumerable}))),a.forEach(function(o){J(n,o,t[o])})}return n}function J(n,e,t){return e in n?Object.defineProperty(n,e,{value:t,enumerable:!0,configurable:!0,writable:!0}):n[e]=t,n}var p=function(e,t){var a=g({},e,t.attrs);return s(v,g({},a,{icon:q}),null)};p.displayName="DownloadOutlined";p.inheritAttrs=!1;const z=p,H=""+new URL("mode-json5-368a455d.js",import.meta.url).href;r.config.setModuleUrl("ace/mode/json",L);r.config.setModuleUrl("ace/mode/json",H);r.config.setModuleUrl("ace/mode/xml",k);r.config.setModuleUrl("ace/theme/eclipse",B);r.config.setModuleUrl("ace/ext-language/tools",N);const G={name:"Document",components:{editor:A,CopyOutlined:T,DownloadOutlined:z,EditorShow:j(()=>D(()=>import("./EditorShow-25090517.js"),["./EditorShow-25090517.js","./index-5309f902.js","..\\css\\index-5be93158.css","./ace-eb2d5ee9.js","./ext-language_tools-e2443b09.js"],import.meta.url))},props:{api:{type:Object,required:!0},swaggerInstance:{type:Object,required:!0}},setup(){const n=C(),e=I(()=>n.language),{messages:t}=R();return{language:e,messages:t}},data(){return{openApiRaw:"",name:"OpenAPI.json"}},created(){this.openApiRaw=x.json5stringify(this.api.openApiRaw),this.name=this.api.summary+"_OpenAPI.json",setTimeout(()=>{this.copyOpenApi()},500)},methods:{getCurrentI18nInstance(){return this.messages[this.language]},triggerDownloadOpen(){var n=this.openApiRaw,e=document.createElement("a"),t={},a=this.name,o=window.URL.createObjectURL(new Blob([n],{type:(t.type||"text/plain")+";charset="+(t.encoding||"utf-8")}));e.href=o,e.download=a||"file",e.click(),window.URL.revokeObjectURL(o)},copyOpenApi(){const n="btnCopyOpenApi"+this.api.id,e=new V("#"+n,{text:()=>this.openApiRaw});e.on("success",()=>{const a=this.getCurrentI18nInstance().message.copy.open.success;u.info(a)}),e.on("error",t=>{const o=this.getCurrentI18nInstance().message.copy.open.fail;u.info(o)})}}},X={class:"document"},F={style:{"margin-top":"10px"},id:"knife4jDocumentOpenApiShowEditor"};function Q(n,e,t,a,o,f){const w=l("CopyOutlined"),d=S,_=l("DownloadOutlined"),h=M,O=l("editor-show");return U(),E("div",X,[s(h,{style:{"margin-top":"10px"}},{default:i(()=>[s(d,{type:"primary",id:"btnCopyOpenApi"+t.api.id},{default:i(()=>[s(w),c("span",null,m(n.$t("open.copy")),1)]),_:1},8,["id"]),s(d,{style:{"margin-left":"10px"},onClick:f.triggerDownloadOpen},{default:i(()=>[s(_),P(),c("span",null,m(n.$t("open.download")),1)]),_:1},8,["onClick"])]),_:1}),c("div",F,[s(O,{value:o.openApiRaw,"onUpdate:value":e[0]||(e[0]=y=>o.openApiRaw=y),theme:"eclipse"},null,8,["value"])])])}const te=b(G,[["render",Q]]);export{te as default};
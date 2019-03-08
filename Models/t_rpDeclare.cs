using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using netbu;

namespace netbu.Models
{
    public class Field
    {
        public string FieldName {get;set;}
        public string Title {get;set;}
        public string Vis{get;set;}
        public string Type{get;set;}
        public string GroupDec{get;set;}
        public string IdDeclare{get;set;}
        public Dictionary<string,string> LookUp {get; set;}
        public string ClassName{get;set;}
        public string Editor {get;set;}

        public string jointrue {get;set;}
    }
    public class t_rpDeclare
    {
        public int IdDeclare {get;set;}
        public string fmtSaveFieldList {get;set;}
        public DataRow rw{get;set;}

        public Dictionary<string, Field> ListField{get;set;}
        
    }
}
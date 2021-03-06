using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.ComponentModel.CollectionConverter;


namespace suggestionscsharp {



public  class Suggestion {
        public string value { get; set; }
        public string unrestricted_value { get; set; }
        public PartyData data;

        public override string ToString() {
            return value;
        }
    }

public class SuggestResponse {
        public List<Suggestion> suggestions { get; set; }
    }    


public class AddressResponse {
            public string value { get; set; }
            public string unrestricted_value { get; set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public AddressData data { get; set; }
            public override string ToString()
            {
                return this.value;
            }
    
    }

    public class AddressData {
        public string source      { get; set; }
        public string postal_code { get; set; }
        public string country     { get; set; }

        public string region_fias_id   { get; set; }
        public string region_kladr_id  { get; set; }
        public string region_with_type { get; set; }
        public string region_type      { get; set; }
        public string region_type_full { get; set; }
        public string region           { get; set; }

        public string area_fias_id   { get; set; }
        public string area_kladr_id  { get; set; }
        public string area_with_type { get; set; }
        public string area_type      { get; set; }
        public string area_type_full { get; set; }
        public string area           { get; set; }

        public string city_fias_id   { get; set; }
        public string city_kladr_id  { get; set; }
        public string city_with_type { get; set; }
        public string city_type      { get; set; }
        public string city_type_full { get; set; }
        public string city           { get; set; }

        public string city_area { get; set; }

        public string city_district_fias_id   { get; set; }
        public string city_district_kladr_id  { get; set; }
        public string city_district_with_type { get; set; }
        public string city_district_type      { get; set; }
        public string city_district_type_full { get; set; }
        public string city_district           { get; set; }

        public string settlement_fias_id   { get; set; }
        public string settlement_kladr_id  { get; set; }
        public string settlement_with_type { get; set; }
        public string settlement_type      { get; set; }
        public string settlement_type_full { get; set; }
        public string settlement           { get; set; }

        public string street_fias_id   { get; set; }
        public string street_kladr_id  { get; set; }
        public string street_with_type { get; set; }
        public string street_type      { get; set; }
        public string street_type_full { get; set; }
        public string street           { get; set; }

        public string house_fias_id   { get; set; }
        public string house_kladr_id  { get; set; }
        public string house_type      { get; set; }
        public string house_type_full { get; set; }
        public string house           { get; set; }

        public string block_type      { get; set; }
        public string block_type_full { get; set; }
        public string block           { get; set; }

        public string flat_type           { get; set; }
        public string flat_type_full      { get; set; }
        public string flat                { get; set; }
        public string flat_area           { get; set; }
        public string square_meter_price  { get; set; }
        public string flat_price          { get; set; }

        public string postal_box     { get; set; }
        public string fias_id        { get; set; }
        public string fias_level     { get; set; }
        public string kladr_id       { get; set; }
        public string capital_marker { get; set; }

        public string okato            { get; set; }
        public string oktmo            { get; set; }
        public string tax_office       { get; set; }
        public string tax_office_legal { get; set; }

        public string timezone { get; set; }

        public string geo_lat { get; set; }
        public string geo_lon { get; set; }
        public string qc_geo  { get; set; }

        public string beltway_hit      { get; set; }
        public string beltway_distance { get; set; }

        [TypeConverter(typeof(ArrayConverter))]
        public List<string> history_values { get; set; }

        [TypeConverter(typeof(ArrayConverter))]
        public List<Метро> metro { get; set; }

        public override string ToString()
        {
            return this.source;
        }
    }

    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public class Метро {
        public string  name     { get; set; }
        public string  line     { get; set; }
        public decimal distance { get; set; }

        public override string ToString()
        {
            return this.name;
        }
        
    }

    public class AddressBound {
        public string value { get; set; }
        public AddressBound(string name) {
            this.value = name;
        }
    }

    public class BankData {
        public AddressResponse address { get; set; }

        public string bic                   { get; set; }
        public string swift                 { get; set; }
        public string registration_number   { get; set; }
        public string correspondent_account { get; set; }

        public BankNameData name    { get; set; }
        public string okpo          { get; set; }
        public BankOpfData opf      { get; set; }
        public string phone         { get; set; }
        public string rkc           { get; set; }
        public PartyStateData state { get; set; }

    }

    public class BankNameData {
        public string payment   { get; set; }
        public string full      { get; set; }
        public string @short    { get; set; }
    }

    public class BankOpfData {
        public BankType type    { get; set; }
        public string full      { get; set; }
        public string @short    { get; set; }
    }

    public enum BankType {
        BANK,
        NKO,
        BANK_BRANCH,
        NKO_BRANCH,
        RKC,
        OTHER
    }

    public class EmailData {
        public string value     { get; set; }
        public string local     { get; set; }
        public string domain    { get; set; }
    }

    public class FioData {
        public string surname       { get; set; }
        public string name          { get; set; }
        public string patronymic    { get; set; }
        public string gender        { get; set; }
        public override string ToString()
        {
            return surname + " " + name + " " + patronymic;
        }
    }

    public enum FioPart {
        SURNAME,
        NAME,
        PATRONYMIC
    }
    public class CapitalData
    {
        public string type { get; set; }
        public string value { get; set; }
        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class Founder
    {
        public string inn { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FioData fio {get;set;}
        public string hid { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public override string ToString()
        {
            try
            {
                string s = name;
                if (type == "PHYSICAL")
                    s = fio.ToString();
                return s;
            }
            catch 
            {
                return "?";
            }
            
        }
    }

    public class Manager
    {
        public string inn { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FioData fio { get; set; }
        public string post { get; set; }
        public string hid { get; set; }
        public string type { get; set; }
        public override string ToString()
        {
            try
            {
                string s = fio.ToString() + ", " + post;
                return s;
            }
            catch { 
            return "?";
            }
        }
    }

    
    public class Okved { 
     public bool main { get; set; }
     public string type { get; set; }
     public string  code { get; set; }
     public string  name { get; set; }
     public override string ToString()
     {
         return name;
     }

    }

    public class PartyData {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AddressResponse address { get; set; }

        public string branch_count         { get; set; }

        
        public PartyBranchType branch_type { get; set; }

        public string inn       { get; set; }
        public string kpp       { get; set; }
        public string ogrn      { get; set; }
        [Browsable(false)]
        public string ogrn_date { get; set; }
        [DisplayName("Дата выдачи ОГРН")]
        public string ogrn_date_fmt
        {
            get {
                string res = "";
                if (!string.IsNullOrEmpty(ogrn_date))
                {
                    long d = long.Parse(ogrn_date);
                    res = (new DateTime(1970, 1, 1)).AddMilliseconds(d).ToString("dd.MM.yyyy");
                }
                return res;
            }
        }
        
        public string hid       { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CapitalData capital { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PartyManagementData management { get; set; }

        [TypeConverter(typeof(ArrayConverter))]
        public List<Founder> founders { get; set; }
        
        [TypeConverter(typeof(ArrayConverter))]
        public List<Manager> managers { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PartyNameData name             { get; set; }

        public string okpo       { get; set; }
        public string okved      { get; set; }
        [TypeConverter(typeof(ArrayConverter))]
        public List<Okved> okveds { get; set; }


        public string okved_type { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PartyOpfData opf     { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PartyStateData state { get; set; }

        
        public PartyType type       { get; set; }
        public override string ToString()
        {
            return this.inn;
        }
    }

    public enum PartyBranchType {
        MAIN,
        BRANCH
    }

    public class PartyManagementData {
        public string name { get; set; }
        public string post { get; set; }
        public override string ToString()
        {
            return this.name;
        }
    }

    public class PartyNameData {
        public string full_with_opf     { get; set; }
        public string short_with_opf    { get; set; }
        public string latin             { get; set; }
        public string full              { get; set; }
        public string @short            { get; set; }

        public override string ToString()
        {
            return this.full_with_opf;
        }

    }

    public class PartyOpfData {
        public string code      { get; set; }
        public string full      { get; set; }
        public string @short    { get; set; }

        public override string ToString()
        {
            return this.@short;
        }
    }

    public class PartyStateData {
        [Browsable(false)]
        public string actuality_date    { get; set; }
        [Browsable(false)]
        public string registration_date { get; set; }
        [Browsable(false)]
        public string liquidation_date  { get; set; }

        [DisplayName("дата актуальности сведений")]
        public string actuality_date_fmt { get {
            return this.fmtdate(actuality_date);
        } 
        }

        [DisplayName("дата регистрации")]
        public string registration_date_fmt
        {
            get
            {
                return this.fmtdate(registration_date);
            }
        }

        [DisplayName("дата ликвидации")]
        public string liquidation_date_fmt
        {
            get
            {
                return this.fmtdate(liquidation_date);
            }
        }

        public PartyStatus status       { get; set; }

        public override string ToString()
        {
            return this.status.ToString();
        }
        public string fmtdate(string ogrn_date)
        {
            string res = "";
            if (!string.IsNullOrEmpty(ogrn_date))
            {
                long d = long.Parse(ogrn_date);
                res = (new DateTime(1970, 1, 1)).AddMilliseconds(d).ToString("dd.MM.yyyy");
            }
            return res;
        }
    }

    public enum PartyStatus {
        ACTIVE,
        LIQUIDATING,
        LIQUIDATED
    }

    public enum PartyType {
        LEGAL,
        INDIVIDUAL
    }


}
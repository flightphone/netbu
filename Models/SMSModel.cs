using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using netbu;

namespace netbu.Models{
    public class SMSmodel
    {

    }

    public class sms_event
    {
        public int[] internal_errors;
        public string message_id;
        public int status;
    }

    public class events_info0
    {
        public sms_event[] events_info;
        public string key;

    }

    public class sms_info
    {
        public events_info0[] events_info;
    }
}
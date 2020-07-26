using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Controls;

namespace WpfBu.Models
{
    public class RootForm
    {
        //public MainWindow Parent { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        //public ContentControl userMenu { get; set; }
        //public ContentControl userContent { get; set; }
        public virtual void start(object o)
        { 
        }
        public override string ToString()
        {
            return text;
        }
    }
}

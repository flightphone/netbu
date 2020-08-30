using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Controls;

namespace WpfBu.Models
{
    public class RootForm
    {
        
        public string text { get; set; }
        public virtual void start(object o)
        { 
        }
        public override string ToString()
        {
            return text;
        }
    }
}

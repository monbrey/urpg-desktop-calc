using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URPGDesktopCalc
{
    public class Object
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public Object(string c, string n)
        {
            Code = c;
            Name = n;
        }
    }
}

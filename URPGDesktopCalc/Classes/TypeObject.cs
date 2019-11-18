using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URPGDesktopCalc
{
    public class TypeObject : Object
    {
        public Compat Compatibility;

        public TypeObject(string c, string n) : base(c,n)
        {
        }
    }
}
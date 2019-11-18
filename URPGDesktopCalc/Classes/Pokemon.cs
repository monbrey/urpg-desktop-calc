using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URPGDesktopCalc
{
    public class Pokemon
    {
        public string Name { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public double MaxHP { get; set; }
        public double ATT { get; set; }
        public double DEF { get; set; }
        public double SPA { get; set; }
        public double SPD { get; set; }
        public double SPE { get; set; }

        public Pokemon()
        {
        }

        public Pokemon(String[] values)
        {
            Name = values[0];

            Type1 = values[1];
            Type2 = values[2];

            MaxHP = double.Parse(values[3]);
            ATT = double.Parse(values[4]);
            DEF = double.Parse(values[5]);
            SPA = double.Parse(values[6]);
            SPD = double.Parse(values[7]);
            SPE = double.Parse(values[8]);
        }
    }
}

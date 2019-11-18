using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URPGDesktopCalc
{
    class StatMod
    {
        public double Modifier { get; set; }
        public string Name { get; set; }

        public StatMod(double mod, string n)
        {
            Modifier = mod;
            Name = n;
        }
    }
}

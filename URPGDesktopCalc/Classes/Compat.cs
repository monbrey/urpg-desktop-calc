using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URPGDesktopCalc
{
    public class Compat
    {
        public string[] SE { get; set; }
        public string[] NVE { get; set; }
        public string[] I { get; set; }

        public Compat(string[] se, string[] nve, string[] i)
        {
            SE = se;
            NVE = nve;
            I = i;
        }

        public double Match(string a, string b, string Ability)
        {
            double result = 1.0;

            if (SE != null)
            {
                if(SE.Contains(a))
                    result *= 2.0;
                if (SE.Contains(b))
                    result *= 2.0;
            }

            if (NVE != null)
            {
                foreach (string s in NVE)
                    if (s == a || s == b)
                        result *= 0.5;
            }

            if (I != null)
            {
                foreach (string s in I)
                    if (s == a || s == b)
                    {
                        if (Ability == "SCR")
                            result *= 1.0;
                        else
                            result *= 0.0;
                    }
            }

            return result;
        }
    }
}

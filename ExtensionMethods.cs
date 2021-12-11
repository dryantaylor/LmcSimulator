using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmcSimulator
{
    static class ExtensionMethods
    {
        private static readonly string[] mnemonics = new string[] { "HLT", "ADD", "SUB", "STA", "LDA", "BRA", "BRZ", "BRP", "INP", "OUT", "OTC", "DAT" };
        public static bool IsData(this string str)
        {
            // true if a ushort
            return ushort.TryParse(str, out _);
        }
        public static bool IsLabel(this string str) 
        {
            // true if not (a mnemonic or a ushort)
            return Array.IndexOf(mnemonics, str.ToUpper()) == -1 && !IsData(str);
        }

    }
}

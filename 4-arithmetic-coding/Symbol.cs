using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypt4
{
    class Symbol
    {
        public char Character { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public Symbol(char Character, decimal Low, decimal High)
        {
            this.Character = Character;
            this.Low = Low;
            this.High = High;
        }
        public bool InRange(decimal value)
        {
            if (value >= this.Low && value < this.High)
                return true;
            return false;
        }
    }
}

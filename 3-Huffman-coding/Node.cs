using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypt3
{
    class Node: IComparable<Node>
    {
        public char? Character { get; set; }
        public int Frequency { get; set; }
        public Node Left { get; set; } = null;
        public Node Right { get; set; } = null;
        public Node(char? Character, int Frequency)
        {
            this.Character = Character;
            this.Frequency = Frequency;
        }

        public int CompareTo(Node other)
        {
            return other.Frequency.CompareTo(this.Frequency);
        }
    }
}

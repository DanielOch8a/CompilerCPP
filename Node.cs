using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class Node
    {
        public string pointer = null;
        public string lexeme;
        public int token;
        public int row;
        public Node Next = null;

        public Node(string pointer,string lexeme, int token, int row)
        {
            this.lexeme = lexeme;
            this.token = token;
            this.row = row;
            this.pointer = pointer;
        }
    }
}

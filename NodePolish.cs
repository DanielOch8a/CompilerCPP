using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class NodePolish
    {
        public string lexeme;
        public int type;
        public NodePolish Next = null;

        public NodePolish() { }

        public NodePolish(string lexeme, int token)
        {
            this.lexeme = lexeme;
            this.type = token;
        }

    }
}

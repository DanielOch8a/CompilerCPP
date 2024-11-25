using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class NodeVariableList
    {
        public int type;
        public string lexeme;
        public bool isInicialized = false;
        public NodeVariableList Next = null;

        public NodeVariableList(int type,string lexeme)
        {
            this.type = type;
            this.lexeme = lexeme;
        }
    }
}

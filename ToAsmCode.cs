using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Compiler.LinkedPolishList;

namespace Compiler
{
    internal class ToAsmCode
    {
        public LinkedList<LinkedPolishList.QuadrupleNode> quadruples { get; set; }
        public NodeVariableList headV { get; set; }

        private string asmCode = "";

        public void AsmCode()
        {
            NodeVariableList current = headV;

            asmCode += ";/StartHeader\r\nINCLUDE macros.mac\r\nDOSSEG\r\n.MODEL SMALL\r\n.STACK 100h\r\n.DATA";
            while (current != null)
            {
                switch (current.type)
                {
                    case 218://int
                        asmCode = asmCode + "\r\n\t\t\t" + current.lexeme + " DWORD ?" ;
                        break;
                    case 239://string
                        asmCode = asmCode + "\r\n\t\t\t" + current.lexeme + " BYTE 64 DUP(?)";
                        break;
                    default:
                        break;
                }
                current = current.Next;
            }

            asmCode += "\r\n.CODE\r\n.386\r\nBEGIN:\t\t\tMOV     AX, @DATA\r\n\t\t\tMOV     DS, AX\r\nCALL  COMPI\r\n\t\t\tMOV AX, 4C00H\r\n\t\t\tINT 21H\r\nCOMPI  PROC";

            foreach (var item in quadruples)
            {
                
                switch (item.op)
                {
                    case "="://=
                        current = headV;
                        while (current != null)
                        {
                            if ((item.result == current.lexeme) && current.type == 218)
                            {
                                asmCode = asmCode + "\r\n\tI_ASIGNAR MACRO " + item.arg1 + ", " + item.result;
                            }
                            else if ((item.result == current.lexeme) && current.type == 239)
                            {
                                asmCode = asmCode + "\r\n\tS_ASIGNAR MACRO " + item.arg1 + ", " + item.result;
                            }
                            current = current.Next;
                        }
                        break;
                    default:
                        break;
                }
            }

                asmCode += "\r\nCOMPI  ENDP\r\nEND BEGIN";
        }

        public void PrintAsmCode()
        {
            Console.WriteLine("\n-----------STARTS ASM CODE------------");
            Console.WriteLine(asmCode);
        }
    }
}

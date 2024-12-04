using System;
using System.Collections.Generic;
using System.IO;
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
                        asmCode = asmCode + "\r\n\t\t\t" + current.lexeme + " dw ?" ;
                        break;
                    case 239://string
                        asmCode = asmCode + "\r\n\t\t\t" + current.lexeme + " db 64 DUP(?)";
                        break;
                    default:
                        break;
                }
                current = current.Next;
            }

            asmCode += "\r\n.CODE\r\n.386\r\nBEGIN:\r\n            MOV     AX, _DATA\r\n            MOV     DS, AX\r\nCALL  COMPI\r\n            MOV AX, 4C00H\r\n            INT 21H\r\nCOMPI  PROC\r\n";

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
                                asmCode += "\r\n\tI_ASIGNAR MACRO " + item.arg1 + ", " + item.result;
                            }
                            else if ((item.result == current.lexeme) && current.type == 239)
                            {
                                asmCode += "\r\n\tS_ASIGNAR MACRO " + item.arg1 + ", " + item.result;
                            }
                            current = current.Next;
                        }
                        break;
                    case "+":
                        asmCode += "\r\n\tSUMAR MACRO " + item.arg1 + ", " + item.arg2 + ", "+item.result;
                        break;
                    case "-":
                        asmCode += "\r\n\tRESTA MACRO " + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "*":
                        asmCode += "\r\n\tMULTI MACRO " + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "/":
                        asmCode += "\r\n\tDIVIDE MACRO " + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "cin":
                        asmCode += "\r\n\tREAD  MACRO " + item.result;
                        break;
                    case "cout":
                        asmCode += "\r\n\tWRITE     MACRO " + item.arg1;
                        break;
                    default:
                        break;
                }
            }

                asmCode += "\r\n\r\n\t\tret\r\n\r\nCOMPI  ENDP\r\nEND BEGIN";

            string path = @"C:\temp\pruebas.asm";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(asmCode);
                }
            }
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(asmCode);
            }
        }

        public void PrintAsmCode()
        {
            Console.WriteLine("\n-----------STARTS ASM CODE------------");
            Console.WriteLine(asmCode);
        }
    }
}

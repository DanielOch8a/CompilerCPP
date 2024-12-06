using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Compiler.LinkedPolishList;

namespace Compiler
{
    internal class ToAsmCode
    {
        public LinkedList<LinkedPolishList.QuadrupleNode> quadruples { get; set; }
        public NodeVariableList headV { get; set; }
        public NodeVariableList headTempV { get; set; }

        private string asmCode = "";
        private string temporalPointer;
        private string temporalPointer2;
        private int temporalCont=1;
        

        public void AsmCode()
        {
            NodeVariableList currentV = headV;
            NodeVariableList currentTempV = headTempV;

            asmCode += ";/StartHeader\r\nINCLUDE macrosc.mac\r\n.MODEL SMALL\r\n.STACK 100h\r\n.DATA\r\n\t\t\t\r\n\t\t\tLISTAPAR    LABEL BYTE";
            while (currentV != null)
            {
                switch (currentV.type)
                {
                    case 218://int
                        asmCode = asmCode + "\r\n\t\t\t" + currentV.lexeme + " db 0" ;
                        break;
                    case 239://string
                        asmCode = asmCode + "\r\n\t\t\t" + currentV.lexeme + " db 64 DUP(?)";
                        break;
                    default:
                        break;
                }
                currentV = currentV.Next;
            }

            while (currentTempV != null)
            {
                switch (currentTempV.type)
                {
                    case 218://int
                        asmCode = asmCode + "\r\n\t\t\t" + currentTempV.lexeme + " db 0";
                        break;
                    case 239://string
                        asmCode = asmCode + "\r\n\t\t\t" + currentTempV.lexeme + " db 64 DUP(?)";
                        break;
                    default:
                        break;
                }
                currentTempV = currentTempV.Next;
            }

            asmCode += "\r\n.CODE\r\nBEGIN:\r\n            MOV     AX, @DATA\r\n            MOV     DS, AX\r\nCALL  COMPI\r\n            MOV AX, 4C00H\r\n            INT 21H\r\nCOMPI  PROC\r\n";

            foreach (var item in quadruples)
            {
                
                switch (item.op)
                {
                    case "="://=
                        currentV = headV;
                        if (item.pointer != null)
                        {
                            temporalPointer = "\r\n" + temporalPointer2 + ":";
                            asmCode += temporalPointer;
                        }
                        while (currentV != null)
                        {
                            if ((item.result == currentV.lexeme) && currentV.type == 218)
                            {
                                asmCode += "\r\n\tI_ASIGNAR\t" + item.result + ", " + item.arg1;
                            }
                            else if ((item.result == currentV.lexeme) && currentV.type == 239)
                            {
                                asmCode += "\r\n\tS_ASIGNAR\t" + item.result + ", " + item.arg1;
                            }
                            currentV = currentV.Next;
                        }
                        break;
                    case "+":
                        asmCode += "\r\n\tSUMAR\t" + item.arg1 + ", " + item.arg2 + ", "+item.result;
                        break;
                    case "-":
                        asmCode += "\r\n\tRESTA\t" + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "*":
                        asmCode += "\r\n\tMULTI\t" + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "/":
                        asmCode += "\r\n\tDIVIDE\t" + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "cin":
                        if (item.pointer != null)
                        {
                            temporalPointer = "\r\n" + temporalPointer2 + ":";
                            if (item.pointer == "S")
                            {
                                temporalPointer += "\r\n" + item.pointer + temporalCont + ":";
                                temporalCont++;
                            }
                            asmCode += temporalPointer;
                        }
                        asmCode += "\r\n\tREAD\t" + item.result;
                        break;
                    case "cout":
                        if (item.pointer != null)
                        {
                            temporalPointer = "\r\n" + temporalPointer2 + ":";
                            if (item.pointer == "S")
                            {
                                temporalPointer += "\r\n" + item.pointer + temporalCont + ":";
                                temporalCont++;
                            }
                            asmCode += temporalPointer;
                        }
                        asmCode += "\r\n\tITOA\t" +"LISTAPAR, "+ item.arg1;
                        asmCode += "\r\n\tWRITE\t" + "LISTARPAR";
                        break;
                    case "<":
                        if (item.pointer != null)
                        {
                            temporalPointer = "\r\n" + temporalPointer2 + ":";
                            if (item.pointer == "S")
                            {
                                temporalPointer += "\r\n" + item.pointer + temporalCont + ":";
                                temporalCont++;
                            }
                            asmCode += temporalPointer;
                        }
                        asmCode += "\r\n\tI_MENOR\t" + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "<=":
                        if (item.pointer != null)
                        {
                            temporalPointer = "\r\n" + temporalPointer2 + ":";
                            if (item.pointer == "S")
                            {
                                temporalPointer += "\r\n" + item.pointer + temporalCont + ":";
                                temporalCont++;
                            }
                            asmCode += temporalPointer;
                        }
                        asmCode += "\r\n\tI_MENORIGUAL\t" + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case ">":
                        if (item.pointer != null)
                        {
                            temporalPointer = "\r\n" + temporalPointer2 + ":";
                            if (item.pointer == "S")
                            {
                                temporalPointer += "\r\n" + item.pointer + temporalCont + ":";
                                temporalCont++;
                            }
                            asmCode += temporalPointer;
                        }
                        asmCode += "\r\n\tI_MAYOR\t"  + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case ">=":
                        if (item.pointer != null)
                        {
                            temporalPointer = "\r\n" + temporalPointer2 + ":";
                            if (item.pointer == "S")
                            {
                                temporalPointer += "\r\n" + item.pointer + temporalCont + ":";
                                temporalCont++;
                            }
                            asmCode += temporalPointer;
                        }
                        asmCode += "\r\n\tI_MAYORIGUAL\t" + item.arg1 + ", " + item.arg2 + ", " + item.result;
                        break;
                    case "BRF-P":
                        asmCode += "\r\n\tJF\t" + item.arg1 + ", " + item.result;
                        temporalPointer = "\r\n" + item.result + ":";
                        break;
                    case "BRI-Q":
                        asmCode += "\r\n\tJMP \t" + item.result;
                        temporalPointer2=item.result;
                        asmCode += temporalPointer;
                        break;
                    case "BRF-T":
                        asmCode += "\r\n\tJF\t" + item.arg1 + ", " + item.result;
                        temporalPointer = "\r\n" + item.result + ":";
                        break;
                    case "BRI-S":
                        asmCode += "\r\n\tJMP \t" + item.result;
                        temporalPointer2 = item.result+temporalCont;
                        asmCode += temporalPointer;
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

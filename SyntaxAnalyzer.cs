﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class SyntaxAnalyzer
    {
        public bool syntaxError = false;
        public Node p = null;

        public NodeVariableList headV = null;
        public NodeVariableList pV = null;

        public NodeVariableList tempPNode = null;
        public NodeVariableList tempPVNode = null;

        public LinkedPolishList linkedPolishL = new LinkedPolishList();
        //public LinkedPolishList pP = null;
        int errorValue;
        int tempNumRow;
        int tempToken;

        Stack<Node> nodes = new Stack<Node>();


        public void Syntax()
        {

            if (p != null && (p.token == 218 /*int*/ || p.token == 223  /*void*/)) // Data Type
            {
                p = p.Next;
                if (p != null && p.token == 237) // Main
                {
                    p = p.Next;
                    if (p != null && p.token == 116) // Left Parenthesis (
                    {
                        p = p.Next;
                        if (p != null && p.token == 117) // Right Parenthesis )
                        {
                            p = p.Next;
                            if (p != null && p.token == 129) //Left Brace {
                            {
                                p = p.Next;
                                if (p != null && ValidateStatement()) // Block
                                {
                                    while (p != null && !syntaxError && p.token != 222)
                                    {
                                        Block();
                                    }

                                    if (p != null && p.token == 222) // Return
                                    {
                                        p = p.Next;
                                        if (p != null && p.token == 101) // Return Digit
                                        {
                                            p = p.Next;
                                            if (p != null && p.token == 120) // Semicolon ;
                                            {
                                                p = p.Next;
                                                if (p != null && p.token == 130) // Right Brace }
                                                {
                                                    p = p.Next;
                                                }
                                                else
                                                {
                                                    PrintError("right brace");
                                                }
                                            }
                                            else
                                            {
                                                PrintError("semicolon");
                                            }
                                        }
                                        else
                                        {
                                            PrintError("a digit");
                                        }
                                    }
                                    else
                                    {
                                        if (!syntaxError)
                                        {
                                            PrintError("return");
                                        }
                                    }
                                }
                                else
                                {
                                    PrintError("valid statement (read, write, declare, operations, if, while, do while)");
                                }
                            }
                            else
                            {
                                PrintError("left brace");
                            }
                        }
                        else
                        {
                            PrintError("right parenthesis");
                        }
                    }
                    else
                    {
                        PrintError("Left parenthesis");
                    }
                }
                else
                {
                    PrintError("reserved word main");
                }

                PrintVariableNodes();
                linkedPolishL.PrintPolishNodes();
            }
            else
            {
                PrintError("data type");
            }
        }

        private bool ValidateStatement()
        {
            if (p.token == 234/*cin*/ || p.token == 235/*cout*/ || p.token == 206 /*bool*/ || p.token == 212 /*double*/ || p.token == 218 /*int*/
                        || p.token == 220 /*char*/ || p.token == 229 /*float*/ || p.token == 239/*string*/ /*data types*/
                    || p.token == 100/*id*/ || p.token == 213/*if*/
                    || p.token == 227/*while*/)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Block()
        {
            if (ValidateStatement())
            {
                switch (p.token)
                {
                    case 234:
                        Console.WriteLine("|Reading started on line {0,-2} |", p.row);
                        nodes.Push(p);
                        p = p.Next;
                        Read();
                        break;
                    case 235:
                        Console.WriteLine("|Printing started on line {0,-2} |", p.row);
                        nodes.Push(p);
                        p = p.Next;
                        Write();
                        break;
                    case 218: /*int*/
                        tempPNode = pV; //checkpoint
                        tempToken = p.token;
                        p = p.Next;
                        Declare();
                        break;
                    case 220: /*char*/
                        tempPNode = pV; //checkpoint
                        tempToken = p.token;
                        p = p.Next;
                        Declare();
                        break;
                    case 206: /*bool*/
                        tempPNode = pV; //checkpoint
                        tempToken = p.token;
                        p = p.Next;
                        Declare();
                        break;
                    case 229: /*float*/
                        tempPNode = pV; //checkpoint
                        tempToken = p.token;
                        p = p.Next;
                        Declare();
                        break;
                    case 212: /*double*/
                        tempPNode = pV; //checkpoint
                        tempToken = p.token;
                        p = p.Next;
                        Declare();
                        break;
                    case 239: /*string*/
                        tempPNode = pV; //checkpoint
                        tempToken = p.token;
                        p = p.Next;
                        Declare();
                        break;
                    case 100:
                        if (IsDeclared(p.lexeme))
                        {
                            Console.WriteLine("|Variable {0,0}| initialized on line {1,-2} |", p.lexeme, p.row);
                            linkedPolishL.PushNodePolish(p.token, p.lexeme);
                            p = p.Next;
                            Initialize();
                        }
                        else
                        {
                            errorValue = 506;
                            PrintErrorMessage(p.lexeme, p.row);
                            syntaxError = true;
                        }
                        break;
                    case 213:
                        Console.WriteLine("|If statement started on line {0,-2} |", p.row);
                        p = p.Next;
                        If();
                        break;
                    case 227:
                        Console.WriteLine("|While loop started on line {0,-2} |", p.row);
                        p = p.Next;
                        While();
                        break;
                }
            }
            else
            {
                syntaxError = true;
                if (syntaxError)
                {
                    PrintError("Valid statement");
                }
            }
        }

        private void Declare()
        {
            if (p != null && p.token == 100)//id
            {
                string tempLexeme;
                tempLexeme = p.lexeme;
                tempNumRow = p.row;
                p = p.Next;
                if (p != null && (p.token == 120 || p.token == 119))//; OR ,
                {
                    if (p != null && p.token == 119)//,
                    {

                        if (!IsDeclared(tempLexeme)) //Checamos si el lexema (variable) se encuentra declarado.
                        {
                            PushNodeVariable(tempToken, tempLexeme);
                        }
                        else
                        {
                            errorValue = 507;
                            PrintErrorMessage(tempLexeme, tempNumRow);
                            syntaxError = true;
                        }
                        p = p.Next;
                        Declare();
                    }
                    else if (p != null && p.token == 120)//;
                    {
                        if (!IsDeclared(tempLexeme))
                        {
                            PushNodeVariable(tempToken, tempLexeme);
                        }
                        else
                        {
                            errorValue = 507;
                            PrintErrorMessage(tempLexeme, tempNumRow);
                            syntaxError = true;
                        }
                        p = p.Next;
                    }
                    else
                    {
                        pV = tempPNode;
                        PrintError("Semicolon");
                    }
                }
                else
                {
                    if (tempPNode == null)
                    {
                        headV = null;
                    }
                    else
                    {
                        pV = tempPNode;
                        pV.Next = null;
                    }
                    Console.WriteLine("|Comma to declare more variables or semicolon to end is expected| line {0,-2}|", tempNumRow);
                }
            }
            else
            {
                if (tempPNode == null)
                {
                    headV = null;
                }
                else
                {
                    pV = tempPNode;
                    pV.Next = null;
                }
                PrintError("Variable name");
            }
        }

        private void Read()
        {
            if (p != null && p.token == 128) // Input Stream >>
            {
                p = p.Next;
                if (p != null && p.token == 100) // Id
                {
                    //Added for incompatibility of types and U2
                    linkedPolishL.PushNodePolish(p.token, p.lexeme);
                    p = p.Next;

                    //if (p != null && p.token == 128)
                    //{
                    //    Read();
                    //}
                    if (p != null && p.token == 120)//;
                    {
                        //Added for incompatibility of types and U2
                        PopNodesToPolishList();
                        p = p.Next;
                    }
                    else
                    {
                        PrintError("Semicolon");
                    }
                }
                else
                {
                    PrintError("Variable name to read");
                }
            }
            else
            {
                PrintError("Input stream (>>)");
            }
        }

        private void Write()
        {
            if (p != null && p.token == 127) // Output Stream <<
            {
                p = p.Next;
                if (p != null && (p.token == 126 || p.token == 100 || p.token == 240)) // String or Variable or Endl
                {
                    //Added for incompatibility of types and U2
                    linkedPolishL.PushNodePolish(tempToken, p.lexeme);
                    p = p.Next;
                    if (p != null && p.token == 127)//<<
                    {
                        Write();
                    }
                    else if (p != null && p.token == 120)//;
                    {
                        //Added for incompatibility of types and U2
                        PopNodesToPolishList();
                        p = p.Next;
                    }
                    else
                    {
                        PrintError("Semicolon");
                    }
                }
                else
                {
                    PrintError("Complete string, Variable, or Endl");
                }
            }
            else
            {
                PrintError("Output stream (<<)");
            }
        }

        private void Initialize()
        {
            if (p != null && p.token == 123) // Equal sign =
            {
                nodes.Push(p);
                p = p.Next;
                Operations();
            }
            else
            {
                PrintError("equal sign to initialize variable");
            }
        }

        private void Operations()
        {
            if (p != null && (p.token == 126 /*string*/ || p.token == 101/*digit*/
                    || p.token == 102/*decimal*/ || p.token == 225/*true*/
                    || p.token == 226/*false*/ || p.token == 100))
            {
                if (IsDeclared(p.lexeme) || p.token == 126 /*string*/ || p.token == 101/*digit*/
                    || p.token == 102/*decimal*/ || p.token == 225/*true*/
                    || p.token == 226/*false*/)
                {
                    linkedPolishL.PushNodePolish(p.token, p.lexeme);
                    p = p.Next;
                    if (p != null && (p.token == 103/*+*/ || p.token == 104/*-*/
                        || p.token == 105/* * */ || p.token == 106/* / */))
                    {
                        //Added for incompatibility of types and U2
                        if ( ((p.token == 105 || p.token == 106) && (nodes.Peek().token == 106 || nodes.Peek().token == 105)) /* (* && /) */
                            || ((p.token == 103 || p.token == 104) && (nodes.Peek().token == 104 || nodes.Peek().token == 103)) /* (+ && -) */
                            || ((p.token == 103 || p.token == 104) && (nodes.Peek().token == 105 || nodes.Peek().token == 106)))
                        {
                            Node tempPNode = nodes.Pop();
                            linkedPolishL.PushNodePolish(tempPNode.token, tempPNode.lexeme);
                            nodes.Push(p);
                            p = p.Next;
                            Operations();
                        }
                        else
                        {
                            nodes.Push(p);
                            p = p.Next;
                            Operations();
                        }
                    }
                    else if (p != null && p.token == 120) //;
                    {
                        PopNodesToPolishList();

                        p = p.Next;
                    }
                    else
                    {
                        PrintError("Semicolon");
                    }
                }
                else
                {
                    errorValue = 506;
                    PrintErrorMessage(p.lexeme, p.row);
                    syntaxError = true;
                }
            }
            else
            {
                PrintError("Variable or value");
            }
        }

        private void If()
        {
            if (p != null && p.token == 116/*(*/)
            {
                p = p.Next;
                if (p != null && p.token == 100/*variable*/)
                {
                    if (IsDeclared(p.lexeme))
                    {
                        //Added for incompatibility of types and U2
                        linkedPolishL.PushNodePolish(p.token, p.lexeme);
                        p = p.Next;
                        if (p != null && (p.token == 107/*>*/ || p.token == 108/*>=*/ || p.token == 109/*<*/
                               || p.token == 110/*<=*/ || p.token == 111/*==*/ || p.token == 112/*<>*/ || p.token == 113 /*||*/
                               || p.token == 114 /*&&*/ || p.token == 115 /*!*/))
                        {
                            //Added for incompatibility of types and U2
                            nodes.Push(p);
                            p = p.Next;
                            if (p != null && (p.token == 126 /*string*/ || p.token == 101/*digit*/
                        || p.token == 102/*decimal*/ || p.token == 225/*true*/
                        || p.token == 226/*false*/ || p.token == 100/*variable*/))
                            {
                                //Added for incompatibility of types and U2
                                linkedPolishL.PushNodePolish(p.token, p.lexeme);
                                p = p.Next;
                                if (p != null && p.token == 117/*)*/)
                                {
                                    //Added for incompatibility of types and U2
                                    PopNodesToPolishList();
                                    linkedPolishL.PushNodePolish(213, "BRF-P");
                                    p = p.Next;
                                    if (p != null && p.token == 129/*{*/)
                                    {
                                        p = p.Next;
                                        if (p != null && ValidateStatement())
                                        {
                                            Block();
                                            if (p != null && p.token == 130/*}*/)
                                            {
                                                linkedPolishL.PushNodePolish(232, "BRI-Q");
                                                p = p.Next;
                                                if (p != null && p.token == 232)/*else*/
                                                {
                                                    if (p != null && p.Next.token == 129/*{*/)
                                                    {
                                                        p = p.Next;
                                                        p = p.Next;
                                                        if (p != null && ValidateStatement())
                                                        {
                                                            Block();
                                                            if (p != null && p.token == 130/*}*/)
                                                            {
                                                                Console.WriteLine("|If statement closed on line {0,-2} |", p.row);
                                                                p = p.Next;
                                                            }
                                                            else
                                                            {
                                                                if (!syntaxError)
                                                                {
                                                                    PrintError("Closing brace else");
                                                                }
                                                                syntaxError = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            PrintError("Valid statement in else");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PrintError("Opening brace else");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                PrintError("Closing brace");
                                            }
                                        }
                                        else
                                        {
                                            PrintError("Valid statement");
                                        }
                                    }
                                    else
                                    {
                                        PrintError("Opening brace");
                                    }
                                }
                                else
                                {
                                    PrintError("Closing parenthesis");
                                }
                            }
                            else
                            {
                                PrintError("Variable to compare");
                            }
                        }
                        else
                        {
                            PrintError("Relational operator");
                        }
                    }
                    else
                    {
                        errorValue = 506;
                        PrintErrorMessage(p.lexeme, p.row);
                        syntaxError = true;
                    }
                }
                else
                {
                    PrintError("Condition");
                }
            }
            else
            {
                PrintError("Opening parenthesis");
            }
        }

        private void While()
        {
            if (p != null && p.token == 116/*(*/)
            {
                p = p.Next;
                if (p != null && p.token == 100/*variable*/)
                {
                    if (IsDeclared(p.lexeme))
                    {
                        //Added for incompatibility of types and U2
                        linkedPolishL.PushNodePolish(p.token, p.lexeme);

                        p = p.Next;
                        if (p != null && (p.token == 107/*>*/ || p.token == 108/*>=*/ || p.token == 109/*<*/
                               || p.token == 110/*<=*/ || p.token == 111/*==*/ || p.token == 112/*<>*/
                               || p.token == 113 /*||*/ || p.token == 114 /*&&*/ || p.token == 115 /*!*/))
                        {
                            nodes.Push(p);
                            p = p.Next;
                            if (p != null && (p.token == 126 /*string*/ || p.token == 101/*digit*/
                        || p.token == 102/*decimal*/ || p.token == 225/*true*/
                        || p.token == 226/*false*/ || p.token == 100))
                            {
                                linkedPolishL.PushNodePolish(p.token, p.lexeme);
                                p = p.Next;
                                if (p != null && p.token == 117/*)*/)
                                {
                                    //Added for incompatibility of types and U2
                                    PopNodesToPolishList();
                                    linkedPolishL.PushNodePolish(227, "BRF-T");
                                    p = p.Next;
                                    if (p != null && p.token == 129/*{*/)
                                    {
                                        p = p.Next;
                                        if (p != null && ValidateStatement())
                                        {
                                            Block();
                                            if (!syntaxError)
                                            {
                                                if (p != null && p.token == 130/*}*/)
                                                {
                                                    p = p.Next;
                                                    linkedPolishL.PushNodePolish(227, "BRI-S");
                                                }
                                                else
                                                {
                                                    PrintError("Closing brace");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PrintError("Valid statement");
                                        }

                                    }
                                    else
                                    {
                                        PrintError("opening brace");
                                    }
                                }
                                else
                                {
                                    PrintError("closing parenthesis");
                                }
                            }
                            else
                            {
                                PrintError("variable to compare");
                            }
                        }
                        else
                        {
                            PrintError("Relational operator");
                        }
                    }
                    else
                    {
                        errorValue = 506;
                        PrintErrorMessage(p.lexeme, p.row);
                        syntaxError = true;
                    }
                }
                else
                {
                    PrintError("Condition");
                }
            }
            else
            {
                PrintError("opening parenthesis");
            }
        }

        private void PrintError(string missingValue)
        {
            syntaxError = true;
            if (p == null)
            {
                //Console.WriteLine("    Expected " + missingValue + " at the end of the document");
                Console.WriteLine("There is nothing here");
            }
            else
            {
                Console.WriteLine("|{0,0} is expected| line {1,-2}|", missingValue, p.row);
            }
        }

        string[,] errors ={
        //      0                                 1
        {"Error Variable no declarada",       "506"},
        {"Error Variable Multideclarada"  ,       "507"},
        {"Error de incompatibilidad"    ,       "508"},
    };

        private void PrintErrorMessage(string lexeme, int numLine)
        {
            for (int i = 0; i < errors.GetLength(0); i++)
            {
                if (errorValue == Convert.ToInt32(errors[i, 1]))
                {
                    Console.WriteLine("ERROR " + errorValue + ": " +
                                errors[i, 0] + " |" + lexeme + "|" + " in line " + numLine);
                    break;
                }
            }
        }


        // OPERATIONS
        /*SISTEMA DE TIPOS*/
        // COLUMNS
        //         218 (int)    229 (real)    239 (string)    220 (char)    225 (bool)
        int[,] matrizTypesPlus = {
    // +,     int,real,string,char,bool
    /*int*/   { 1,    1,   0,   0,   0 },
    /*real*/  { 1,    1,   0,   0,   0 },
    /*string*/{ 0,    0,   1,   239,   508 },
    /*char*/  { 0,    0,   239,   239,   508 },
    /*bool*/  { 0,    0,   508,   508,   508 },
        };
        int[,] matrizTypesMinus = {
/* int - */    { 218,        229,        508,          508,          508 },
/* real - */   { 229,        229,        508,          508,          508 },
/* string - */ { 508,        508,        508,          508,          508 },
/* char - */   { 508,        508,        508,          508,          508 },
/* bool - */   { 508,        508,        508,          508,          508 }, };
        int[,] matrizTypesMultiplication = {
/* int * */    { 218,        229,        508,          508,          508 },
/* real * */   { 229,        229,        508,          508,          508 },
/* string * */ { 508,        508,        508,          508,          508 },
/* char * */   { 508,        508,        508,          508,          508 },
/* bool * */   { 508,        508,        508,          508,          508 }, };
        int[,] matrizTypesDivition = {
/* int / */    { 229,        229,        508,          508,          508 },
/* real / */   { 229,        229,        508,          508,          508 },
/* string / */ { 508,        508,        508,          508,          508 },
/* char / */   { 508,        508,        508,          508,          508 },
/* bool / */   { 508,        508,        508,          508,          508 }, };

        /*SISTEMA DE RELACIONALES*/
        int[,] matrizTypesEquality = {
/* int == */    { 225,        508,        508,          508,          508 },
/* real == */   { 508,        225,        508,          508,          508 },
/* string == */ { 508,        508,        225,          225,          508 },
/* char == */   { 508,        508,        225,          225,          508 },
/* bool == */   { 508,        508,        508,          508,          225 }, };
        int[,] matrizTypesInequality = {
/* int != */    { 225,        508,        508,          508,          508 },
/* real != */   { 508,        225,        508,          508,          508 },
/* string != */ { 508,        508,        225,          225,          508 },
/* char != */   { 508,        508,        225,          225,          508 },
/* bool != */   { 508,        508,        508,          508,          225 }, };
        int[,] matrizTypesGreaterThan = {
/* int < */    { 225,        225,        508,          508,          508 },
/* real < */   { 225,        225,        508,          508,          508 },
/* string < */ { 508,        508,        508,          508,          508 },
/* char < */   { 508,        508,        508,          508,          508 },
/* bool < */   { 508,        508,        508,          508,          508 }, };
        int[,] matrizTypesLessThan = {
/* int > */    { 225,        225,        508,          508,          508 },
/* real > */   { 225,        225,        508,          508,          508 },
/* string > */ { 508,        508,        508,          508,          508 },
/* char > */   { 508,        508,        508,          508,          508 },
/* bool > */   { 508,        508,        508,          508,          508 }, };
        int[,] matrizTypesAsignation = {
/* int = */    { 218,        508,        508,          508,          508 },
/* real = */   { 229,        229,        508,          508,          508 },
/* string = */ { 508,        508,        239,          239,          508 },
/* char = */   { 508,        508,        508,          220,          508 },
/* bool = */   { 508,        508,        508,          508,          225 }, };

        /*OPERADORES LOGICOS*/
        int[,] matrizTypesAND = {
/* int && */   { 508,        508,        508,          508,          508 },
/* real && */  { 508,        508,        508,          508,          508 },
/* string && */{ 508,        508,        508,          508,          508 },
/* char && */  { 508,        508,        508,          508,          508 },
/* bool && */  { 508,        508,        508,          508,          225 }, };
        int[,] matrizTypesOR = {
/* int  ||*/   { 508,        508,        508,          508,          508 },
/* real  ||*/  { 508,        508,        508,          508,          508 },
/* string || */{ 508,        508,        508,          508,          508 },
/* char || */  { 508,        508,        508,          508,          508 },
/* bool || */  { 508,        508,        508,          508,          225 }, };
        int[,] matrizTypesNOT = {
/* int ! */    { 508,        508,        508,          508,          508 },
/* real ! */   { 508,        508,        508,          508,          508 },
/* string ! */ { 508,        508,        508,          508,          508 },
/* char ! */   { 508,        508,        508,          508,          508 },
/* bool ! */   { 508,        508,        508,          508,          225 }, };

        public void PushNodeVariable(int type, string lexeme)
        {
            NodeVariableList node = new NodeVariableList(type, lexeme);
            if (headV == null)
            {
                headV = node;
                pV = headV;
            }
            else
            {
                pV.Next = node;
                pV = node;
            }
        }
        //public void PushNodePolish(int type, string lexeme)
        //{
        //    LinkedPolishList node = new LinkedPolishList(lexeme, type);
        //    if (headP == null)
        //    {
        //        headP = node;
        //        pP = headP;
        //    }
        //    else
        //    {
        //        pP.Next = node;
        //        pP = node;
        //    }
        //}
        public void PrintVariableNodes()
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Variables list");
            Console.WriteLine("-----------------------------------------");
            pV = headV;
            while (pV != null)
            {
                Console.WriteLine("|Type {0,0}| Lexeme {1,0}|", pV.type, pV.lexeme);
                pV = pV.Next;
            }
        }
        //added for semantic
        public bool IsDeclared(string lexeme)
        {
            tempPNode = headV;

            while (tempPNode != null)
            {
                if (lexeme == tempPNode.lexeme)
                {
                    return true;
                }
                tempPNode = tempPNode.Next;
            }
            return false;
        }

        //Added for incompatibility of types and U3
        public void PopNodesToPolishList()
        {
            Node tempNode;

            while (nodes.Count > 0)
            {
                tempNode = nodes.Pop();
                linkedPolishL.PushNodePolish(tempNode.token, tempNode.lexeme);
            }
        }
        //Added for incompatibility of types and U2
        //public void PrintPolishNodes()
        //{
        //    Console.WriteLine("-----------------------------------------");
        //    Console.WriteLine("Polish list");
        //    Console.WriteLine("-----------------------------------------");
        //    pP = headP;
        //    while (pP != null)
        //    {
        //        Console.WriteLine("|Type {0,0}| Lexeme {1,0}|", pP.type, pP.lexeme);
        //        pP = pP.Next;
        //    }
        //}
        //Added for incompatibility of types
    }
}
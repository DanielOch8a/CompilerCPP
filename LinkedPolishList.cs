using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class LinkedPolishList
    {
        private Node head;
        private Node tail;

        /*Basic Operations System*/
        private int[,] matrizTypesAsignation = {
        //         0  1  2       3    4
        // +,   int real string char bool
/*0 int = */    { 1,  0,  0,      0,  0 },
/*1 real = */   { 1,  1,  0,      0,  0 },
/*2 string = */ { 0,  0,  1,      1,  0 },
/*3 char = */   { 0,  0,  0,      1,  0 },
/*4 bool = */   { 0,  0,  0,      0,  1 }, };
        private int[,] matrizTypesPlus = {
    // +,     int,real,string,char,bool
    /*int*/   { 1,    1,   0,   0,   0 },
    /*real*/  { 1,    1,   0,   0,   0 },
    /*string*/{ 0,    0,   1,   1,   0 },
    /*char*/  { 0,    0,   1,   1,   0 },
    /*bool*/  { 0,    0,   0,   0,   0 },
        };
        private int[,] matrizTypesMinus = {
       // -,    int  real string char,bool
/* int - */    { 1,   1,   0,     0,   0 },
/* real - */   { 1,   1,   0,     0,   0 },
/* string - */ { 0,   0,   0,     0,   0 },
/* char - */   { 0,   0,   0,     0,   0 },
/* bool - */   { 0,   0,   0,     0,   0 }, };
        private int[,] matrizTypesMultiplication = {
/* int * */    { 1,  1,  0,  0,  0 },
/* real * */   { 1,  1,  0,  0,  0 },
/* string * */ { 0,  0,  0,  0,  0 },
/* char * */   { 0,  0,  0,  0,  0 },
/* bool * */   { 0,  0,  0,  0,  0 }, };
        private int[,] matrizTypesDivition = {
/* int / */    { 1,  1,  0,  0,  0 },
/* real / */   { 1,  1,  0,  0,  0 },
/* string / */ { 0,  0,  0,  0,  0 },
/* char / */   { 0,  0,  0,  0,  0 },
/* bool / */   { 0,  0,  0,  0,  0 }, };

        /*Relacionals System*/
        int[,] matrizTypesEquality = {
/* int == */    { 1,  0,  0,  0,  0 },
/* real == */   { 0,  1,  0,  0,  0 },
/* string == */ { 0,  0,  1,  1,  0 },
/* char == */   { 0,  0,  1,  1,  0 },
/* bool == */   { 0,  0,  0,  0,  1 }, };
        int[,] matrizTypesInequality = {
/* int <> */    { 1,  0,   0,  0,  0},
/* real <> */   { 0,  1,   0,  0,  0},
/* string <> */ { 0,  0,   1,  1,  0},
/* char <> */   { 0,  0,   1,  1,  0},
/* bool <> */   { 0,  0,   0,  0,  1}, };
        int[,] matrizTypesGreaterThan = {
/* int < */     { 1,  1,  0,  0,  0 },
/* real < */    { 1,  1,  0,  0,  0 },
/* string < */  { 0,  0,  0,  0,  0 },
/* char < */    { 0,  0,  0,  0,  0 },
/* bool < */    { 0,  0,  0,  0,  0 }, };
        int[,] matrizTypesLessThan = {
/* int > */     { 1,  1,  0,  0,  0 },
/* real > */    { 1,  1,  0,  0,  0 },
/* string > */  { 0,  0,  0,  0,  0 },
/* char > */    { 0,  0,  0,  0,  0 },
/* bool > */    { 0,  0,  0,  0,  0 }, };


        public class Node
        {
            public string lexeme { get; set; }
            public int type { get; set; }
            public Node Next { get; set; }

            public Node(string lexeme, int type)
            {
                this.lexeme = lexeme;
                this.type = type;
            }
        }

        public class NodeTemp
        {
            public string op2 { get; set; }
            public string op1 { get; set; }

            public int errorValue { get; set; }

            public NodeTemp()
            {

            }
        }

        public void PushNodePolish(int type, string lexeme)
        {
            Node newNode = new Node(lexeme, type);
            if (head == null)
            {
                head = newNode;
                tail = head;
            }
            else
            {
                tail.Next = newNode;
                tail = newNode;
            }
        }

        public void PrintPolishNodes()
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Polish list");
            Console.WriteLine("-----------------------------------------");
            Node current = head;
            while (current != null)
            {
                Console.WriteLine("|Type {0,0}| Lexeme {1,0}|", current.type, current.lexeme);
                current = current.Next;
            }
        }

        public void TypesAsignation(NodeVariableList variableList)
        {
            Node current = head;
            NodeVariableList currentVariableList = variableList;
            while (current != null)
            {
                currentVariableList = variableList;
                while (currentVariableList != null)
                {
                    if(current.lexeme == currentVariableList.lexeme)
                    {
                        current.type = currentVariableList.type;

                    }
                    currentVariableList = currentVariableList.Next;
                }
                
                current = current.Next;
            }
        }

        public NodeTemp CheckCompatibility()
        {
            Stack<Node> stack = new Stack<Node>();
            Node current = head;
            Node tempOp2,tempOp1, temp;
            NodeTemp returnnode = new NodeTemp();
            int op1, op2, mtvalue;
            while(current != null)
            {
                if (current.type == 206 /*bool*/ || current.type == 212 /*double*/ || current.type == 218 /*int*/
                    || current.type == 220 /*char*/ || current.type == 229 /*float*/ || current.type == 239 /*string*/ 
                    || current.type == 101 /*Integer Digits*/ || current.type == 102 /*Real Digits*/ || current.type == 225 /*true*/ 
                    || current.type == 226 /*false*/ || current.type == 126 /*cadena*/)
                {
                    stack.Push(current);
                }
                else if(current.type == 123 /* = */ || current.type == 103 /* + */ || current.type == 104 /* - */
                    || current.type == 105 /* * */ || current.type == 106 /* / */ || current.type == 107 /* > */
                    || current.type == 108 /* >= */ || current.type == 109 /*<*/ || current.type == 110 /*<=*/
                    || current.type == 111 /*==*/ || current.type == 112 /*==*/)
                {
                    tempOp2 = stack.Pop();
                    tempOp1 = stack.Pop();
                    op2 = tempOp2.type;
                    op1 = tempOp1.type;
                    if (op2 == 218 || op2 == 101) /*int, integer*/
                    {
                        op2 = 0;
                    }else if (op2 == 212 || op2 == 229 || op2==102)/*double, float, real*/
                    {
                        op2 = 1;
                    }
                    else if (op2 == 239 || op2 == 126)/*string, cadena*/
                    {
                        op2 = 2;
                    }
                    else if (op2 == 220) /*char*/
                    {
                        op2 = 3;
                    }
                    else if (op2 == 206 /*bool*/ || op2 == 225 /*true*/ || op2 == 226 /*false*/)
                    {
                        op2 = 4;
                    }

                    if (op1 == 218 || op1 == 101) /*int, integer*/
                    {
                        op1 = 0;
                    }
                    else if (op1 == 212 || op1 == 229 || op1 == 102)/*double, float, real*/
                    {
                        op1 = 1;
                    }
                    else if (op1 == 239 || op1 == 126)/*string, cadena*/
                    {
                        op1 = 2;
                    }
                    else if (op1 == 220) /*char*/
                    {
                        op1 = 3;
                    }
                    else if (op1 == 206 /*bool*/ || op1 == 225 /*true*/ || op1 == 226 /*false*/)
                    {
                        op1 = 4;
                    }

                    switch (current.type)
                    {
                        case 123:/* = */
                            mtvalue = matrizTypesAsignation[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            break;
                        case 103:/* + */
                            mtvalue = matrizTypesPlus[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            temp = tempOp2;
                            stack.Push(temp);
                            break ;
                        case 104:/* - */
                            mtvalue = matrizTypesMinus[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            temp = tempOp2;
                            stack.Push(temp);
                            break;
                        case 105:/* * */
                            mtvalue = matrizTypesMultiplication[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            temp = tempOp2;
                            stack.Push(temp);
                            break;
                        case 106:/* / */
                            mtvalue = matrizTypesDivition[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            temp = tempOp2;
                            stack.Push(temp);
                            break;
                        case 107:/* > */
                            mtvalue = matrizTypesGreaterThan[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            break;
                        case 108:/* >= */
                            mtvalue = matrizTypesGreaterThan[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            break;
                        case 109:/* < */
                            mtvalue = matrizTypesLessThan[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            break;
                        case 110:/* <= */
                            mtvalue = matrizTypesLessThan[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            break;
                        case 111:/* == */
                            mtvalue = matrizTypesEquality[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            break;
                        case 112:/* <> */
                            mtvalue = matrizTypesInequality[op1, op2];
                            if (mtvalue == 0)
                            {
                                returnnode.op2 = tempOp2.lexeme;
                                returnnode.op1 = tempOp1.lexeme;
                                returnnode.errorValue = 508;
                                return returnnode;
                            }
                            break;
                        default:
                            returnnode.errorValue = 509;
                            return returnnode;
                    }
                }
                    current = current.Next;
            }
            returnnode.errorValue = 0;
            return returnnode;
        }

        public class QuadrupleNode
        {

            public string pointer { get; set; }
            public string op { get; set; }
            public string arg1 { get; set; }
            public string arg2 { get; set; }
            public string result { get; set; }

            public QuadrupleNode(string pointer, string op, string arg1, string arg2, string result)
            {
                this.pointer = pointer;
                this.op = op;
                this.arg1 = arg1;
                this.arg2 = arg2;
                this.result = result;
            }
        }

        public LinkedList<QuadrupleNode> quadrupleNodes = new LinkedList<QuadrupleNode>();

        public void ConvertPolishToQuadruple()
        {
            Node current = head;
            Node tempNode;
            Stack<Node> nodesStack = new Stack<Node>();
            string pointer = "null";
            string op;
            string arg1;
            string arg2;
            string result;

            string temp = "temp";
            int numTemp = 1;
            while (current != null)
            {
                if(current != null && (current.type == 126 /*string*/ || current.type == 101/*digit*/
                        || current.type == 102/*decimal*/ || current.type == 225/*true*/
                        || current.type == 226/*false*/ || current.type == 100/*variable*/ || current.type == 206 /*bool*/ || current.type == 212 /*double*/ || current.type == 218 /*int*/
                        || current.type == 220 /*char*/ || current.type == 229 /*float*/ || current.type == 239/*string*/ /*data types*/))
                {
                    nodesStack.Push(current);
                }else if ((current != null && current.type == 123/* = */))
                {
                    arg2 = nodesStack.Pop().lexeme;
                    result = nodesStack.Pop().lexeme;

                    op = current.lexeme;
                    QuadrupleNode quadrupleNode = new QuadrupleNode(pointer, op, arg2, " ", result);
                    quadrupleNodes.AddLast(quadrupleNode);
                }
                else if (current != null && (current.type == 103/*+*/ || current.type == 104/*-*/
                        || current.type == 105/* * */ || current.type == 106/* / */ || current.type == 107/*>*/ || current.type == 108/*>=*/ || current.type == 109/*<*/
                               || current.type == 110/*<=*/ || current.type == 111/*==*/ || current.type == 112/*<>*/ || current.type == 113 /*||*/
                               || current.type == 114 /*&&*/ || current.type == 115 /*!*/))
                {
                    arg2 = nodesStack.Pop().lexeme;
                    arg1 = nodesStack.Pop().lexeme;

                    tempNode = new Node(temp + numTemp,100);
                    nodesStack.Push(tempNode);
                    op = current.lexeme;
                    QuadrupleNode quadrupleNode = new QuadrupleNode(pointer, op, arg1, arg2, temp + numTemp);
                    numTemp++;
                    quadrupleNodes.AddLast(quadrupleNode);
                }
                else if (current != null && (current.type == 234))//cin
                {
                    result = nodesStack.Pop().lexeme;

                    op = current.lexeme;
                    QuadrupleNode quadrupleNode = new QuadrupleNode(pointer, op, " ", " ", result);
                    quadrupleNodes.AddLast(quadrupleNode);
                }
                else if (current != null && (current.type == 235))//cout
                {
                    arg1 = nodesStack.Pop().lexeme;

                    op = current.lexeme;
                    QuadrupleNode quadrupleNode = new QuadrupleNode(pointer, op, arg1, " ", " ");
                    quadrupleNodes.AddLast(quadrupleNode);
                }
                else if (current != null && (current.lexeme == "BRF-P" || current.lexeme == "BRF-T"))//BRF
                {
                    arg1 = nodesStack.Pop().lexeme;
                    result = current.lexeme.Substring(current.lexeme.Length - 1);

                    op = current.lexeme;
                    QuadrupleNode quadrupleNode = new QuadrupleNode(pointer, op, arg1, " ", result);
                    quadrupleNodes.AddLast(quadrupleNode);
                }
                else if (current != null && (current.lexeme == "BRI-Q" || current.lexeme == "BRI-S"))//BRI
                {
                    result = current.lexeme.Substring(current.lexeme.Length - 1);

                    op = current.lexeme;
                    QuadrupleNode quadrupleNode = new QuadrupleNode(pointer, op, " ", " ", result);
                    quadrupleNodes.AddLast(quadrupleNode);
                }
                current = current.Next;
            }
        }

        public void PrintQuadruple()
        {
            Console.WriteLine("\n+---------------+Quadruples+---------------+");
            Console.WriteLine("+---------+----+---------+---------+---------+");
            Console.WriteLine("{0,-10} {1,-5} {2,-10} {3,-10} {4,-10}", "Pointer", "Op", "Op1", "Op2", "Result");
            Console.WriteLine("+---------+----+---------+---------+---------+");
            foreach (var item in quadrupleNodes)
            {
                Console.WriteLine("{0,-10} {1,-5} {2,-10} {3,-10} {4,-10}", item.pointer, item.op, item.arg1, item.arg2, item.result);
            }
        }
    }
}

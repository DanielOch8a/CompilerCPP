using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Compiler
{
    internal class Lexical
    {
        public Node Head = null;
        Node p;
        int state = 0;
        int column;
        int mtValue;
        int numrow = 1;
        int caracter = 0;
        string lexemeBuilding = "";
        public bool foundedError = false;

        string fileUbication = ".\\codigo1_erroresBasicos.txt";

        int[,] matriz = {
            // COLUMNS
        //    l    d    +    -    *    >    <    =    |    &    !    /    (    )    .    ,    ;    "   eb  tab  eol  eof  oc    {     }
//STATES /    0    1    2    3    4    5    6    7    8    9   10   11   12   13   14   15   16   17   18   19   20   21   22   23   24
/*0*/   {     1,   2, 103, 104, 105,   5,   6,   7,   8,   9, 115,  10, 116, 117, 118, 119, 120,  14,   0,   0,   0,   0, 505,  129, 130},
/*1*/   {     1,   1, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  100, 100},
/*2*/   {   101,   2, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101,   3, 101, 101, 101, 101, 101, 101, 101, 101,  101, 101},  
/*3*/   {   500,   4, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500,  500, 500}, 
/*4*/   {   102,   4, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102,  102, 102},
/*5*/   {   107, 107, 107, 107, 107, 128, 107, 108, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107},
/*6*/   {   109, 109, 109, 109, 109, 112, 127, 110, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109},
/*7*/   {   123, 123, 123, 123, 123, 123, 123, 111, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123, 123},
/*8*/   {   501, 501, 501, 501, 501, 501, 501, 501, 113, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501},
/*9*/   {   502, 502, 502, 502, 502, 502, 502, 502, 502, 114, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502},
/*10*/  {   106, 106, 106, 106,  11, 106, 106, 106, 106, 106, 106,  13, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106},
/*11*/  {    11,  11,  11,  11,  12,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11, 503,  11, 11, 11},
/*12*/  {    11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,   0,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11,  11, 11, 11},
/*13*/  {    13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,  13,   0,  13,  13, 13, 13},
/*14*/  {    14,  14,  14,  14,  14,  14,  14,  14,  14,  14,  14,  14,  14,  14,  14,  14,  14, 126,  14,  14,  504, 504, 14, 14, 14}
    };

        string[,] keywords ={
        //      0               1
        {"asm",          "200"},
        {"default",      "201"},
        {"for",          "202"},
        {"new",          "203"},
        {"delete",       "204"},
        {"static",       "205"},
        {"bool",         "206"},
        {"goto",         "208"},
        {"private",      "209"},
        {"struct",       "210"},
        {"break",        "211"},
        {"double",       "212"},
        {"if",           "213"},
        {"switch",       "214"},
        {"case",         "215"},
        {"public",       "216"},
        {"catch",        "217"},
        {"int",          "218"},
        {"class",        "219"},
        {"char",         "220"},
        {"long",         "221"},
        {"return",       "222"},
        {"void",         "223"},
        {"short",        "224"},
        {"true",         "225"},
        {"false",        "226"},
        {"while",        "227"},
        {"continue",     "228"},
        {"float",        "229"},
        {"protected",    "230"},
        {"unNextned",    "231"},
        {"else",         "232"},
        {"using",        "233"},
        {"cin",          "234"},
        {"cout",         "235"},
        {"ifelse",       "236"},
        {"main",       "237"},
        {"string",       "239"},
        {"endl",       "240"},
    };

        string[,] errors ={
        //      0                                   1
        {"Digit Expected",                 "500"},
        {"| Expected",                     "501"},
        {"& Expected",                     "502"},
        {"/ Expected",                     "503"},
        {"\" Expected",                    "504"},
        {"Symbol Not Identified",          "505"}
    };

        FileStream file = null;
        public Lexical()
        {
            //try
            //{
                file = new FileStream(fileUbication, FileMode.Open, FileAccess.Read);
                while (caracter != -1)
                {
                    caracter = file.ReadByte();
                    if (char.IsLetter((char)caracter))
                    {
                        column = 0;
                    }
                    else if (char.IsDigit((char)caracter))
                    {
                        column = 1;
                    }
                    else
                    { //Este switch identificara que simbolo se ingreso para indicar la columna en donde se manejara el futuro valorMT
                        switch ((char)caracter)
                        {
                            case '+':
                                column = 2;
                                break;
                            case '-':
                                column = 3;
                                break;
                            case '*':
                                column = 4;
                                break;
                            case '>':
                                column = 5;
                                break;
                            case '<':
                                column = 6;
                                break;
                            case '=':
                                column = 7;
                                break;
                            case '|':
                                column = 8;
                                break;
                            case '&':
                                column = 9;
                                break;
                            case '!':
                                column = 10;
                                break;
                            case '/':
                                column = 11;
                                break;
                            case '(':
                                column = 12;
                                break;
                            case ')':
                                column = 13;
                                break;
                            case '.':
                                column = 14;
                                break;
                            case ',':
                                column = 15;
                                break;
                            case ';':
                                column = 16;
                                break;
                            case '\"':
                                column = 17;
                                break;
                            case ' ':
                                column = 18; //EB
                                break;
                            case '\t':
                                column = 19; //tab
                                break;
                            case '\n':
                                column = 20; //EOL
                                numrow++;
                                break;
                            case '\r':
                                column = 20; //EOL
                                break;
                            case '\uffff':   //EOF
                                column = 21;
                                break;
                            case '{':
                                column = 23;
                                break;
                            case '}':
                                column = 24;
                                break;
                            default:
                                column = 22;
                                break;
                        }
                    }
                    mtValue = matriz[state, column];
                    if (mtValue < 100) //states
                    {
                        state = mtValue;

                        if (state == 0)
                        {
                            lexemeBuilding = "";
                        }
                        else
                        {
                            lexemeBuilding = lexemeBuilding + (char)caracter;
                        }
                    } //Si se ubica entre estos valores Next significa que es un identificador o es una palabra reservada
                    else if (mtValue >= 100 && mtValue < 500)
                    {
                        //Aquí entra para validar las palabras clave, ya que en primera instancia cada palabra que registra es un id
                        //por lo que debe comprobar si es un id solamente o en realidad una palabra reservada
                        if (mtValue == 100)
                        {
                            KeywordValidation();
                        }
                        //Este if comprueba si son palabras las que se guardaran, por lo que si son ALGUNOS simbolos no es necesario ir un caracter para atras en documento
                        if (mtValue == 100 || mtValue == 101 || mtValue == 102
                            || mtValue == 106 || mtValue == 107 || mtValue == 109 || mtValue == 123
                            || (mtValue >= 200)
                            )
                        {
                            file.Seek(file.Position - 1, SeekOrigin.Begin);
                        }
                        else
                        {
                            lexemeBuilding = lexemeBuilding + (char)caracter;
                        }
                        PushNode();
                        state = 0;
                        lexemeBuilding = "";
                    }
                    else
                    {
                        //Dependiendo del estado sera cierto error
                        //if (state == 3 || state == 8 || state == 9 || state == 11 || state == 14)
                        //{
                        //    switch (state)
                        //    {
                        //        case 3:
                        //            mtValue = 500;
                        //            break;
                        //        case 8:
                        //            mtValue = 501;
                        //            break;
                        //        case 9:
                        //            mtValue = 502;
                        //            break;
                        //        case 11:
                        //            mtValue = 503;
                        //            break;
                        //        case 14:
                        //            mtValue = 504;
                        //            break;
                        //    }
                            //if (mtValue == 504)
                            //{
                            //    numrow--;
                            //    PrintErrorMessage();
                            //    numrow++;
                            //}
                            //else
                            //{
                                PrintErrorMessage();
                            //}
                        //}
                        state = 0;
                        lexemeBuilding = "";
                    }
                }
                PrintNodes();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //finally
            //{
            //    try
            //    {
            //        if (file != null)
            //        {
            //            file.Close();
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //    }
            //}
        }
        private void PrintNodes()
        {
            p = Head;
            while (p != null)
            {
                Console.WriteLine("- Lexeme: {0,-7} | Token: {1,-3} | Line: {2,-3} |", p.lexeme, p.token, p.row);

                p = p.Next;
            }
        }
        private void KeywordValidation()
        {
            for (int i = 0; i < keywords.GetLength(0); i++)
            {
                if (lexemeBuilding == keywords[i, 0])
                {
                    mtValue = Convert.ToInt32(keywords[i, 1]);
                    break;
                }
            }
        }
        private void PrintErrorMessage()
        {
            for (int i = 0; i < errors.GetLength(0); i++)
            {
                if (mtValue == Convert.ToInt32(errors[i, 1]))
                {
                    Console.WriteLine("ERROR " + mtValue + ": " +
                                errors[i, 0] + " in line " + (numrow));
                    break;
                }
            }
            foundedError = true;
        }

        private void PushNode()
        {
            Node Node = new Node(lexemeBuilding, mtValue, numrow);
            if (Head == null)
            {
                Head = Node;
                p = Head;
            }
            else
            {
                p.Next = Node;
                p = Node;
            }
        }

    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler;

namespace Compiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Lexical lexical = new Lexical();
            if (!lexical.foundedError)
            {
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine("Lexical Analizer has done succesfully without errors");
                Console.WriteLine("-------------------------------------------------\n");
            }
            else
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("Lexical Analizer has done with some errors\n");

            }

            SyntaxAnalyzer syntatic = new SyntaxAnalyzer();
            syntatic.p = lexical.Head;
            Console.WriteLine("Starting Syntax Analyzer");
            Console.WriteLine("--------------------------------------------------");
            syntatic.Syntax();
            if (!syntatic.syntaxError)
            {
                Console.WriteLine("Syntax Analyzer Finished");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
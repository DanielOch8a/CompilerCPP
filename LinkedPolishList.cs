using System;
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
    }
}

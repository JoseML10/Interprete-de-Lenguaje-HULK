using System.Xml.Serialization;
using Project ; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project.Binding.Main
{
   internal static class Program
    {        
        static  Dictionary<string  , BoundFuncionExpression> Funciones = new Dictionary<string , BoundFuncionExpression>();
        static int contador =0;
        private static void Main()
        {

            // string input ="function fib(n)=> if(n>1) fib(n-1)+fib(n-2) else 1  ";
            // string input =" fib(n-1)+fib(n-2)   ";

            // string input="function suma(n) => if(n<=0) 0 else suma(n) + suma(n-1)";

            // AnLex jj = new AnLex(input);
            // List<Token> hola = jj.GetTokens();
            // foreach(var item in hola)
            // {
            //     Console.WriteLine($"{item.Kind} : {item.Value}");
            // }
            //  var syntaxTree = SyntaxTree.Parse(input);
            // //  PrettyPrint(syntaxTree.Root);
            //  Dictionary<string , object> variables = new Dictionary<string, object>();
            //  var binder = new Binder();
            //  var boundExpression = binder.BindExpression(syntaxTree.Root);
            //  Dictionary<string,object> scope = new Dictionary<string, object> ();
            //  var e = new Evaluator (boundExpression , variables , Funciones ,scope);
            //  var result = e.Evaluate();
            //  Console.WriteLine(result);   

        //      static void PrettyPrint(Node node, string indent = "", bool isLast = true)
        // {
        //     var marker = isLast ? "└──" : "├──";

        //     Console.Write(indent);
        //     Console.Write(marker);
        //     Console.Write(node.Kind);

        //     if (node is Token t && t.Value != null)
        //     {
        //         Console.Write(" ");
        //         Console.Write(t.Value);
        //     }

        //     Console.WriteLine();
            
        //     indent += isLast ? "   " : "│   ";

        //     var lastChild = node.GetChildren().LastOrDefault();

        //     foreach (var child in node.GetChildren())            
        //         PrettyPrint(child, indent, child == lastChild);
        // }
        
        Console.WriteLine("Bienvenido a mi intérprete. Ingresa una expresión para evaluar o escribe 'salir' para terminar.");

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input.ToLower() == "salir")
                break;

            try
            {

                var syntaxTree = SyntaxTree.Parse(input);
                Dictionary<string , object> variables = new Dictionary<string, object>();
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);
                Dictionary<string,object> scope = new Dictionary<string, object> ();
                
                var e = new Evaluator (boundExpression , variables , Funciones ,scope);
                var result = e.Evaluate();
                foreach (var itme in scope)
                {
                    Console.WriteLine($"{itme.Key} : {itme.Value}");
                }
                Console.WriteLine(result);   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al evaluar la expresión: {ex.Message} ");
            }
        }
    }

    }
}




    

                
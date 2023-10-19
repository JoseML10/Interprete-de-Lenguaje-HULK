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
        private static void Main()
        {
        
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




    

                
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

            string input ="let x=7 in x+2";

            AnLex jj = new AnLex(input);
            List<Token> tokens = jj.GetTokens();
            foreach ( var item in tokens)
            {
                Console.WriteLine($"{item.Kind } : {item.Value}");
            }

                var syntaxTree = SyntaxTree.Parse(input);
                Dictionary<string , BoundExpression> variables = new Dictionary<string, BoundExpression>();
                var binder = new Binder(variables);
                var boundExpression = binder.BindExpression(syntaxTree.Root);
                Dictionary<string,object> scope = new Dictionary<string, object> ();
                var e = new Evaluator (boundExpression , binder._variables , Funciones ,scope);
                var result = e.Evaluate();
                Console.WriteLine(result);   
            
    //     Console.WriteLine("Bienvenido a mi intérprete. Ingresa una expresión para evaluar o escribe 'salir' para terminar.");

    //     while (true)
    //     {
    //         Console.Write("> ");
    //         string input = Console.ReadLine();

    //         if (string.IsNullOrEmpty(input))
    //             continue;

    //         if (input.ToLower() == "salir")
    //             break;

    //         try
    //         {

    //             var syntaxTree = SyntaxTree.Parse(input);
    //             Dictionary<string , BoundExpression> variables = new Dictionary<string, BoundExpression>();
    //             var binder = new Binder(variables);
    //             var boundExpression = binder.BindExpression(syntaxTree.Root);
    //             Dictionary<string,object> scope = new Dictionary<string, object> ();
    //             var e = new Evaluator (boundExpression , binder._variables , Funciones ,scope);
    //             var result = e.Evaluate();
    //             Console.WriteLine(result);   
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Error al evaluar la expresión: {ex.Message} ");
    //         }
    //     }
    }

    }
}



    

                
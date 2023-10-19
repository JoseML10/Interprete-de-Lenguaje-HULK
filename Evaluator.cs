namespace Project.Binding
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name , Type type)
        {
            Name=name;
            Type=type;

        }

        public string Name {get;}
        public Type Type {get;}
    }

    class Evaluator 
 {
    private readonly  BoundExpression Root ;
    public  Dictionary<string , object> Variables;
    public readonly Dictionary<string, BoundFuncionExpression> Funciones;

    public  Dictionary<string, object> FunctionScope;
    public Evaluator ( BoundExpression root , Dictionary<string , object> _variables , Dictionary<string,BoundFuncionExpression> funciones ,   Dictionary<string, object> functionScope)
    {
        Root=root;  
        Variables=_variables; 
        Funciones = funciones ;
        FunctionScope=functionScope;
    }

    public object  Evaluate()
    {

        return EvaluateExpression (Root);
    }

    private object  EvaluateExpression (BoundExpression root )
    {

      if(root is BoundAssignmentExpression j )
      {
        var left = EvaluateExpression(j.Expresion);
        Variables.Add(j.Name,left);
        return null;
      }

      if(root is BoundInExpression t)
      {
        foreach(var item in t.Variables)
        {
          var left = EvaluateExpression(item);
        }
        
        var right =EvaluateExpression(t._Expression);
        return right;

      }
      
     
       if(root is BoundBinaryExpression b )
       {
          
          dynamic left =  EvaluateExpression ( b.Left);
          dynamic right = EvaluateExpression (b.Right);
 
          switch (b.Op.Kind)
          {

            case  BoundBinaryOperatorKind.Adition :
            return  left +  right ; 
            case BoundBinaryOperatorKind.Substraction :
            return  left - right ;
            case BoundBinaryOperatorKind.Multiplication :
            return  left * right;
            case BoundBinaryOperatorKind.Division :
            return  left /  right ;
            case BoundBinaryOperatorKind.OLogico :
            return left || (bool)right;
            case BoundBinaryOperatorKind.YLogico :
            return left &&  right ;
            case BoundBinaryOperatorKind.Igual :
            return Equals(left,right) ;
            case BoundBinaryOperatorKind.Distinto :
            return !Equals(left,right);
            case BoundBinaryOperatorKind.ComparacionMayor :
            return left>right;
            case BoundBinaryOperatorKind.ComparacionMayorIgual :
            return left>right||Equals(left,right);
            case BoundBinaryOperatorKind.ComparacionMenor :
            return left<right;
            case BoundBinaryOperatorKind.ComparacionMenorIgual :
            return left<right || Equals(left,right);
            case BoundBinaryOperatorKind.OperadorPotencia :
            return Math.Pow((double) left, (double) right);
            case BoundBinaryOperatorKind.RestoDivision:
            return left%right;
            case BoundBinaryOperatorKind.Concatenar:
            return left+right;
            

            default : 
            throw new Exception ($"Unexpected Binary Operator : {b.Op} ");
          }
          
       }

        if(root is BoundLiteralExpression n )
        {
        return  n.Value;
        }

        if(root is BoundMathExpression d)
        {

          dynamic expression = EvaluateExpression(d.Expression);

          switch(d.Identifier)
          {
             case "sen":
             return Math.Sin((double)expression);
             case "cos":
             return Math.Cos((double)expression);
             case "tan":
             return Math.Tan((double)expression);
             case "cot":
             return Math.Cos((double)expression)/Math.Sin((double)expression);
             
          }
          
        }

       if(root is BoundIfExpression a)
       {
          var condicion = (bool)EvaluateExpression(a.Condicion);
          if(condicion)
          {
            return EvaluateExpression(a.ThenEx);
          }
          else
          { 
            return EvaluateExpression(a.ElseCond);
          }
       }

       if(root is BoundCadenaExpression f)
       {
         
         return f.Cadena;
          
       }

       if(root is BoundPrintExpression g)
       {

        var expression = EvaluateExpression(g.Expression);
        return expression;
       }

       if(root is BoundFuncionExpression w)
       {
           Funciones.Add(w.Name.Value , w);
           return null;
           
       }

 if(root is BoundCallFuncionExpression z)
{        
    
    var oldFunctionScope = FunctionScope;

    
    FunctionScope = new Dictionary<string, object>(FunctionScope);

    for (int i = 0; i < z.Parametros.Count; i++)
    {
        if(FunctionScope.ContainsKey(Funciones[z.Name.Value].Parametros[i].Value))
        {
            FunctionScope[Funciones[z.Name.Value].Parametros[i].Value] = EvaluateExpression(z.Parametros[i]);
        }
        else
        {
            FunctionScope.Add(Funciones[z.Name.Value].Parametros[i].Value , EvaluateExpression(z.Parametros[i]));
        }
    }

   
    var result = EvaluateExpression(Funciones[z.Name.Value].Body);

    
    FunctionScope = oldFunctionScope;

    return result;
}

       if(root is BoundUnaryExpression u)
       {
          var operand =  EvaluateExpression(u.Operand);

          switch (u.Op.Kind)
          {
          case BoundUnaryOperatorKind.Identity :
          return (int ) operand ;
          case BoundUnaryOperatorKind.Negation :
          return -(int ) operand ;
          case BoundUnaryOperatorKind.NegacionLogica :
          return !(bool) operand;
          default : 
          throw new Exception ($"Unexpected Unary Operator : {u.Op.Kind}");
          }
          

       }
       
       if(root is BoundVariableExpression m)
       {

         if(m.Name=="PI")
         {
          return (decimal)Math.PI;
         }
         else if(Variables.ContainsKey(m.Name))
         {
           var result = Variables[m.Name];
           return result;
         }
        else 
        {
          return FunctionScope[m.Name];
        }
       
       }
      
       throw new Exception ($"Unexpected node : {root.Kind} ");


 }

 

}
}
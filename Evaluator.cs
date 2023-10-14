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
    public  Dictionary<string , BoundExpression> Variables;
    public readonly Dictionary<string, BoundFuncionExpression> Funciones;

    public  Dictionary<string, object> FunctionScope;
    public Evaluator ( BoundExpression root , Dictionary<string , BoundExpression> _variables , Dictionary<string,BoundFuncionExpression> funciones ,   Dictionary<string, object> functionScope)
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
        return left;
      }

      if(root is BoundInExpression t)
      {
        var left = EvaluateExpression(t.Variable.Expresion);
        var right =EvaluateExpression(t._Expression);
        return right;

      }
      
     
       if(root is BoundBinaryExpression b )
       {
          
          var left =  EvaluateExpression ( b.Left);
          var right =  EvaluateExpression (b.Right);
           
          switch (b.Op.Kind)
          {

            case  BoundBinaryOperatorKind.Adition :
            return (int) left + (int) right ; 
            case BoundBinaryOperatorKind.Substraction :
            return (int) left - (int)right ;
            case BoundBinaryOperatorKind.Multiplication :
            return (int) left* (int)right;
            case BoundBinaryOperatorKind.Division :
            return (int) left / (int) right ;
            case BoundBinaryOperatorKind.OLogico :
            return (bool) left || (bool)right;
            case BoundBinaryOperatorKind.YLogico :
            return (bool) left && (bool) right ;
            case BoundBinaryOperatorKind.Igual :
            return Equals(left,right) ;
            case BoundBinaryOperatorKind.Distinto :
            return !Equals(left,right);

            default : 
            throw new Exception ($"Unexpected Binary Operator : {b.Op} ");
          }
          
       }

        if(root is BoundLiteralExpression n )
       return  n.Value;

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
        
        for (int i = 0; i < z.Parametros.Count; i++)
        {
        FunctionScope.Add(Funciones[z.Name.Value].Parametros[i].Value , EvaluateExpression(z.Parametros[i]));
        }
                
        return EvaluateExpression(Funciones[z.Name.Value].Body);

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
         if(Variables.ContainsKey(m.Name))
         {
           var result = EvaluateExpression(Variables[m.Name]);
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
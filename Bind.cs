using Microsoft.Win32.SafeHandles;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;
using System.Xml;
using System.Security.Principal;
using System ;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.Json;
using System.Dynamic;
using System.Data;
using System.Security;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace Project.Binding
{
   internal enum BoundNodeKind 
   {
 
    Cadena ,

    PrintExpression,
    UnaryExpression,

    FuncionExpression,

    FuncionCallExpression,
    NumberExpression,

    BinaryExpression,

    VariableExpression,

    AssignmentExpression,

    IfExpression,

   }

   internal abstract class BoundNode
   {
    public abstract BoundNodeKind Kind {get;}
   } 

   internal abstract class BoundExpression : BoundNode
   {
     public abstract Type Type {get;}
     
   }

   internal enum BoundUnaryOperatorKind
   {
    Identity,
    Negation,

    NegacionLogica,
   }
    
    internal sealed class BoundCadenaExpression:BoundExpression
    {

      public BoundCadenaExpression(string cadena )
      {
        Cadena = cadena; 
      }

      public string  Cadena {get;}

      public override BoundNodeKind Kind {get{return BoundNodeKind.Cadena;}}

      public override Type Type {get{return typeof(string);}}


    }


    internal sealed class BoundPrintExpression : BoundExpression
    {
       public BoundPrintExpression(BoundExpression expression )
       {
           Expression = expression  ;

       }

       public BoundExpression Expression {get;}

       public override BoundNodeKind Kind {get{return BoundNodeKind.PrintExpression;}}

      public override Type Type {get{return Expression.Type;}}

    }
    internal sealed class BoundVariableExpression : BoundExpression
    {   
      public BoundVariableExpression(string name , Type type)
      {
         Name = name ;
         Type=type;
      }

    
       public string Name {get;}
       public override Type Type {get;}

       public override BoundNodeKind Kind 
       {
        get{return BoundNodeKind.VariableExpression;}
       }


    }
    internal sealed class BoundLiteralExpression : BoundExpression
    {

      public BoundLiteralExpression(object value)
      {
        Value=value;
      }

      public object Value {get;}
        public override BoundNodeKind Kind 
        {
          get{return BoundNodeKind.NumberExpression;}}
    

        public override Type Type
        {
          get{return Value.GetType();}
        }
    }

   internal class BoundUnaryOperator
   {
    public BoundUnaryOperator ( Token.TokenType syntaxkind , BoundUnaryOperatorKind kind , Type operandType)
    : this ( syntaxkind ,kind , operandType , operandType)
    {
    }
    

     public BoundUnaryOperator ( Token.TokenType syntaxkind , BoundUnaryOperatorKind kind , Type operandType , Type resulttype)
    {
      SyntaxKind = syntaxkind ;
      Kind=kind;
      OperandType =operandType;
      ResultType = resulttype;

    }

    private static BoundUnaryOperator[] operadores =
    {

      new BoundUnaryOperator(Token.TokenType.OperadorSuma , BoundUnaryOperatorKind.Identity , typeof(int)),
      new BoundUnaryOperator(Token.TokenType.OperadorResta , BoundUnaryOperatorKind.Negation , typeof(int)),
      new BoundUnaryOperator(Token.TokenType.Negacion, BoundUnaryOperatorKind.NegacionLogica, typeof(bool)),
    };

    public static BoundUnaryOperator Bind(Token.TokenType type , Type operandtype)
    {
      foreach ( var op in operadores )
      {
        if(op.SyntaxKind==type && op.OperandType==operandtype)
        {
          return op;
        }
      }

      return null;
    }


    public Token.TokenType SyntaxKind {get;}
    public BoundUnaryOperatorKind Kind  {get;}
    public Type OperandType {get;}
    public Type ResultType  {get;}
   }

   internal class BoundBinaryOperator
   {
  
     public BoundBinaryOperator ( Token.TokenType syntaxkind , BoundBinaryOperatorKind kind , Type type):this
     (syntaxkind , kind , type , type , type )
     {}

     public BoundBinaryOperator ( Token.TokenType syntaxkind , BoundBinaryOperatorKind kind , Type operandtype , Type resulttype):this
     (syntaxkind , kind , operandtype , operandtype ,resulttype )
     {}

     public BoundBinaryOperator ( Token.TokenType syntaxkind , BoundBinaryOperatorKind kind , Type lefttype , Type rightype , Type resulttype)
    {
      SyntaxKind = syntaxkind ;
      Kind=kind;
      LeftType = lefttype;
      RigthType =rightype;
      ResultType = resulttype;

    }

    private static BoundBinaryOperator[] operadores =
    {
      new BoundBinaryOperator(Token.TokenType.OperadorIgual, BoundBinaryOperatorKind.Igual , typeof(int) , typeof(bool)),
      new BoundBinaryOperator(Token.TokenType.OperadorSuma , BoundBinaryOperatorKind.Adition , typeof(int) ),
      new BoundBinaryOperator(Token.TokenType.OperadorResta , BoundBinaryOperatorKind.Substraction , typeof(int)),
      new BoundBinaryOperator(Token.TokenType.OperadorMult, BoundBinaryOperatorKind.Multiplication , typeof(int)),
      new BoundBinaryOperator(Token.TokenType.OperadorDiv, BoundBinaryOperatorKind.Division , typeof(int)),
      new BoundBinaryOperator(Token.TokenType.OperadorDistinto, BoundBinaryOperatorKind.Distinto , typeof(int) ,typeof(bool)),
      new BoundBinaryOperator(Token.TokenType.Disyuncion, BoundBinaryOperatorKind.OLogico, typeof(bool)),
      new BoundBinaryOperator(Token.TokenType.Conjuncion, BoundBinaryOperatorKind.YLogico , typeof(bool)),
      new BoundBinaryOperator(Token.TokenType.OperadorIgual, BoundBinaryOperatorKind.Igual, typeof(bool)),
      new BoundBinaryOperator(Token.TokenType.OperadorDistinto, BoundBinaryOperatorKind.Distinto , typeof(bool)),

    };

    public static BoundBinaryOperator Bind(Token.TokenType type , Type lefttype , Type rigthtype)
    {
    
      foreach ( var op in operadores )
      {
        if(op.SyntaxKind==type && op.LeftType==lefttype && op.RigthType==rigthtype)
        {
          return op;
        }

      }

      return null;
    }


    public Token.TokenType SyntaxKind {get;}
    public BoundBinaryOperatorKind Kind  {get;}
    public Type LeftType {get;}
    public Type RigthType {get;}

    public Type ResultType  {get;}
   }
    internal sealed class BoundUnaryExpression : BoundExpression
   {
      public BoundUnaryExpression(BoundUnaryOperator operatorKind , BoundExpression operand)
      {
        Op = operatorKind;
        Operand = operand;

      }
      
      public BoundUnaryOperator Op {get;}
      public BoundExpression Operand {get;}

      public override Type Type
      {
        get {return Op.ResultType;}

      }

        public override BoundNodeKind Kind
        {
          get{return BoundNodeKind.UnaryExpression;}
        }

    }

    internal enum BoundBinaryOperatorKind
    {
      Adition,
      Substraction,
      Multiplication,

      Division,

      Igual,

      Distinto,

      OLogico,
      YLogico,

    }

    internal sealed class BoundBinaryExpression  : BoundExpression
    {
      public BoundBinaryExpression(BoundExpression left , BoundBinaryOperator op , BoundExpression right)
       {
        Left=left;
        Op =op ;
        Right =right;
       }

      
       
       public BoundExpression Left{get;}

       public BoundBinaryOperator Op {get;}
       public BoundExpression Right {get;}

        public override BoundNodeKind Kind 
        {
          get{return BoundNodeKind.BinaryExpression;}
        }

        public override Type Type 
        {
          get{return Op.ResultType;}
        }
    }
    

internal sealed class BoundFuncionExpression : BoundExpression
{

public BoundFuncionExpression( Token name , List<Token> parametros , BoundExpression body)
{
  Name=name;
  Parametros = parametros;
  Body=body;

}

public Token Name{get;}
public List<Token> Parametros {get;}
public BoundExpression Body {get;}

public override BoundNodeKind Kind 
  {
    get{return BoundNodeKind.FuncionExpression;}
  }

  public override Type Type{get;}

}

internal sealed class BoundCallFuncionExpression : BoundExpression
{

public BoundCallFuncionExpression( Token name , List<BoundExpression> parametros )
{
  Name=name;
  Parametros = parametros;


}

public Token Name{get;}
public List<BoundExpression> Parametros {get;}

public override BoundNodeKind Kind 
  {
    get{return BoundNodeKind.FuncionCallExpression;}
  }

  public override Type Type{get;}

}


internal sealed class BoundIfExpression : BoundExpression 
{

  public BoundIfExpression ( BoundExpression condicion  , BoundExpression thenEx,  BoundExpression elsecond)
  {
    
    Condicion=condicion;
    ThenEx=thenEx;
    ElseCond=elsecond;

  }


  public BoundExpression Condicion {get;}

  public BoundExpression ThenEx{get;}

  public BoundExpression ElseCond{get;}


  public override BoundNodeKind Kind 
  {
    get{return BoundNodeKind.IfExpression;}
  }

  public override Type Type{get;}

}


    internal sealed class Binder 
    {
       private readonly List<string > diagnostics = new List<string>();
       public   Dictionary <string,BoundExpression > _variables ;

       public Binder (Dictionary <string , BoundExpression> variables)
       {
        _variables =variables;
       }

       public IEnumerable <string>  Diagnostics (List<string > diagnostics)
       {
        foreach ( string j in diagnostics)
        yield return j;
       }



      public BoundExpression BindExpression(Expression syntax)
      {

        switch (syntax.Kind)
        {
          case Token.TokenType.PrintExpression:
          return BindPrintExpression((PrintExpression)syntax);
          case Token.TokenType.Numero :
          return BindNumberExpression((NumberToken)syntax);
          case Token.TokenType.ExpresionUnaria:
          return BindUnaryExpression((UnaryExpression)syntax);
          case Token.TokenType.IfExpression:
          return BindIfExpression((IfExpression)syntax);
          case Token.TokenType.Cadena:
          return BindCadenaExpression((StringExpression)syntax);
          case Token.TokenType.ExpresionFuncion:
          return BindFuncionExpression((FuncionExpression)syntax);
          case Token.TokenType.LlamadaFuncion:
          return BindCallFuncionExpression((CallFuncionExpression)syntax);
          case Token.TokenType.ExpresionBinaria :
          return BindBinaryExpression((BinaryExpression)syntax);
          case Token.TokenType.ExpresionParentesis :
          return BindParentesisExpression((ExpresionParentesis)syntax);
          case Token.TokenType.AssignmentExpression :
          return BindAssignmentExpression((AssignmentExpression)syntax);
          case Token.TokenType.NameExpression :
          return BindNameExpression((NameExpression)syntax);
          case Token.TokenType.InExpression :
          return BindInExpression((InExpression)syntax);


          default : throw new Exception ($"Unexpected syntax {syntax.Kind}");

        }
      }

      private BoundExpression BindNumberExpression (NumberToken syntax)
      {
        var value = syntax.Value;
        return new BoundLiteralExpression(value);
      }

      private BoundExpression BindUnaryExpression ( UnaryExpression syntax)
      {
        var boundOperand = BindExpression(syntax.Operando);
        var boundOperator = BoundUnaryOperator.Bind(syntax.Operador.Kind , boundOperand.Type);

        if(boundOperator ==null)
        {
          diagnostics.Add($"Unary Operator {syntax.Operador } is not definied for type {boundOperand.Type}");
          return boundOperand ;
        }
        return new BoundUnaryExpression ( boundOperator  , boundOperand);
      }


      private BoundExpression BindCadenaExpression(StringExpression syntax)
      {
         string  cadena = syntax.Value;
         ;

         return new BoundCadenaExpression(cadena);

      }

      private BoundExpression BindPrintExpression(PrintExpression syntax)
      {
         var expression = BindExpression(syntax.Expresion);

         return new BoundPrintExpression(expression);

      }

      private BoundExpression BindBinaryExpression (BinaryExpression syntax)
      {
        var boundleft=BindExpression(syntax.Left);
        var boundRight = BindExpression (syntax.Right);

        if(boundleft.Kind==BoundNodeKind.VariableExpression || boundRight.Kind==BoundNodeKind.VariableExpression)
        {
          var boundOperator=BoundBinaryOperator.Bind(syntax.Operador.Kind , typeof(int) , typeof(int) );
          return new BoundBinaryExpression (boundleft , boundOperator, boundRight);
        }
        else
        {
         
         var boundOperator = BoundBinaryOperator.Bind(syntax.Operador.Kind , boundleft.Type , boundRight.Type);
         return new BoundBinaryExpression (boundleft , boundOperator, boundRight);
        }
       
 
      }

      private BoundExpression BindParentesisExpression (ExpresionParentesis syntax)
      {
        
        return BindExpression (syntax.Expresion);

      }

      private BoundExpression BindNameExpression (NameExpression syntax)
      {

        var name = syntax.Identifier.Value;

        var type = GetType();
        return new BoundVariableExpression (name , type );

        
      }


      private BoundExpression BindFuncionExpression(FuncionExpression syntax)
      {
         var nombre = syntax.Name;
         List<Token> parametros = syntax.Parameters;
         var body = BindExpression(syntax.Body);

         return new BoundFuncionExpression(nombre,parametros,body);

      }

      private BoundExpression BindCallFuncionExpression(CallFuncionExpression syntax)
      {
         var name = syntax.Name;
         List<BoundExpression> argumentos = new List<BoundExpression>();

         foreach(var item in syntax.Argumentos)
         {
            var process = BindExpression(item);
            argumentos.Add(process);

         }

         return new BoundCallFuncionExpression(name , argumentos);



      }
      private BoundExpression BindIfExpression (IfExpression syntax)
      {

         var condicion = BindExpression(syntax.Condicion);
         var thenEx =BindExpression(syntax.ThenEx);
         var elsecond =BindExpression(syntax.ElseCond);

         return new BoundIfExpression(condicion , thenEx ,  elsecond);

      }

      private BoundExpression BindAssignmentExpression (AssignmentExpression syntax)
      {
         var name = syntax.Identifier.Value;
         var boundExpression = BindExpression(syntax.Expression);

         var defaultvalue =
         boundExpression.Type == typeof(int)
         ? (object)0
         :boundExpression.Type==typeof(bool)
         ?(object)false
         :null;

         if(defaultvalue==null)
         {
          throw new Exception ($"Unsupported variable type : {boundExpression.Type}");
         }
         
         _variables.Add(name , boundExpression);

         return  new BoundAssignmentExpression(name  , boundExpression);
        
      }

      private BoundExpression BindInExpression (InExpression syntax)
      {
         var _variable = BindAssignmentExpression(syntax.Variable);
         var boundExpression = BindExpression(syntax.Expression);

         return  new BoundInExpression((BoundAssignmentExpression)_variable , Token.TokenType.OperadorPob, boundExpression);
        
      }

      private BoundUnaryOperatorKind? BindUnaryOperatorKind(Token.TokenType kind , Type operandType  )
      {
        if(operandType==typeof(int))
         switch (kind)
        {
          case Token.TokenType.OperadorSuma :
          return BoundUnaryOperatorKind.Identity;
          case Token.TokenType.OperadorResta :
          return BoundUnaryOperatorKind.Negation;

        }

        if(operandType==typeof(bool))
        {
          switch (kind)
          {
            case Token.TokenType.Negacion :
            return BoundUnaryOperatorKind.NegacionLogica;
  
          }
        }
        return null;

       
      }

      private BoundBinaryOperatorKind? BindBinaryOperatorKind(Token.TokenType kind , Type leftType , Type righttype )
      {
  
        if(leftType == typeof(int)||righttype == typeof(int))
        {
         
            switch (kind)
        {
          case Token.TokenType.OperadorSuma :
          return BoundBinaryOperatorKind.Adition;
          case Token.TokenType.OperadorResta :
          return BoundBinaryOperatorKind.Substraction;
          case Token.TokenType.OperadorMult :
          return BoundBinaryOperatorKind.Multiplication;
          case Token.TokenType.OperadorDiv :
          return BoundBinaryOperatorKind.Division;

        }
         
        }

        if(leftType==typeof(bool) && righttype==(typeof(bool)))
        {
          switch(kind)
          {
          case Token.TokenType.Disyuncion :
          return BoundBinaryOperatorKind.YLogico;
          case Token.TokenType.Conjuncion :
          return BoundBinaryOperatorKind.OLogico;

          }
        }
        

        return null;
         
        
        
        
      }

    }
      internal sealed class BoundAssignmentExpression : BoundExpression
      {
        public BoundAssignmentExpression(string name , BoundExpression expression)
        {
          Name = name ; 
          Expresion = expression;
        }

        public string Name {get;}
        public BoundExpression Expresion{get;}

        public override BoundNodeKind Kind {get{return BoundNodeKind.AssignmentExpression;}}
        public override Type Type {get{return  Expresion.Type;}}
      }

      internal sealed class BoundInExpression : BoundExpression
      {
        public BoundInExpression(BoundAssignmentExpression variable  , Token.TokenType operadorin , BoundExpression expression)
        {
          Variable = variable;
          OperadorIn = operadorin;
          _Expression = expression;
        }

        public BoundAssignmentExpression Variable {get;}
       
        public Token.TokenType OperadorIn{get;}
        public BoundExpression _Expression{get;}

        public override BoundNodeKind Kind {get{return BoundNodeKind.AssignmentExpression;}}
        public override Type Type {get{return  _Expression.Type;}}
      }




  

}
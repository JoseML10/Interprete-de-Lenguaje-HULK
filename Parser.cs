using System.Collections.Concurrent;
using System.Threading.Tasks;
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
using Project.Binding;
using System.Collections;
using System.Transactions;

namespace Project
{


public abstract class Node 
{

public abstract Token.TokenType  Kind {get;}

public abstract IEnumerable<Node> GetChildren();


}

public abstract class Expression : Node
{
 
}

// sealed class ExpresionLiteral:Expression
// {
//   public ExpresionLiteral (Token literal ) :
//   this  (literal , literal.Value)
//   {}
  
  
//    public ExpresionLiteral (Token literal , object value )
//   {
//     Literal = literal;
//     Value = value;
//   }

//   public override Token.TokenType Kind
//   {
//     get{return Token.TokenType.Cadena;}
//   } 

//   public Token Literal {get;}
//   public object  Value {get;}

//   public override IEnumerable <Node> GetChildren()
//   {
//     yield return Literal;
//   }
// }
sealed class ExpresionParentesis : Expression
{

    public ExpresionParentesis ( Token openpar , Expression expresion , Token closepar)
    {

        OpenPar=openpar;
        Expresion = expresion ;
        ClosePar = closepar;

    }

    public override Token.TokenType Kind 
    {
       get {return Token.TokenType.ExpresionParentesis;}

    }
    
    public override IEnumerable <Node> GetChildren()
    {

        yield return OpenPar;
        yield return Expresion;
        yield return ClosePar;
    }
    public Token OpenPar {get;}
    public Token ClosePar {get;}
    public Expression Expresion {get;}


}


sealed class PrintExpression : Expression
{

    public PrintExpression ( Token printop , Token openpar ,  Expression expresion , Token closepar)
    {
        PrintOp=printop;
        OpenPar=openpar;
        Expresion = expresion ;
        ClosePar = closepar;

    }

    public override Token.TokenType Kind 
    {
       get {return Token.TokenType.PrintExpression;}

    }
    
    public override IEnumerable <Node> GetChildren()
    {
        yield return PrintOp;
        yield return OpenPar;
        yield return Expresion;
        yield return ClosePar;
    }
    public Token OpenPar {get;}
    public Token ClosePar {get;}

    public Token PrintOp {get;}
    public Expression Expresion {get;}


}


sealed class NumberToken : Expression
{

    public NumberToken(Token number)
    {
      Number = number ;
      Value = int.Parse(number.Value);
    }

    public override Token.TokenType Kind 
    {get
    {
        return Token.TokenType.Numero;
    }
    
    
    }
    public Token Number {get;}

    public int Value {get;}
    

    public override IEnumerable<Node> GetChildren()
    {
       yield return Number; 

    }
    
    
}

sealed class BinaryExpression :Expression 
{
  public BinaryExpression (Expression left , Token operador , Expression right )
{
  Left = left ;
  Operador =  operador ;
  Right = right ;
}

public Expression Left {get;}
public Token Operador {get;}
public Expression Right {get;}

public override Token.TokenType Kind {get{return Token.TokenType.ExpresionBinaria;}}

public override IEnumerable<Node> GetChildren()
{

    yield return Left;
    yield return Operador;
    yield return Right;
    
}

}


public sealed class NameExpression : Expression 
{
   public NameExpression (Token identifier)
   {
    Identifier =identifier;
   }

   public Token Identifier {get;}

   

   public override Token.TokenType Kind{get{return Token.TokenType.NameExpression;}}

   public override IEnumerable<Node> GetChildren()
   {
    yield return Identifier ;
   }

}

public sealed class AssignmentExpression : Expression 
{

  public AssignmentExpression (Token identifier , Token operadorasignacion , Expression expression)
  {
    Identifier=identifier;
    OperadorAsignacion = operadorasignacion;
    Expression = expression ;

  }

  public Token Identifier {get;}
  public Token OperadorAsignacion {get;}
  public Expression Expression {get;}

   public override Token.TokenType Kind{get{return Token.TokenType.AssignmentExpression;}}

   public override IEnumerable<Node> GetChildren()
   {
    yield return Identifier ;
    yield return OperadorAsignacion;
    yield return Expression ;
   }

}

public sealed class InExpression : Expression 
{

  public InExpression (AssignmentExpression variable , Token operador_in ,  Expression expression)
  {
    Variable=variable;
    OperadorIn = operador_in ;
    Expression = expression ;

  }

  public AssignmentExpression Variable {get;}
  public Expression Expression {get;}

  public Token OperadorIn {get;}

   public override Token.TokenType Kind{get{return Token.TokenType.InExpression;}}

   public override IEnumerable<Node> GetChildren()
   {
    yield return Variable ;
    yield return Expression ;
   }

}

public sealed class IfExpression : Expression 
{

  public IfExpression (Token operadorif , Expression condicion , Expression thenEx ,  Token operadorelse , Expression elsecond)
  {
    OperadorIf=operadorif;
    Condicion=condicion;
    ThenEx=thenEx;
    OperadorElse=operadorelse;
    ElseCond=elsecond;

  }

  public Token OperadorIf {get;}
  public Expression Condicion {get;}

  public Expression ThenEx{get;}

  public Token OperadorElse {get;}

  public Expression ElseCond{get;}


   public override Token.TokenType Kind{get{return Token.TokenType.IfExpression;}}

   public override IEnumerable<Node> GetChildren()
   {
    yield return OperadorIf ;
    yield return Condicion ;
    yield return OperadorElse ;
    yield return ElseCond ;

   }

}




sealed class UnaryExpression :Expression 
{

public UnaryExpression ( Token operador , Expression operando )
{
  Operando = operando ;
  Operador =  operador ;
  
}

public Expression Operando {get;}
public Token Operador {get;}


public override Token.TokenType Kind {get{return Token.TokenType.ExpresionUnaria;}}

public override IEnumerable<Node> GetChildren()
{

    
    yield return Operador;
    yield return Operando;
   
    
}

}

public sealed class FuncionExpression : Expression
{

  public FuncionExpression(Token name , List<Token> parameters , Expression body)
  {

     Name = name ; 
     Parameters=parameters;
     Body=body;
  }

 
  public Token Name { get;}
  public List<Token> Parameters { get;}
  public Expression Body { get;}

  public override Token.TokenType Kind {get{return Token.TokenType.ExpresionFuncion;}}

public override IEnumerable<Node> GetChildren()
{
    yield return Body;   
}
}

public sealed class CallFuncionExpression : Expression 
{

  public CallFuncionExpression (Token name , List<Expression> argumentos)
  {
    Name = name ; 
    Argumentos = argumentos;
  }

  public Token Name {get;}

  public Expression hola {get;}
  public List<Expression> Argumentos {get;}

  public IEnumerable<Expression> GetArguments()
  {
    foreach (var item in Argumentos)
    {
         yield return item;
      
    }
  }

   public override Token.TokenType Kind {get{return Token.TokenType.LlamadaFuncion;}}

   public override IEnumerable<Node> GetChildren()
   {
     yield return hola ;
   }

}
public sealed class StringExpression :Expression 
{

public StringExpression ( Token cadena )
{
  
  Cadena = cadena ;
  
}

public Token Cadena {get;}
public string Value {get{return Cadena.Value;}}

public override Token.TokenType Kind {get{return Token.TokenType.Cadena;}}

public override IEnumerable<Node> GetChildren()
{
    yield return Cadena;   
}

}

sealed class SyntaxTree 
{

public SyntaxTree(IEnumerable<string> errores , Expression root , Token endoffile)
{

Errores = errores.ToArray();    
Root = root;
Endoffile = endoffile;
 
}

public IReadOnlyList<string> Errores;
public Expression Root{get;}
public Token  Endoffile {get;}

public static SyntaxTree Parse (string text)
{
    var parser = new Parser(text);
    return parser.Parse();
}

}


class Parser
{
 

private int position ;
private Token[] tok;

private List<string> errorespar =  new List<string>();
 public Parser ( string text )
 {

    AnLex hola = new AnLex(text);
            
    List <Token> lt = new List <Token> (hola.GetTokens());
    tok = lt.ToArray();
     
    
 }

 public IEnumerable<string> Errores()
  {

  foreach (var error in errorespar)
  {
    yield return error;
  }
  }


 private Token Peek ( int  offset )
 {
   var index =  position + offset ;

   if(offset >= tok.Length)
   {
     return tok [tok.Length-1];
   }
   return tok[index];

 }

 private Token Current()
 {
    return Peek(0);

 }

 private Token NextToken()
 {
   var current = Current() ;
   position ++;
   return current ;

 }

 private Token MatchToken ( Token.TokenType kind)
 {

    if ( Current().Kind == kind )
    {
        return NextToken();
    }

    errorespar.Add($"Unexpected token: <{Current().Kind} , expected : <{kind}> ");
    return new Token ( kind , null);
 }

public SyntaxTree Parse()
{
 
 
 var expresion = ParseExpression ();
 var endoffile = MatchToken (Token.TokenType.FinArchivo);
 return new SyntaxTree ( errorespar , expresion , endoffile);

}

// private Expression ParseExpression()
// {
//     return ParseSR();
// }

private Expression ParseExpression()
{
  return ParseAssignmentExpression();
}

private Expression ParseAssignmentExpression()
{
  
  
  if(Peek(0).Kind==Token.TokenType.VariableToken && 
  Peek(1).Kind == Token.TokenType.IdentifierToken 
  && Peek(2).Kind == Token.TokenType.OperadorAsignacion )
  {

    NextToken();
    var identifierToken = NextToken();
    var operadorIgual = NextToken();
    var right = ParseAssignmentExpression();
    var operadorIn=NextToken();
    var expression = ParseAssignmentExpression();
    return new InExpression(new AssignmentExpression(identifierToken , operadorIgual , right ) , operadorIn , expression);
  }
  if(Peek(0).Kind==Token.TokenType.OperadorIf)
  {
    var operadorif=NextToken();
    var condicion = ParseAssignmentExpression();
    var thenEx = ParseAssignmentExpression();
    var operadorelse =  NextToken();
    var elsecond = ParseAssignmentExpression();
    return new IfExpression(operadorif , condicion , thenEx , operadorelse ,elsecond);

  }
  if(Peek(0).Kind==Token.TokenType.Cadena)
  {
    var cadena =NextToken();
    return new StringExpression(cadena);
  }
  else if(Peek(0).Value=="print")
  {
    var operadorprint=NextToken();
    var openpar = NextToken();
    var cadena = ParseAssignmentExpression();
    var closepar=NextToken();
    return new PrintExpression(operadorprint , openpar , cadena , closepar);
  }
  if (Peek(0).Value=="function")
  {
    List<Token> parametros = new List<Token>();
     var functionword=NextToken();
     var nombre =  NextToken();
     var openpar= NextToken();
     while(Current().Kind != Token.TokenType.LastParent)
     {
       var parametro=NextToken();
       parametros.Add(parametro);

       if(Current().Kind==Token.TokenType.Coma)
       {
         NextToken();
       }
     }
     var clospar = NextToken();
     var operadorfuncion=NextToken();
     var body = ParseAssignmentExpression();
     return new FuncionExpression(nombre , parametros , body);
  }
  if (Peek(0).Kind==Token.TokenType.IdentifierToken 
  && Peek(1).Kind==Token.TokenType.FirstParent)
  {

    List<Expression> argumentos = new List<Expression>();
    var name = NextToken();
    var FirstParent=NextToken();
     while(Current().Kind != Token.TokenType.LastParent)
     {
       var parametro=ParseAssignmentExpression();
       argumentos.Add(parametro);

       if(Current().Kind==Token.TokenType.Coma)
       {
         NextToken(); 
       }
     }
     var closepar = NextToken();
     return new CallFuncionExpression(name , argumentos);
  }
 
  return ParseBinaryExpression();

}
 private Expression ParseBinaryExpression(int parentPrecedence = 0)
{
 Expression left;

 var unaryOperatorPrecedence = GetUnaryExpressionPrecedence(Current().Kind);

if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
{
var operatorToken = NextToken();
var operand = ParseBinaryExpression(unaryOperatorPrecedence);
left = new UnaryExpression ( operatorToken, operand);               
}
else
{
left = ParsePrimaryExpression();
}            

while (true)
{
var precedence = GetBinaryExpressionPrecedence(Current().Kind);
if (precedence == 0 || precedence <= parentPrecedence)
break;

var operatorToken = NextToken();
var right = ParseBinaryExpression(precedence);
left = new BinaryExpression( left, operatorToken, right);
}

return left;
}

private static int GetUnaryExpressionPrecedence(Token.TokenType kind)
{

  switch(kind)
  {
    case Token.TokenType.OperadorSuma:
    case Token.TokenType.OperadorResta:
    case Token.TokenType.Negacion :
    return 6;

    default : return 0 ;
  }
}
private static  int GetBinaryExpressionPrecedence ( Token.TokenType kind)
{

  switch (kind)
  {
    case Token.TokenType.OperadorSuma:
    case Token.TokenType.OperadorResta:
     return 4;
    case Token.TokenType.OperadorMult:
    case Token.TokenType.OperadorDiv:
    return 5;
    case Token.TokenType.OperadorIgual:
    case Token.TokenType.OperadorDistinto:
    return 3;
    case Token.TokenType.Disyuncion :
    return 1;
    case Token.TokenType.Conjuncion :
    return 2;
   
    default : return 0;

  }
}

  public static Token.TokenType GetKeyword ( string text)
  {
    switch(text)
    {
      case "true" :
      return Token.TokenType.TrueToken;
      case "false" :
      return Token.TokenType.FalseToken;
      default :
      return Token.TokenType.Identificador;
      

    }
    
  }
 private Expression ParsePrimaryExpression ()
 {
    
    switch (Current().Kind)
    {
      case Token.TokenType.FirstParent :
      {
         var left = NextToken();
        var expresion = ParseExpression();
        var right = MatchToken (Token.TokenType.LastParent);
        return new ExpresionParentesis (left , expresion , right);
      }
    
      case Token.TokenType.TrueToken :
      case Token.TokenType.FalseToken :  

      {
        var keywordToken = NextToken(); 
      var value=keywordToken.Kind == Token.TokenType.TrueToken;
      return new NumberToken(Current());
      }

       case Token.TokenType.IdentifierToken :
       {
        var identifierToken = NextToken();
        return  new NameExpression(identifierToken);
       }
       
      default :
      var number = MatchToken (Token.TokenType.Numero);
      return new NumberToken ( number);
 }
    
 }
  
    

 public sealed  class  Compilacion
 {
    public Compilacion (SyntaxTree syntax)
    {
      Syntax = syntax ;
      
    }

    public SyntaxTree Syntax {get;} 

    //  public EvaluationResult Evaluate ()
    // {
    //   var binder = new Binder();
    //   var boundExpression = binder.BindExpression(Syntax.Root);

    //   var diagnostics = Syntax.Errores.Concat(binder.Diagnostics).ToArray();
    //   var evaluator = new Evaluator (boundExpression);
    //   var value = evaluator.Evaluate();

    // }

 }


 public struct TextSpan
 {
  public TextSpan ( int start, int length)
  {
    Start=start;
    Length=length;

  }

  public int Start {get;}
  public int Length {get;}
  public int End {get{return Start+Length;}}

 }

 public class Diagnostic 
 {
   public Diagnostic ( TextSpan span , string message)
   {
     Span=span ;
     Message=message;
   }


   public TextSpan Span{get;}
  public string  Message {get;}

  public override string ToString()
 {
  return Message;
 }
          
  }


 public sealed class EvaluationResult
 {
     public EvaluationResult(IEnumerable<string> errores , object value)
     {
      Errores=errores.ToArray();
      Value=value;

     }

     public IReadOnlyList<string> Errores {get;}
     public object Value{get;}

 }

 public void ParseExpressionUntil(Token.TokenType type)
 {
    while(Current().Kind!=type)
    {
      ParseAssignmentExpression();
    }

     
 }
 

}

}
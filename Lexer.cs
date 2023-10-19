using System.Reflection.Metadata;
using System ;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.IO.Pipes;
using System.Reflection;





namespace Project.Binding
{


    


    public  class Token : Node 
    {

        public  enum TokenType
        {

            PrintExpression,

            OperadorFuncion,

            ExpresionFuncion,

            LlamadaFuncion,
            Cadena,
            Identificador,
            Numero,
            ExpresionBinaria,
            OperadorSuma,
            OperadorResta,
            OperadorMult,
            OperadorDiv,
            OperadorPob,
            OperadorResto,
            OperadorConcat,
            OperadorAsignacion,

            ExpresionUnaria,
            PuntoComa,
            FirstParent,

            OperadorFuncionSimple,
            OperadorCadenaAux,
            LastParent,
            LlaveAbierta,
            LlaveCerrada,

            OperadorIf,

            OperadorElse,

            IfExpression,

            VariableToken,
            VariableFToken,
            Coma,

            InExpression,
            PalabraReservada,
            OperadorComparacionMayor,
            OperadorComparacionMenor,
            OperadorComparacionMayorIgual,
            OperadorComparacionMenorIgual,

            AssignmentExpression,

            OperadorIgual,
            OperadorDistinto,

            ExpresionParentesis,

            FinArchivo,

            TrueToken,
            FalseToken,
            IdentifierToken,

            NameExpression,

            Negacion,

            Conjuncion ,

            Disyuncion ,

        }

        public override TokenType Kind { get; }
        public string Value { get; set; }

        public Token(TokenType type, string value)
        {
            Kind = type;
            Value = value;
        }

        public override IEnumerable<Node> GetChildren()
        {
           return Enumerable.Empty<Token>();

        }

    }

    class AnLex
    {

        private readonly string code;
        private int position;

        public char Current {get{return Peek(0);}}

        private List<string> erroreslex = new List<string>();

        public AnLex(string code)
        {
            this.code = code;
            position = 0;

        }

        public IEnumerable<string> Errores()
        {

            foreach (var error in erroreslex)
            {
                yield return error;
            }
        }


        public List<Token> GetTokens()
        {
            var tokens = new List<Token>();

            while (true)
            {
                
                 
                if(position >= code.Length )
                {
                    tokens.Add(new Token(Token.TokenType.FinArchivo , "" ));
                    break;
                }

            
                if (char.IsWhiteSpace(Current))
                {

                    position++;
                    continue;

                }

               
                

                else if (Char.IsDigit(Current))
                {
                    string number = GetNumber();
                    tokens.Add(new Token ( Token.TokenType.Numero ,number ));
                    // var start = position ;
                    
                    // while(Char.IsDigit(Current))
                    // {
                    //     position++;     
                    // }

                    // var length = position - start ;
                    // var text = code.Substring ( start , length  );

                    // if(!int.TryParse ( text , out var value))
                    // {

                    //     erroreslex.Add($"El numero {text} no puede ser representado por Int ");
                    // }
                    // else
                    // {
                        
                    //     tokens.Add(new Token ( Token.TokenType.Numero , text ));
                        
                    // }



                }

                else if (Char.IsLetter(Current))
                {
                    var identifier = GetIdentifier();

                    if(identifier=="let")
                    {
                        tokens.Add(new Token(Token.TokenType.VariableToken, identifier));
                    }
                    else if(identifier=="in")
                    {
                        tokens.Add(new Token(Token.TokenType.VariableFToken, identifier));
                    }
                    else if (IsKeyword(identifier))
                    {
                        tokens.Add(new Token(Token.TokenType.PalabraReservada, identifier));

                    }
                    else if (identifier=="if")
                    {
                        tokens.Add(new Token(Token.TokenType.OperadorIf, identifier));
                    }
                    else if(IsMathFunction(identifier))
                    {
                        tokens.Add(new Token(Token.TokenType.OperadorFuncionSimple, identifier));
                    }
                    else
                    {
                        tokens.Add(new Token(Token.TokenType.IdentifierToken, identifier));

                    }


                }

                

                else if (Current == '"')
                {
                    var str = GetString();
                    tokens.Add(new Token(Token.TokenType.Cadena, str));

                }

                

                    switch (Current)
                    {
                        case ';':
                            tokens.Add(new Token(Token.TokenType.PuntoComa, ";"));
                            position++;
                            break;

                        case '+':
                            tokens.Add(new Token(Token.TokenType.OperadorSuma, "+"));
                            position++;
                            break;

                        case '-':
                        
                            tokens.Add(new Token(Token.TokenType.OperadorResta, "-"));
                            position++;
                            break;
                        case '*':
                            tokens.Add(new Token(Token.TokenType.OperadorMult, "*"));
                            position++;
                            break;
                        case '/':
                            tokens.Add(new Token(Token.TokenType.OperadorDiv, "/"));
                            position++;
                            break;
                        case '^':
                            tokens.Add(new Token(Token.TokenType.OperadorPob, "^"));
                            position++;
                            break;
                        case '%':
                            tokens.Add(new Token(Token.TokenType.OperadorResto, "%"));
                            position++;
                            break;
                        case '@':
                            tokens.Add(new Token(Token.TokenType.OperadorConcat, "@"));
                            position++;
                            break;
                        case '=':

                            if (Next() == '=')
                            {
                                tokens.Add(new Token(Token.TokenType.OperadorIgual, "=="));
                                position += 2;

                            }
                            else if(Next()== '>')
                            {
                                tokens.Add(new Token (Token.TokenType.OperadorFuncion , "=>" ));
                                position+=2;

                            }
                            else
                            {
                                 tokens.Add(new Token (Token.TokenType.OperadorAsignacion , "=" ));
                                position++;
                            }

                            break;

                        case '!':

                            if (Next() == '=')
                            {
                                tokens.Add(new Token(Token.TokenType.OperadorDistinto, "!="));
                                 position += 2;

                            }
                            else
                            {
                                tokens.Add(new Token(Token.TokenType.Negacion , "!"));
                                position++;
                            }

                            break;

                            case '&' :
                            if(Next()=='&')
                            {
                                tokens.Add(new Token (Token.TokenType.Conjuncion , "&&"));
                                position +=2;
                            }
                            break;
                            case '\\' :
                            if(Next()=='&')
                            {
                                tokens.Add(new Token (Token.TokenType.OperadorCadenaAux , "\\"));
                                position +=2;
                            }
                            break;

                            case '|' : 
                            if(Next()=='|')
                            {
                                tokens.Add(new Token (Token.TokenType.Disyuncion , "||"));
                                position+=2;
                            }
                            break;


                        case '<':
                            if (Next() == '=')
                            {

                                tokens.Add(new Token(Token.TokenType.OperadorComparacionMenorIgual, "<="));
                               position += 2;
                            }
                            else
                            {

                                tokens.Add(new Token(Token.TokenType.OperadorComparacionMenor, "<"));
                               position++;
                            }

                            break;

                        case '>':
                            if (Next() == '=')
                            {

                                tokens.Add(new Token(Token.TokenType.OperadorComparacionMayorIgual, ">="));
                                position += 2;
                            }

                            else
                            {
                                tokens.Add(new Token(Token.TokenType.OperadorComparacionMayor, " >"));
                               position++;
                            }
                            break;

                        case '(':
                            tokens.Add(new Token(Token.TokenType.FirstParent, "("));
                            position++;
                            break;
                        case ')':
                            tokens.Add(new Token(Token.TokenType.LastParent, ")"));
                            position++;
                            break;
                        case ',':
                            tokens.Add(new Token(Token.TokenType.Coma, ","));
              position++;
                            break;
                        case '{':
                            tokens.Add(new Token(Token.TokenType.LlaveAbierta, "{"));
                            position++;
                            break;
                        case '}':
                            tokens.Add(new Token(Token.TokenType.LlaveCerrada, "}"));
                            position++;
                            break;

                        default:
                            
                            erroreslex.Add($" ERROR : Caracter incorrecto : '{Current}'");
                            position++;
                            break;
                    
                    }

                   

                

            //         if(char.IsLetter(currentchar))
            //     {
            //         var start = position;

            //         while(char.IsLetter(currentchar))
            //         position ++;

            //         var length = position - start ;
            //         var text = code.Substring(start , length);
            //         var kind =  Parser.GetKeyword(text);
            //         tokens.Add(new Token (kind , text )); 
            //     }
        }


            return tokens;

        }








        private string GetNumber()
        {

            var start = position;

            while (position < code.Length && (char.IsDigit(code [position] ) || code[position] == '.'))
            {

                position++;

            }

            return code.Substring(start, position - start);

        }

        private string GetIdentifier()
        {

            var start = position;

            while (position < code.Length && (char.IsLetterOrDigit(code[position]) || code[position] == '_')) 
            {
                position++;

            }

            return code.Substring(start, position - start);

        }

        private bool IsKeyword(string identifier)
        {
            switch (identifier)
            {
                case "print":
                
                case "function":
                    
                case "else":

               return true;

                default:

              return   false;
            }

        }

         private bool IsMathFunction(string identifier)
        {
            switch (identifier)
            {
                case "tan":
                
                case "cot":
                    
                case "sen":

                case "cos":

                case "log":

               return true;

                default:

              return   false;
            }

        }

        private string GetString()
        {
            var start = position + 1;
            position++;

            while (position < code.Length && code[position] != '"')
            {

                position++;

            }

            var str = code.Substring(start, position - start);

            position++;

            return str;

        }

        private char Peek(int offset)
        {

            int nextposition = position + offset;

            if (nextposition < code.Length)
            {

                return code[nextposition];

            }

            else
            {
                return '\0';

            }


        }

        private char Next ()
        {

           return  Peek(1);

        }


    }




}
namespace myc
{
    public class AST_Parse(StringReader tokenStream)
    {
        public readonly TokenStream TokenStream = new(tokenStream);

        protected void Expect(TokensEnum expected)
        {
            string actual = TokenStream.Get_Token();
            if (actual != expected.ToString())
            {
                throw new Exception(
                    string.Format("Expected: {0}, Actual: {1}", expected, actual)
                );
            }
        }

        public AST_Program Parse()
        {
            return Parse_Program();
        }


        public AST_Program Parse_Program()
        {
            AST_Function func = Parse_Function();

            try
            {
                string token = TokenStream.Get_Token();
                throw new Exception("Unexpected tokens after function definition");
            }
            catch (EndOfStreamException)
            {
                //We expect an exception here and can ignore it.
            }

            return new(func);
        }

        public AST_Function Parse_Function()
        {
            Expect(TokensEnum.KWInt);
            AST_Identifier ident = Parse_Identifier();
            Expect(TokensEnum.OpenParen);
            Expect(TokensEnum.KWVoid);
            Expect(TokensEnum.CloseParen);
            Expect(TokensEnum.OpenBrace);

            TokensEnum nextToken = TokenStream.Peek_TokenEnum();

            List<AST_BlockItem> blockItems = [];
            while (nextToken != TokensEnum.CloseBrace)
            {
                blockItems.Add(Parse_BlockItem());
                nextToken = TokenStream.Peek_TokenEnum();
            }
            // AST_Statement stmt = Parse_Statement();
            Expect(TokensEnum.CloseBrace);

            return new(ident, blockItems);
        }

        public AST_BlockItem Parse_BlockItem()
        {
            TokensEnum token = TokenStream.Peek_TokenEnum();

            switch (token)
            {
                case TokensEnum.KWInt:
                    return new AST_BlockDeclaration(Parse_Declaration());
                default:
                    return new AST_BlockStatement(Parse_Statement());
            }

        }


        //(* <declaration> ::= "int" <identifier> [ "=" <exp> ] ";" *)
        public AST_Declaration Parse_Declaration()
        {
            Expect(TokensEnum.KWInt);
            AST_Identifier ident = Parse_Identifier();

            TokensEnum keyToken = Enum.Parse<TokensEnum>(TokenStream.Get_Token());
            AST_Factor? init_exp;
            switch (keyToken)
            {
                case TokensEnum.Semicolon:
                    init_exp = null;
                    break;
                case TokensEnum.Assignment:
                    Console.WriteLine("found Assignment");
                    init_exp = Parse_Expression(0);
                    Expect(TokensEnum.Semicolon);
                    break;
                default:
                    throw new Exception(string.Format("Invalid Decaration Token: {0}", keyToken));
            }

            return new(ident.Name, init_exp);
        }

        public AST_Identifier Parse_Identifier()
        {
            string token = TokenStream.Get_Token();
            string[] ident = token.Split(" ");
            if (
                ident.Length != 2 ||
                ident[0] != TokensEnum.Identifier.ToString()
               )
            {
                throw new Exception("Invalid Indentifier Token");
            };
            string name = ident[1].Replace("\"", "");

            return new(name);
        }


        public AST_Statement Parse_Statement()
        {
            Console.WriteLine("Entering Parse_Statement");
            TokensEnum token = TokenStream.Peek_TokenEnum();

            switch (token)
            {
                case TokensEnum.KWReturn:
                    Expect(TokensEnum.KWReturn);
                    AST_Factor retExpr = Parse_Expression(0);
                    Expect(TokensEnum.Semicolon);
                    return new AST_Return(retExpr);
                case TokensEnum.Semicolon:
                    Expect(TokensEnum.Semicolon);
                    return new AST_Null();
                default:
                    AST_Factor expr = Parse_Expression(0);
                    Expect(TokensEnum.Semicolon);
                    return new AST_Expression(expr);
            };
        }

        public AST_Factor Parse_Expression(int minPrecedence)
        {

            AST_Factor leftFactor = Parse_Factor();

            TokensEnum nextToken = TokenStream.Peek_TokenEnum();
            Console.WriteLine(string.Format("Entering Parse_Expression: {0}",nextToken));

            // Parse expression loop
            while (GetPrecedence(nextToken) >= minPrecedence)
            {
                if (nextToken == TokensEnum.Assignment)
                {
                    Console.WriteLine("Handle Token");
                    string _ = TokenStream.Get_Token();  //consume the '=' token
                    AST_Factor rightFactor = Parse_Expression(GetPrecedence(nextToken));

                    leftFactor = new AST_Assignment(leftFactor, rightFactor);
                    Console.WriteLine(string.Format("assignment {0}", leftFactor.Print()));
                }
                else
                {
                    Console.WriteLine("Handle Other");
                    AST_BinaryOp oper = Parse_BinaryOp();
                    AST_Factor rightFactor = Parse_Expression(GetPrecedence(nextToken) + 1);

                    leftFactor = new AST_Binary(leftFactor, oper, rightFactor);
                    Console.WriteLine(string.Format("binary {0}", leftFactor.Print()));
                }
                //Peek to see what the next token is
                nextToken = TokenStream.Peek_TokenEnum();
                Console.WriteLine(string.Format("Next Token: {0}", nextToken));
            }
            Console.WriteLine("Leaving Parse_Expression");
            return leftFactor;
        }

        public AST_Factor Parse_Factor()
        {
            TokensEnum keyToken = TokenStream.Peek_TokenEnum();

            Console.WriteLine(string.Format("Enter Parse_Factor {0}",keyToken));
            switch (keyToken)
            {
                case TokensEnum.Constant:
                    return Parse_Constant();
                case TokensEnum.Identifier:
                    return new AST_Var(Parse_Identifier());
                case TokensEnum.Tilde:
                case TokensEnum.Hyphen:
                case TokensEnum.LogicalNOT:
                    AST_UnaryOp unOp = Parse_UnaryOp();
                    AST_Factor unaryInnerExp = Parse_Factor();
                    return new AST_Unary(unOp, unaryInnerExp);
                case TokensEnum.OpenParen:
                    _ = TokenStream.Get_Token();  //consume the '('
                    AST_Factor innerExp = Parse_Expression(0);
                    Expect(TokensEnum.CloseParen);
                    return innerExp;
                default:
                    throw new Exception(string.Format("Invalid Expression Token: {0}", keyToken));

            }
        }

        public AST_Int Parse_Constant()
        {
            string? token = TokenStream.Get_Token();
            string[] tokenParts = token.Split(' ');

            if (tokenParts.Length != 2)
            {
                throw new Exception("Invalid Expression - Constant missing value");
            }

            return new AST_Int(int.Parse(tokenParts[1]));
        }

        public AST_UnaryOp Parse_UnaryOp()
        {
            string? token = TokenStream.Get_Token();
            string[] tokenParts = token.Split(' ');

            _ = Enum.TryParse(tokenParts[0], out TokensEnum keyToken);

            return keyToken switch
            {
                TokensEnum.Tilde => new AST_Complement(),
                TokensEnum.Hyphen => new AST_Negate(),
                TokensEnum.LogicalNOT => new AST_Not(),
                _ => throw new Exception("Invalid Unary Op Token"),
            };
        }


        public AST_BinaryOp Parse_BinaryOp()
        {
            string? token = TokenStream.Get_Token();
            string[] tokenParts = token.Split(' ');

            _ = Enum.TryParse(tokenParts[0], out TokensEnum keyToken);

            switch (keyToken)
            {
                case TokensEnum.Plus:
                    return new AST_Add();
                case TokensEnum.Hyphen:
                    return new AST_Subtract();
                case TokensEnum.Asterisk:
                    return new AST_Multiply();
                case TokensEnum.ForwardSlash:
                    return new AST_Divide();
                case TokensEnum.Percent:
                    return new AST_Mod();
                case TokensEnum.LogicalAND:
                    return new AST_LogicalAnd();
                case TokensEnum.LogicalOR:
                    return new AST_LogicalOr();
                case TokensEnum.EqualEqual:
                    return new AST_Equal();
                case TokensEnum.NotEqual:
                    return new AST_NotEqual();
                case TokensEnum.LessThan:
                    return new AST_LessThan();
                case TokensEnum.GreaterThan:
                    return new AST_GreaterThan();
                case TokensEnum.LessOrEqual:
                    return new AST_LessOrEqual();
                case TokensEnum.GreaterOrEqual:
                    return new AST_GreaterOrEqual();

                default:
                    throw new Exception(string.Format("Invalid Binary Op Token: {0}", keyToken));
            }
        }

        private bool IsBinaryOp(TokensEnum opToken)
        {
            return opToken switch
            {
                TokensEnum.Asterisk or
                TokensEnum.ForwardSlash or
                TokensEnum.Percent or
                TokensEnum.Plus or
                TokensEnum.Hyphen or
                TokensEnum.LessThan or
                TokensEnum.GreaterThan or
                TokensEnum.LessOrEqual or
                TokensEnum.GreaterOrEqual or
                TokensEnum.EqualEqual or
                TokensEnum.NotEqual or
                TokensEnum.LogicalAND or
                TokensEnum.LogicalOR
                    => true,

                _ => false,
            };
        }

        private int GetPrecedence(TokensEnum opToken)
        {
            return opToken switch
            {
                TokensEnum.Asterisk or
                TokensEnum.ForwardSlash or
                TokensEnum.Percent => 50,

                TokensEnum.Plus or
                TokensEnum.Hyphen => 45,

                TokensEnum.LessThan or
                TokensEnum.GreaterThan or
                TokensEnum.LessOrEqual or
                TokensEnum.GreaterOrEqual => 35,

                TokensEnum.EqualEqual or
                TokensEnum.NotEqual => 30,

                TokensEnum.LogicalAND => 10,

                TokensEnum.LogicalOR => 5,

                TokensEnum.Assignment => 1,

                _ => 0,
            };
        }


    }
}
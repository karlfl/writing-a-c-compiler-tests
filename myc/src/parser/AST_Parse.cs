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

            return new(ident, Parse_Block());
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
            }
            ;
            string name = ident[1].Replace("\"", "");

            return new(name);
        }


        public AST_Statement Parse_Statement()
        {
            TokensEnum token = TokenStream.Peek_TokenEnum();

            switch (token)
            {
                case TokensEnum.KWReturn:
                    Expect(TokensEnum.KWReturn);
                    AST_Factor retExpr = Parse_Expression(0);
                    Expect(TokensEnum.Semicolon);
                    return new AST_Return(retExpr);
                case TokensEnum.KWIf:
                    Expect(TokensEnum.KWIf);
                    Expect(TokensEnum.OpenParen);
                    AST_Factor cond = Parse_Expression(0);
                    Expect(TokensEnum.CloseParen);
                    AST_Statement thenStmt = Parse_Statement();
                    TokensEnum elseToken = TokenStream.Peek_TokenEnum();
                    AST_Statement? elseStmt = null;
                    if (elseToken == TokensEnum.KWElse)
                    {
                        Expect(TokensEnum.KWElse);
                        elseStmt = Parse_Statement();
                    }
                    return new AST_If(cond, thenStmt, elseStmt);
                case TokensEnum.OpenBrace:
                    return new AST_Compound(Parse_Block());
                case TokensEnum.KWBreak:
                    Expect(TokensEnum.KWBreak);
                    Expect(TokensEnum.Semicolon);
                    return new AST_Break("");
                case TokensEnum.KWContinue:
                    Expect(TokensEnum.KWContinue);
                    Expect(TokensEnum.Semicolon);
                    return new AST_Continue("");
                case TokensEnum.KWWhile:
                    Expect(TokensEnum.KWWhile);
                    Expect(TokensEnum.OpenParen);
                    AST_Factor whileCond = Parse_Expression(0);
                    Expect(TokensEnum.CloseParen);
                    AST_Statement whileBody = Parse_Statement();
                    return new AST_While(whileCond, whileBody, "");
                case TokensEnum.KWDo:
                    Expect(TokensEnum.KWDo);
                    AST_Statement doBody = Parse_Statement();
                    Expect(TokensEnum.KWWhile);
                    Expect(TokensEnum.OpenParen);
                    AST_Factor doCond = Parse_Expression(0);
                    Expect(TokensEnum.CloseParen);
                    Expect(TokensEnum.Semicolon);
                    return new AST_DoWhile(doBody, doCond, "");
                case TokensEnum.KWFor:
                    Expect(TokensEnum.KWFor);
                    Expect(TokensEnum.OpenParen);
                    AST_ForInit init = Parse_ForInit();
                    AST_Factor? forCond = Parse_OptionalExpression(TokensEnum.Semicolon);
                    AST_Factor? forPost = Parse_OptionalExpression(TokensEnum.CloseParen);
                    AST_Statement forBody = Parse_Statement();
                    return new AST_For(init, forCond, forPost, forBody, "");
                case TokensEnum.Semicolon:
                    Expect(TokensEnum.Semicolon);
                    return new AST_Null();
                default:
                    AST_Factor expr = Parse_Expression(0);
                    Expect(TokensEnum.Semicolon);
                    return new AST_Expression(expr);
            }
        }

        private AST_ForInit Parse_ForInit()
        {
            TokensEnum token = TokenStream.Peek_TokenEnum();

            switch (token)
            {
                case TokensEnum.KWInt:
                    return new AST_InitDecl(Parse_Declaration());
                default:
                    AST_Factor? exp = Parse_OptionalExpression(TokensEnum.Semicolon);
                    return new AST_InitExp(exp);
            }
        }

        private AST_Factor? Parse_OptionalExpression(TokensEnum delimiter)
        {
            TokensEnum token = TokenStream.Peek_TokenEnum();

            if (token == delimiter)
            {
                Expect(delimiter);
                return null;
            }
            else
            {
                AST_Factor? exp = Parse_Expression(0);
                Expect(delimiter);
                return exp;
            }
        }

        public AST_Factor Parse_Expression(int minPrecedence)
        {

            AST_Factor leftFactor = Parse_Factor();
            TokensEnum nextToken = TokenStream.Peek_TokenEnum();

            // Parse expression loop
            while (GetPrecedence(nextToken) >= minPrecedence)
            {
                if (nextToken == TokensEnum.Assignment)
                {
                    Expect(TokensEnum.Assignment);  //consume the '=' token
                    AST_Factor rightFactor = Parse_Expression(GetPrecedence(nextToken));
                    leftFactor = new AST_Assignment(leftFactor, rightFactor);
                }
                else if (nextToken == TokensEnum.QuestionMark)
                {
                    AST_Factor middleFactor = Parse_ConditionalMiddle();
                    AST_Factor rightFactor = Parse_Expression(GetPrecedence(nextToken));
                    leftFactor = new AST_Conditional(leftFactor, middleFactor, rightFactor);
                }
                else
                {
                    AST_BinaryOp oper = Parse_BinaryOp();
                    AST_Factor rightFactor = Parse_Expression(GetPrecedence(nextToken) + 1);
                    leftFactor = new AST_Binary(leftFactor, oper, rightFactor);
                }
                //Peek to see what the next token is
                nextToken = TokenStream.Peek_TokenEnum();
            }
            return leftFactor;
        }

        private AST_Factor Parse_ConditionalMiddle()
        {
            Expect(TokensEnum.QuestionMark);
            AST_Factor middle = Parse_Expression(0);
            Expect(TokensEnum.Colon);
            return middle;
        }

        public AST_Factor Parse_Factor()
        {
            TokensEnum keyToken = TokenStream.Peek_TokenEnum();

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
                    Expect(TokensEnum.CloseParen); //consime the ')'
                    return innerExp;
                default:
                    throw new Exception(string.Format("Invalid Expression Token: {0}", keyToken));

            }
        }

        public AST_Int Parse_Constant()
        {
            string? token = TokenStream.Get_Token();
            string[] tokenParts = token.Split(" ");
            switch (Enum.Parse<TokensEnum>(tokenParts[0]))
            {
                case TokensEnum.Constant:
                    if (tokenParts.Length != 2)
                    {
                        throw new Exception("Invalid Expression - Constant missing value");
                    }
                    return new AST_Int(int.Parse(tokenParts[1]));
                default:
                    throw new Exception("Invalid Expression: Constant Expected");

            }
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
            TokensEnum keyToken = Enum.Parse<TokensEnum>(tokenParts[0]);
            // _ = Enum.TryParse(tokenParts[0], out TokensEnum keyToken);

            return keyToken switch
            {
                TokensEnum.Plus => new AST_Add(),
                TokensEnum.Hyphen => new AST_Subtract(),
                TokensEnum.Asterisk => new AST_Multiply(),
                TokensEnum.ForwardSlash => new AST_Divide(),
                TokensEnum.Percent => new AST_Mod(),
                TokensEnum.PlusEqual=> new AST_AddEqual(),
                TokensEnum.HyphenEqual => new AST_SubtractEqual(),
                TokensEnum.AsteriskEqual => new AST_MultiplyEqual(),
                TokensEnum.ForwardSlashEqual => new AST_DivideEqual(),
                TokensEnum.PercentEqual => new AST_ModEqual(),
                TokensEnum.AND => new AST_AND(),
                TokensEnum.OR => new AST_OR(),
                TokensEnum.XOR => new AST_XOR(),
                TokensEnum.LeftShift => new AST_LeftShift(),
                TokensEnum.RightShift => new AST_RightShift(),
                TokensEnum.LogicalAND => new AST_LogicalAnd(),
                TokensEnum.LogicalOR => new AST_LogicalOr(),
                TokensEnum.EqualEqual => new AST_Equal(),
                TokensEnum.NotEqual => new AST_NotEqual(),
                TokensEnum.LessThan => new AST_LessThan(),
                TokensEnum.GreaterThan => new AST_GreaterThan(),
                TokensEnum.LessOrEqual => new AST_LessOrEqual(),
                TokensEnum.GreaterOrEqual => new AST_GreaterOrEqual(),
                _ => throw new Exception(string.Format("Invalid Binary Op Token: {0}", keyToken)),
            };
        }

        public AST_Block Parse_Block()
        {
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

            return new(blockItems);

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


        private static int GetPrecedence(TokensEnum opToken)
        {
            return opToken switch
            {
                TokensEnum.Asterisk or
                TokensEnum.ForwardSlash or
                TokensEnum.Percent => 50,

                TokensEnum.Plus or
                TokensEnum.Hyphen => 45,

                TokensEnum.LeftShift or
                TokensEnum.RightShift => 40,

                TokensEnum.LessThan or
                TokensEnum.GreaterThan or
                TokensEnum.LessOrEqual or
                TokensEnum.GreaterOrEqual => 35,

                TokensEnum.EqualEqual or
                TokensEnum.NotEqual => 30,

                TokensEnum.AND => 25,
                TokensEnum.XOR => 20,
                TokensEnum.OR => 15,

                TokensEnum.LogicalAND => 10,

                TokensEnum.LogicalOR => 5,

                TokensEnum.QuestionMark => 3,

                TokensEnum.Assignment => 1,

                _ => -1,
            };
        }


    }
}
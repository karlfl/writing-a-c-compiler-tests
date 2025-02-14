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
                throw new Exception(string.Format("Invalid Indentifier Token: {0}", token));
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
                switch (nextToken)
                {
                    case TokensEnum.Assignment:
                    case TokensEnum.PlusEqual:
                    case TokensEnum.HyphenEqual:
                    case TokensEnum.AsteriskEqual:
                    case TokensEnum.ForwardSlashEqual:
                    case TokensEnum.PercentEqual:
                    case TokensEnum.ANDEqual:
                    case TokensEnum.OREqual:
                    case TokensEnum.XOREqual:
                    case TokensEnum.LeftShiftEqual:
                    case TokensEnum.RightShiftEqual:
                        {
                            leftFactor = Parse_Assignment(leftFactor, nextToken);
                            break;
                        }
                    case TokensEnum.QuestionMark:
                        {
                            AST_Factor middleFactor = Parse_ConditionalMiddle();
                            AST_Factor rightFactor = Parse_Expression(GetPrecedence(nextToken));
                            leftFactor = new AST_Conditional(leftFactor, middleFactor, rightFactor);
                            break;
                        }
                    default:
                        {
                            AST_BinaryOp oper = Parse_BinaryOp();
                            AST_Factor rightFactor = Parse_Expression(GetPrecedence(nextToken) + 1);
                            leftFactor = new AST_Binary(leftFactor, oper, rightFactor);
                            break;
                        }
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
            AST_Factor factor;

            switch (keyToken)
            {
                case TokensEnum.Constant:
                    factor = Parse_Constant();
                    break;
                case TokensEnum.Identifier:
                    factor = new AST_Var(Parse_Identifier());
                    break;
                case TokensEnum.Tilde:
                case TokensEnum.Hyphen:
                case TokensEnum.LogicalNOT:
                    AST_UnaryOp unOp = Parse_UnaryOp();
                    AST_Factor unaryInnerExp = Parse_Factor();
                    factor = new AST_Unary(unOp, unaryInnerExp);
                    break;
                case TokensEnum.OpenParen:
                    _ = TokenStream.Get_Token();  //consume the '('
                    factor = Parse_Expression(0);
                    Expect(TokensEnum.CloseParen); //consime the ')'
                    break;
                case TokensEnum.DoubleHyphen:
                case TokensEnum.DoublePlus:
                    // Handle prefix increment/decrement
                    AST_BinaryOp preOp = Parse_BinaryOp();
                    //Expect a factor next (most often a variable)
                    AST_Factor afactor = Parse_Factor();
                    if (typeof(AST_Var).Name == afactor.GetType().Name)
                    {
                        factor = new AST_IncDec(preOp, afactor, true);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid Increment/Decrement Token: {0}", afactor));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Invalid Expression Token: {0}", keyToken));

            }

            //look forward to see if there is an increment or decrement suffix
            TokensEnum nextToken = TokenStream.Peek_TokenEnum();
            if (
                nextToken == TokensEnum.DoubleHyphen ||
                nextToken == TokensEnum.DoublePlus)
            {
                AST_BinaryOp postOp = Parse_BinaryOp();
                factor = new AST_IncDec(postOp, factor, false);
            }

            return factor;
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

        public AST_Assignment Parse_Assignment(AST_Factor leftFactor, TokensEnum keyToken)
        {
            // get/consume the current token
            AST_Assignment leftAssigment;
            switch (keyToken)
            {
                case TokensEnum.Assignment:
                    {
                        // consume the '=' token
                        Expect(TokensEnum.Assignment);
                        AST_Factor rightFactor = Parse_Expression(GetPrecedence(keyToken));
                        leftAssigment = new AST_Assignment(leftFactor, rightFactor);
                        break;
                    }
                default:
                    {
                        // This is a compound binary op with an assignment;
                        //consume the 'x=' token (+=, -=, etc.)
                        AST_BinaryOp binOp = Parse_BinaryOp();
                        AST_Factor rightFactor = Parse_Expression(GetPrecedence(keyToken));
                        // Build the binary expression 
                        AST_Factor binFactor = new AST_Binary(leftFactor, binOp, rightFactor);
                        // Build the assigment fusing Right Precence
                        leftAssigment = new AST_Assignment(leftFactor, binFactor);
                        break;
                    }
            }
            return leftAssigment;

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
            // Consume the binary operation
            string? token = TokenStream.Get_Token();
            string[] tokenParts = token.Split(' ');
            TokensEnum keyToken = Enum.Parse<TokensEnum>(tokenParts[0]);

            return keyToken switch
            {
                TokensEnum.Plus or
                TokensEnum.PlusEqual => new AST_Add(),

                TokensEnum.Hyphen or
                TokensEnum.HyphenEqual => new AST_Subtract(),

                TokensEnum.Asterisk or
                TokensEnum.AsteriskEqual => new AST_Multiply(),

                TokensEnum.ForwardSlash or
                TokensEnum.ForwardSlashEqual => new AST_Divide(),

                TokensEnum.Percent or
                TokensEnum.PercentEqual => new AST_Mod(),

                TokensEnum.AND or
                TokensEnum.ANDEqual => new AST_AND(),

                TokensEnum.OR or
                TokensEnum.OREqual => new AST_OR(),

                TokensEnum.XOR or
                TokensEnum.XOREqual => new AST_XOR(),

                TokensEnum.LeftShift or
                TokensEnum.LeftShiftEqual => new AST_LeftShift(),

                TokensEnum.RightShift or
                TokensEnum.RightShiftEqual => new AST_RightShift(),

                TokensEnum.DoubleHyphen => new AST_Subtract(),
                TokensEnum.DoublePlus => new AST_Add(),

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
                TokensEnum.DoublePlus or
                TokensEnum.DoubleHyphen => 55,

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

                TokensEnum.Assignment or
                TokensEnum.PlusEqual or
                TokensEnum.HyphenEqual or
                TokensEnum.AsteriskEqual or
                TokensEnum.ForwardSlashEqual or
                TokensEnum.PercentEqual or
                TokensEnum.ANDEqual or
                TokensEnum.OREqual or
                TokensEnum.XOREqual or
                TokensEnum.LeftShiftEqual or
                TokensEnum.RightShiftEqual
                 => 1,

                _ => -1,
            };
        }


    }
}
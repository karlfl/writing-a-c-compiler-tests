namespace myc
{
    public class AST_Parse
    {
        public readonly StringReader TokenStream;

        public AST_Parse(StringReader tokenStream)
        {
            TokenStream = tokenStream;
        }

        protected string GetNextToken()
        {
            string token = this.TokenStream.ReadLine() ??
                throw new EndOfStreamException("Unexpected End of File");
            // Remove the end token ';' if there is one
            return token.Replace(";", "");
        }

        protected void Expect(TokensEnum expected)
        {
            string actual = GetNextToken();
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
                string token = GetNextToken();
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
            AST_Statement stmt = Parse_Statement();
            Expect(TokensEnum.CloseBrace);

            return new(ident, stmt);
        }

        public AST_Identifier Parse_Identifier()
        {
            string token = GetNextToken();
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
            Expect(TokensEnum.KWReturn);
            AST_Expression expr = Parse_Expression();
            Expect(TokensEnum.Semicolon);

            return new(TokensEnum.KWReturn, expr);
        }

        public AST_Expression Parse_Expression()
        {
            string? token = GetNextToken();
            string[] tokenParts = token.Split(' ');

            _ = Enum.TryParse(tokenParts[0], out TokensEnum keytoken);

            switch (keytoken)
            {
                case TokensEnum.Constant:
                    if(tokenParts.Length != 2) {throw new Exception("Invalid Expression - Constant missing value");}
                    int intValue = int.Parse(tokenParts[1]);
                    return new AST_Constant(intValue);
                case TokensEnum.Tilde:
                case TokensEnum.Hyphen:
                    AST_UnaryOp unOp =  Parse_UnaryOp(keytoken, tokenParts);
                    AST_Expression unExp = Parse_Expression();
                    return new AST_Unary(unOp, unExp);  
                case TokensEnum.OpenParen:
                    AST_Expression innerExp = Parse_Expression();
                    Expect(TokensEnum.CloseParen);
                    return innerExp;             
                default:
                    throw new Exception(string.Format("Invalid Expression Token: {0}", token));

            }
        }

        public AST_UnaryOp Parse_UnaryOp(TokensEnum keyToken, string[] tokens) {
            switch(keyToken) {
                case TokensEnum.Tilde:
                    return new AST_Complement();
                 case TokensEnum.Hyphen:
                    return new AST_Negate();
                default:
                    throw new Exception("Invalid Unary Op Token");
           }
        }



    }
}
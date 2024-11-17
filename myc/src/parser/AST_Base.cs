namespace myc {
    public abstract class AST_Base
    {
        public readonly StringReader TokenStream;

        public AST_Base(StringReader tokenStream)
        {
            TokenStream = tokenStream;
        }

        public abstract void Parse();
        public abstract string Print();

        protected string GetNextToken()
        {
            string token = this.TokenStream.ReadLine() ??
                throw new EndOfStreamException("Unexpected End of File");
            // Remove the end token ';' if there is one
            return token.Replace(";","");
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

    }
}

namespace myc {
    public class AST_Identifier : AST_Base
    {
        string name;
        public AST_Identifier(StringReader tokenStream) : base(tokenStream)
        {
            this.name = "";
        }

        public override void Parse()
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
            this.name = ident[1].Replace("\"","");
           
        }

        public override string Print()
        {
            return this.name;
        }
    }
}
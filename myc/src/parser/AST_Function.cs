using System.Text;

namespace myc
{
    internal class AST_Function : AST_Base
    {
        readonly AST_Identifier identifier;
        readonly AST_Statement statement;

        public AST_Function(StringReader tokenStream) : base(tokenStream)
        {
            this.identifier = new(tokenStream);
            this.statement = new(tokenStream);
        }

        public override void Parse()
        {
            Expect(TokensEnum.KWInt);
            this.identifier.Parse();
            Expect(TokensEnum.OpenParen);
            Expect(TokensEnum.KWVoid);
            Expect(TokensEnum.CloseParen);
            Expect(TokensEnum.OpenBrace);
            this.statement.Parse();
            Expect(TokensEnum.CloseBrace);
        }

        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine("\tFunction(");
            output.AppendLine(string.Format("\t\tname=\"{0}\",", this.identifier.Print()));
            string body = this.statement.Print();
            output.AppendLine(string.Format("\t\tbody={0}", body));
            output.AppendLine("\t)");
            return output.ToString();
        }
    }
}
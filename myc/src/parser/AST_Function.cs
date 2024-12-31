using System.Text;

namespace myc
{
    public class AST_Function : AST_Base
    {
        public readonly AST_Identifier Identifier;
        public readonly List<AST_Statement> Statements = [];

        public AST_Function(AST_Identifier identifier, AST_Statement statement)
        {
            this.Identifier = identifier;
            this.Statements.Add(statement);
        }

        public string Print()
        {
            StringBuilder output = new();
            output.AppendLine("    Function(");
            output.AppendLine(string.Format("        name=\"{0}\",", this.Identifier.Print()));
            string body = this.Statements[0].Print();
            output.AppendLine(string.Format("        body={0}", body));
            output.AppendLine("    )");
            return output.ToString();
        }
    }
}
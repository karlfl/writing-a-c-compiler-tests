using System.Text;

namespace myc
{
    public class AST_Function(AST_Identifier identifier, AST_Block body) : AST_Base
    {
        public readonly AST_Identifier Identifier = identifier;
        public readonly AST_Block Body = body;

        public string Print()
        {
            StringBuilder output = new();
            output.AppendLine("Function(");
            output.AppendLine(string.Format("name=\"{0}\",", this.Identifier.Print()));
            output.AppendLine("body={");
            output.AppendLine(Body.Print());
            output.AppendLine("}");
            output.AppendLine(")");
            return output.ToString();
        }
    }
}
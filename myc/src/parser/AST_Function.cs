using System.Text;

namespace myc
{
    public class AST_Function(AST_Identifier identifier, List<AST_BlockItem> body) : AST_Base
    {
        public readonly AST_Identifier Identifier = identifier;
        public readonly List<AST_BlockItem> Body = body;

        public string Print()
        {
            StringBuilder output = new();
            output.AppendLine("    Function(");
            output.AppendLine(string.Format("        name=\"{0}\",", this.Identifier.Print()));
            output.AppendLine("        body={");
            foreach (AST_BlockItem blockItem in this.Body)
            {
                output.AppendLine(string.Format("            {0}", blockItem.Print()));
            }
            output.AppendLine("        }");
            output.AppendLine("    )");
            return output.ToString();
        }
    }
}
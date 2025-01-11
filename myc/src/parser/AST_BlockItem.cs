using System.Text;

namespace myc
{

    public class AST_Block(List<AST_BlockItem> items) : AST_Base
    {
        public readonly List<AST_BlockItem> Items = items;
        public string Print()
        {
            StringBuilder output = new();
            output.AppendLine("{");
            foreach (AST_BlockItem item in Items)
            {
                output.AppendLine(item.Print());
            }
            output.AppendLine("}");
            return output.ToString();
        }
    }

    public abstract class AST_BlockItem : AST_Base
    {
        public abstract string Print();
    }

    public class AST_BlockStatement(AST_Statement statement) : AST_BlockItem
    {
        public readonly AST_Statement Statement = statement;

        public override string Print()
        {
            return this.Statement.Print();
        }
    }

    public class AST_BlockDeclaration(AST_Declaration declaration) : AST_BlockItem
    {
        public readonly AST_Declaration Declaration = declaration;

        public override string Print()
        {
            return this.Declaration.Print();
        }
    }
}
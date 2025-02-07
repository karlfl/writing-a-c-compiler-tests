namespace myc {
    public abstract class AST_ForInit : AST_Base
    {
        public abstract string Print();
    }

    public class AST_InitDecl(AST_Declaration declaration) : AST_ForInit
    {
        public readonly AST_Declaration Declaration = declaration;

        public override string Print()
        {
            return string.Format(" {0} ", Declaration.Print());
        }
    }

    public class AST_InitExp(AST_Factor? expression) : AST_ForInit
    {
        public readonly AST_Factor? Expression = expression;

        public override string Print()
        {
            return string.Format(" {0} ", Expression?.Print());
        }
    } 

}
namespace myc
{
    public abstract class AST_Expression : AST_Base
    {
        public abstract string Print();
    }

    public class AST_Constant : AST_Expression
    {
        public readonly int Value;

        public AST_Constant(int value)
        {
            this.Value = value;
        }

        public override string Print()
        {
            return string.Format("{0}", this.Value);
        }
    }
    
    public class AST_Unary : AST_Expression
    {
        public readonly AST_UnaryOp UnaryOp;
        public readonly AST_Expression Expression;

        public AST_Unary(AST_UnaryOp unOp, AST_Expression exp)
        {
            this.UnaryOp = unOp;
            this.Expression = exp;
        }

        public override string Print()
        {
            return string.Format("{0}({1})", UnaryOp.Print(), Expression.Print());
        }

    }

}
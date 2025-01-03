namespace myc
{
    public abstract class AST_Factor : AST_Base
    {
        public abstract string Print();
    }

    public class AST_Int(int value) : AST_Factor
    {
        public readonly int Value = value;

        public override string Print()
        {
            return string.Format("{0}", this.Value);
        }
    }
    
    public class AST_Unary(AST_UnaryOp unOp, AST_Factor factor) : AST_Factor
    {
        public readonly AST_UnaryOp UnaryOp = unOp;
        public readonly AST_Factor Factor = factor;

        public override string Print()
        {
            return string.Format("{0}({1})", UnaryOp.Print(), Factor.Print());
        }

    }

    public class AST_Binary(AST_Factor leftFactor, AST_BinaryOp binOp, AST_Factor rightFactor) : AST_Factor
    {
        public readonly AST_Factor LeftFactor = leftFactor;
        public readonly AST_BinaryOp BinaryOp = binOp;
        public readonly AST_Factor RightFactor = rightFactor;

        public override string Print()
        {
            return string.Format("({0} {1} {2})", LeftFactor.Print(), BinaryOp.Print(), RightFactor.Print());
        }

    }

}

using System.Text;

namespace myc
{
    // <program ::= <function>
    public class AST_Program : AST_Base
    {

        public readonly AST_Function Function;

        public AST_Program(AST_Function function)
        {
            Function = function;
        }


        public string Print()
        {
            StringBuilder output = new();
            output.AppendLine("Program(");
            output.AppendLine(Function.Print());
            output.AppendLine(")");
            return output.ToString();
        }
    }
}
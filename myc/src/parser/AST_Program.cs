
using System.Text;

namespace myc
{
    // <program ::= <function>
    public class AST_Program : AST_Base
    {

        public readonly AST_Function Function;

        public AST_Program(StringReader tokenStream) : base(tokenStream)
        {
            Function = new(tokenStream);
        }

        public override void Parse()
        {
            Function.Parse();

            try
            {
                string token = GetNextToken();
                throw new Exception("Unexpected tokens after function definition");
            }
            catch (EndOfStreamException)
            {
                //We expect an exception here and can ignore it.
            }
        }

        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine("Program(");
            output.AppendLine(Function.Print());
            output.AppendLine(")");
            return output.ToString();
        }
    }
}
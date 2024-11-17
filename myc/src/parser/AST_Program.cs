
using System.Text;

namespace myc
{
    // <program ::= <function>
    public class AST_Program : AST_Base
    {

        readonly AST_Function function;

        public AST_Program(StringReader tokenStream) : base(tokenStream)
        {
            function = new(tokenStream);
        }

        public override void Parse()
        {
            function.Parse();

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
            output.AppendLine(function.Print());
            output.AppendLine(")");
            return output.ToString();
        }
    }
}
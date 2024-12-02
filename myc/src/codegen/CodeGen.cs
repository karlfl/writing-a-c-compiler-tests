namespace myc
{
    public static class CodeGen
    {

        public static string Process(AST_Program program)
        {
            CG_Program _program = new(program);
            _program.CodeGen();
            return "";
        }
    }
}
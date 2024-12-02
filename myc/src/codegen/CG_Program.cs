namespace myc
{
    public class CG_Program{
        readonly AST_Program Program;

        readonly CG_Function Function;

        public CG_Program(AST_Program program)
        {
            this.Program = program;
            this.Function = new(program.Function);
        }

        public void CodeGen(){
            

        }
    }
}
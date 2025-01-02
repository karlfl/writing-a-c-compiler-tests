namespace myc
{
    public class Settings
    {
        public string ProgramFile;
        public bool RunThruCodeGen;
        public bool RunThruParser;
        public bool RunThruLexer;
        public bool RunThruTac;

        public bool RunAssembleLink
        {
            get
            {
                //only run the assemble and link if no switches supplied
                return
                    !this.RunThruLexer &&
                    !this.RunThruParser &&
                    !this.RunThruCodeGen &&
                    !this.RunThruTac;
            }
        }

        public Settings(string file, bool codeGen, bool parse, bool lex, bool tac)
        {
            this.ProgramFile = file;
            this.RunThruCodeGen = codeGen;
            this.RunThruParser = parse;
            this.RunThruLexer = lex;
            this.RunThruTac = tac;

        }
    }
}
namespace myc
{
    public class Settings
    {
        public string ProgramFile = "";
        public bool RunThruCodeGen;
        public bool RunThruParser;
        public bool RunThruLexer;
        public bool RunThruTac;

        public bool DebugMode;

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
    }
}
namespace myc
{
    public static class Parse
    {
        public static AST_Program Process(string lexTokens)
        {
            AST_Parse parser = new (new StringReader(lexTokens));

            return parser.Parse();
        }

        public static string Print(AST_Program program)
        {
            if (program != null)
                return program.Print();
            else
                return "";
        }
    }
}
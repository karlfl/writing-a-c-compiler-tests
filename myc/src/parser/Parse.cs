namespace myc
{
    public static class Parse
    {

        public static string Process(string lexTokens)
        {
            //Always start with a program node
            AST_Program program = new(new StringReader(lexTokens));
            program.Parse();

            return program.Print();
        }
    }
}
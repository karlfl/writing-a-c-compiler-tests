namespace myc
{
    public static class MyCompiler
    {
        internal static void Compile(string programFile)
        {
            Console.WriteLine("\nProgram File: {0}\n", programFile);


            Console.WriteLine("Preprocess");
            Utilities.RunGCCPreprocessor(programFile);

            //Compile
            Console.WriteLine("Compile");
            Utilities.GCCCompile(programFile);

            //cleanup preprocessor file
            Utilities.CleanUpGCCPreprocessor(programFile);

            //Assemble and Link
            Console.WriteLine("Assemble and Link");
            Utilities.AssembleAndLink(programFile);

            //cleanup asm file
            Utilities.CleanUpAssembleAndLink(programFile);


            Console.WriteLine("\nComplete\n");
        }

    }
}
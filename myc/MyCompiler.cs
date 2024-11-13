using System.Diagnostics.Tracing;
using System.Reflection.Metadata.Ecma335;

namespace myc
{
    public class MyCompiler
    {
        static bool RunLexer = false;   // Run lexer
        static bool RunParser = false;  // Run Lexer and Parser
        static bool RunCodeGen = false; // Run Lexer, Parser and Assembly Generator


        internal static void Compile(string programFile, bool runLexer, bool runParser, bool runCodeGen)
        {
            RunLexer = runLexer;
            RunParser = runParser;
            RunCodeGen = runCodeGen;

            Console.WriteLine("\nProgram File: {0}\n", programFile);


            Console.WriteLine("Preprocess");
            string prepFilePath = Utilities.RunGCCPreprocessor(programFile);

            //Compile
            Console.WriteLine("Compile");
            //Utilities.GCCCompile(programFile);
            Compile(prepFilePath);

            //cleanup preprocessor file
            Utilities.GCCPreprocessorCleanUp(programFile);

            if (RunCodeGen)
            {
                //Assemble and Link
                Console.WriteLine("Assemble and Link");
                Utilities.AssembleAndLink(programFile);

                //cleanup asm file
                Utilities.AssembleAndLinkCleanUp(programFile);
            }

            Console.WriteLine("\nComplete\n");
        }

        internal static void Compile(string programFile)
        {
            string? source = ReadFile(programFile);
            string tokens;

            if (source != null)
            {
                if (RunLexer)
                {
                    tokens = Lex.Process(source);
                    Console.WriteLine("\n{0}",tokens);

                }
            }
        }

        internal static string? ReadFile(string filePath)
        {
            string prepFilePath = filePath.Split('.')[0] + ".prep";
            try
            {
                // Open the text file using a stream reader.
                using StreamReader reader = new(filePath);

                // Read the stream as a string.
                string source = reader.ReadToEnd().TrimStart();

                // Write the text to the console.
                //Console.WriteLine(source);

                return source;
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read: {0}", filePath);
                Console.WriteLine(e.Message);

                return null;
            }

        }

    }

}
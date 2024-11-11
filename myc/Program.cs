// See https://aka.ms/new-console-template for more information
using myc;

internal class Program
{
    private static int Main(string[] args)
    {
        bool runLexer = false;   // Run lexer
        bool runParser = false;  // Run Lexer and Parser
        bool runCodeGen = false; // Run Lexer, Parser and Assembly Generator

        Console.WriteLine("Hello, World!");

        Console.WriteLine("\nArguments:");
        foreach (string arg in args)
        {
            Console.WriteLine("\t{0}", arg);
        }

        // program file path required as argument
        if (args.Length == 0)
        {
            Console.WriteLine("\nProgram file path missing from arguments\n");
            return -1;
        }

        // Check for arg flags
        runLexer = args.Contains("--lex");
        runParser = args.Contains("--parse");
        runCodeGen = args.Contains("--codeGen");

        string programFile = args[0];

        MyCompiler.Compile(programFile);
        return 0;

    }
}
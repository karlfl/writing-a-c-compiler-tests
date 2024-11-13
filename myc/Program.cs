// See https://aka.ms/new-console-template for more information
using myc;

internal class Program
{
    private static int Main(string[] args)
    {
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
        // --codeGen = Run Lexer, Parser and Assembly Generator
        // --parser = Run Lexer and Parser
        // --lex = Run lexer
        bool runCodeGen = args.Contains("--codeGen");
        bool runParser  = args.Contains("--parse")    || runCodeGen;
        bool runLexer   = args.Contains("--lex")      || runCodeGen || runParser;

        string programFile = args[0];

        MyCompiler.Compile(programFile, runLexer, runParser, runCodeGen);
        return 0;

    }
}